using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public string[] m_Scenes = new string[] { "GameScene", "NextGameScene" };

    public int m_CurrentSceneIndex = 0;
    public CanvasGroup m_FaderCanvasGroup;
    public float m_FadeDuration = 2.0f;
    public bool m_IsFading = false;

    UnityEvent OnFadeFinished;

    void Awake()
    {
        OnFadeFinished = new UnityEvent();
    }

    void Start()
    {
        
        Debug.Log("Application started at " + FormatDate());
        DontDestroyOnLoad(this);
        FadeIn();

        DisableInputs();
        OnFadeFinished.AddListener(OnBeginFadeFinished);
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
            }
        }
        Debug.Log("Inputs enabled successfully!");
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

    IEnumerator LoadNextScene()
    {
        yield return StartCoroutine(LoadSceneAndSetActive(m_Scenes[m_CurrentSceneIndex++]));
    }

    public void OnLevelFinished()
    {
        FadeOut();
    }

    public IEnumerator AsynchronousLoadAndUnload(string scene, int index)
    {
        yield return null;
        AsyncOperation ao = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            // [0, 0.9] > [0, 1]
            float progress = Mathf.Clamp01(ao.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            // Loading completed
            if (ao.progress == 0.9f)
            {
                ao.allowSceneActivation = true;
            }

            yield return null;
            SceneManager.UnloadSceneAsync(index);
        }
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
