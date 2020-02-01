using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public string m_PlayerTag = "Player";
    public Image m_AimPointer;
    public Vector2 m_PosOffset2D = new Vector2(0.0f, 0.0f);
    public float m_Radius = 60.0f;
    private PlayerComponent m_PlayerComponent = null;
    public ControlIconComponent m_InteractControlIcon;
    public Vector2 m_ControlIconOffset2D = new Vector2(0.0f, 2.0f);

    private void Awake()
    {
        EventManager.StartListening("LocalPlayerSpawned", OnLocalPlayerSpawned);
    }

    void OnLocalPlayerSpawned()
    {
        GameObject player = GameObject.FindGameObjectWithTag(m_PlayerTag);
        if (player == null)
        {
            Debug.LogError("Player not found");
            return;
        }

        m_PlayerComponent = player.GetComponent<PlayerComponent>();
    }


    void Update()
    {
        if (m_PlayerComponent == null)
            return;

        UpdateAimPointerPosition();
        UpdateClosestInteractive();
    }

    void UpdateAimPointerPosition()
    {
        Vector2 playerPos = m_PlayerComponent.transform.position;
        Vector2 aimDir = m_PlayerComponent.m_AimingDir;

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

    void UpdateClosestInteractive()
    {
        InteractiveComponent inter = m_PlayerComponent.ClosestInteractive;
        if (inter != null)
        {
            m_InteractControlIcon.SetVisibility(true);
            m_InteractControlIcon.AlignOn(inter, m_ControlIconOffset2D);
            if (inter.holdDuration > 0f && m_PlayerComponent.InteractDuration > 0f)
            {
                 m_InteractControlIcon.Fill(m_PlayerComponent.InteractDuration / inter.holdDuration);
            }
        }
        else
        {
            m_InteractControlIcon.SetVisibility(false);
        }
    }
}
