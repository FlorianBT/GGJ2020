using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private string[] m_Scenes = new string[] { "FirstScene", "SecondScene", "Tibo", "Marie", "Final" };

    public int m_CurrentSceneIndex = 0;
    public CanvasGroup m_FaderCanvasGroup;
    public float m_FadeDuration = 2.0f;
    public bool m_IsFading = false;
    public PlayerComponent m_PlayerPrefab = null;
    private PlayerComponent m_PlayerRef = null;

    public InputActionAsset m_InputActionAsset = null;

    UnityEvent OnFadeFinished;

    void Awake()
    {
        OnFadeFinished = new UnityEvent();
    }

    void Start()
    {
        Debug.Log("Application started at " + FormatDate());
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(m_FaderCanvasGroup.transform.parent);
        SpawnPlayer();

        OnBeginNewScene();
    }

    void SpawnPlayer()
    {
        m_PlayerRef = Instantiate<PlayerComponent>(m_PlayerPrefab);
        m_PlayerRef.gameObject.name = "LocalPlayer";
        PlayerInput playerInput = m_PlayerRef.GetComponent<PlayerInput>();
        playerInput.actions = m_InputActionAsset;
        DontDestroyOnLoad(m_PlayerRef);
    }

    void TeleportPlayerToSpawnPoint()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        m_PlayerRef.transform.position = spawnPoint.transform.position;
        Debug.Log("Player has been teleported to this location: " + spawnPoint.name);

        EventManager.TriggerEvent("LocalPlayerSpawned");
    }

    void OnBeginNewScene()
    {
        TeleportPlayerToSpawnPoint();
        FadeIn();

        DisableInputs();
        OnFadeFinished.AddListener(OnBeginFadeFinished);

        EventManager.StartListening("ArtifactDestroyed", OnArtifactDestroyed);
    }

    void OnArtifactDestroyed()
    {
        EventManager.StopListening("ArtifactDestroyed", OnArtifactDestroyed);

        Debug.Log("[GAMEMANAGER] On Artifact Destroyed!");
        m_PlayerRef.UsePieces();

        OnLevelFinished();
    }

    void DisableInputs()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap("UI");
            }
        }
        Debug.Log("Inputs disabled successfully!");
    }

    void EnableInputs()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap("Player");
                Debug.Log("Inputs enabled successfully!");
                return;
            }
        }
        Debug.Log("Inputs not enabled.");
    }

    void OnBeginFadeFinished()
    {
        EnableInputs();
        OnFadeFinished.RemoveListener(OnBeginFadeFinished);
    }

    void FadeIn()
    {
        m_FaderCanvasGroup.alpha = 1.0f;
        StartCoroutine(Fade(0.0f));
    }

    void FadeOut()
    {
        m_FaderCanvasGroup.alpha = 0.0f;
        StartCoroutine(Fade(1.0f));
    }

    void LoadNextScene()
    {
        int newSceneIndex = m_CurrentSceneIndex + 1;
        StartCoroutine(AsynchronousLoadAndUnload(m_Scenes[newSceneIndex], m_Scenes[m_CurrentSceneIndex]));
        m_CurrentSceneIndex = newSceneIndex;
    }

    public void OnLevelFinished()
    {
        DisableInputs();
        FadeOut();
        OnFadeFinished.AddListener(OnEndLevelFadeOutFinished);
    }

    public void OnEndLevelFadeOutFinished()
    {
        Debug.Log("OnEndLevelFadeOutFinished");
        OnFadeFinished.RemoveListener(OnEndLevelFadeOutFinished);
        LoadNextScene();
    }

    public IEnumerator AsynchronousLoadAndUnload(string scene, string oldScene)
    {
        yield return null;
        AsyncOperation loadAO = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        loadAO.allowSceneActivation = false;

        while (!loadAO.isDone)
        {
            // [0, 0.9] > [0, 1]
            float progress = Mathf.Clamp01(loadAO.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            // Loading completed
            if (loadAO.progress == 0.9f)
            {
                loadAO.allowSceneActivation = true;
            }

            yield return null;

        }

        OnBeginNewScene();
    }

    public IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newScene);
    }

    public IEnumerator Fade(float finalAlpha)
    {
        m_IsFading = true;
        m_FaderCanvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(m_FaderCanvasGroup.alpha - finalAlpha) / m_FadeDuration;
        while (!Mathf.Approximately(m_FaderCanvasGroup.alpha, finalAlpha))
        {
            m_FaderCanvasGroup.alpha = Mathf.MoveTowards(m_FaderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        m_IsFading = false;
        m_FaderCanvasGroup.blocksRaycasts = false;

        OnFadeFinished.Invoke();
    }

    public string FormatDate()
    {
        return ((System.DateTime.Now.Hour.ToString().Length == 1) ? "0" + System.DateTime.Now.Hour.ToString() : System.DateTime.Now.Hour.ToString()) + ":" + System.DateTime.Now.Minute.ToString() + ":" + System.DateTime.Now.Second.ToString();
    }
}
