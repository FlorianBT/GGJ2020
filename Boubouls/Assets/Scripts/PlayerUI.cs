using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image m_AimPointer;
    public Vector2 m_PosOffset2D = new Vector2(0.0f, 0.0f);
    public float m_Radius = 60.0f;
    public PlayerComponent m_PlayerComponent;

    private void Awake()
    {
        if (m_PlayerComponent == null)
        {
            Debug.LogWarning("PlayerUI requires PlayerComponent reference.");
            return;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        UpdateAimPointerPosition();
    }

    void UpdateAimPointerPosition()
    {
        Vector2 playerPos = m_PlayerComponent.transform.position;
        Vector2 aimDir = m_PlayerComponent.m_AimingDir;
        Debug.Log(aimDir);
        if (aimDir == Vector2.zero)
        {
            m_AimPointer.enabled = false;
        }
        else
        {
            m_AimPointer.enabled = true;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(playerPos);
            m_AimPointer.rectTransform.anchoredPosition = screenPos + aimDir * m_Radius + m_PosOffset2D;
        }
    }
}
