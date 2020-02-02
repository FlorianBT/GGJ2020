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
    public ArtifactUIPiecesCount m_ArtifactUI;
    public InventoryUIComponent[] m_Inventories;

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
        UpdateArtifact();
        UpdateInventories();
    }

    void UpdateAimPointerPosition()
    {
        Vector2 muzzlePos = m_PlayerComponent.GetMuzzlePosition();
        Vector2 aimDir = m_PlayerComponent.m_AimingDir;

        if (aimDir == Vector2.zero)
        {
            m_AimPointer.enabled = false;
        }
        else
        {
            m_AimPointer.enabled = true;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(muzzlePos);
            m_AimPointer.rectTransform.anchoredPosition = screenPos + aimDir * m_Radius;// + m_PosOffset2D;
        }
    }

    void UpdateClosestInteractive()
    {
        InteractiveComponent inter = m_PlayerComponent.ClosestInteractive;
        if (inter != null && inter.CanInteract())
        {
            m_InteractControlIcon.SetVisibility(true);
            m_InteractControlIcon.AlignOn(inter, m_ControlIconOffset2D);
            if (inter.holdDuration > 0f)
            {
                 m_InteractControlIcon.Fill(m_PlayerComponent.InteractDuration / inter.holdDuration);
            }
        }
        else
        {
            m_InteractControlIcon.SetVisibility(false);
        }
    }

    void UpdateArtifact()
    {
        m_ArtifactUI.SetVisibility(false);

        if (m_PlayerComponent.ClosestInteractive == null) return;

        ArtifactComponent artifact = m_PlayerComponent.ClosestInteractive.GetComponent<ArtifactComponent>();
        if(artifact == null) return;

        if(!artifact.CanInteract())
        {
            int missing = 3 - m_PlayerComponent.PiecesOwned;
            m_ArtifactUI.SetCount(missing);
            m_ArtifactUI.SetVisibility(true);
            m_ArtifactUI.AlignOn(artifact, m_ControlIconOffset2D);
        }
    }

    void UpdateInventories()
    {
        int m = Mathf.Min(m_Inventories.Length, m_PlayerComponent.PiecesOwned);
        for(int i = 0; i < m; ++i)
        {
            m_Inventories[i].Fill();
        }

        for (int j = m; j < 3; ++j)
        {
            m_Inventories[j].Reset();
        }
    }
}
