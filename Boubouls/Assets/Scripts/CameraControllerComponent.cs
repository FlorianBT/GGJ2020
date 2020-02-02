using System.Collections;
using UnityEngine;

public class CameraControllerComponent : MonoBehaviour
{
    public Vector2 m_DeadZone = Vector2.zero;
    public int m_LerpDurationMS = 750;

    private GameObject m_Player;
    private Vector2 m_LookAt;
    private Camera m_Camera;
    private float m_LerpStart = -1f;

    [Header("Shake")]
    // How long the object should shake for.
    public float shakeDuration = 0.5f;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;

    private Vector2 PlayerPos {
        get {
            return m_Player.transform.position;
        }
        set {
            m_Player.transform.position = value;
        }
    }
    private Vector2 CamPos {
        get {
            return m_Camera.transform.position;
        }
        set {
           Vector3 newPos = new Vector3(value.x, value.y, m_Camera.transform.position.z);
            m_Camera.transform.position = newPos;
        }
    }

    private void Start()
    {
        m_Camera = GetComponent<Camera>();
        Debug.Assert(m_Camera != null, "Camera Controller not attached to a camera");

        m_DeadZone = new Vector2(Mathf.Max(Mathf.Abs(m_DeadZone.x),1f), Mathf.Max(Mathf.Abs(m_DeadZone.y), 1f));

        EventManager.StartListening("Shake", OnShakeEvent);
        Debug.Log("Listening Shake event");

        EventManager.StartListening("LocalPlayerSpawned", OnLocalPlayerSpawned);
    }

    void OnLocalPlayerSpawned()
    {
        EventManager.StopListening("LocalPlayerSpawned", OnLocalPlayerSpawned);

        m_Player = GameObject.FindWithTag("Player");
        Debug.Assert(m_Player != null, "Camera Controller could not find a valid Player in the scene");
        CamPos = PlayerPos;

    }

    private void Stop()
    {
        Debug.Log("Unlistening Shake event");
        EventManager.StopListening("Shake", OnShakeEvent);
    }
    
    private void Update()
    {
        if (m_Player == null)
            return;

        if(m_LerpStart >= 0f) //Transitioning to player
        {
            LockOnPlayer();
        }
        else if(IsOutsideDeadzone())
        {
            m_LerpStart = Time.time;
        }
    }

    private bool IsOutsideDeadzone()
    {
        Vector2 delta = PlayerPos - CamPos;
        bool xOver = Mathf.Abs(delta.x) > m_DeadZone.x;
        bool yOver = Mathf.Abs(delta.y) > m_DeadZone.y;
        return (xOver || yOver);
    }

    private void LockOnPlayer()
    {
        float elapsed = Time.time - m_LerpStart;
        float ratio = Mathf.Clamp01(elapsed / (m_LerpDurationMS / 1000f));

        CamPos = Vector2.Lerp(CamPos, PlayerPos, ratio);

        if(Vector2.Distance(PlayerPos,CamPos) <= Mathf.Epsilon)
        {
            m_LerpStart = -1f;
        }
    }

    void OnShakeEvent()
    {
        Debug.Log("Shake requested, lets go!");
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        Vector3 originalPos = transform.localPosition;
        float shakeRemainingTime = shakeDuration;

        while (shakeRemainingTime > 0.0f)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeRemainingTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = originalPos;
    }
}
