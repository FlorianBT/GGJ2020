using UnityEngine;

public class CameraControllerComponent : MonoBehaviour
{
    public Bounds m_DeadZone;
    public int m_DampDurationMS = 750;

    private GameObject m_Player;
    private Camera m_Camera;

    private Vector3 m_DeltaCenterVec;
    private Vector2 m_CamExtents;
    private Bounds m_WorldBounds;

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

    private bool WorldBound {
        get { return m_WorldBounds != null && (m_WorldBounds.extents.x > 0f || m_WorldBounds.extents.y > 0f); }
    }

    private bool HasDeadZone {
        get { return m_DeadZone != null && (m_DeadZone.extents.x > 0f || m_DeadZone.extents.y > 0f); }
    }
 
    private void Start()
    {
        m_Camera = GetComponent<Camera>();
        Debug.Assert(m_Camera != null, "Camera Controller not attached to a camera");

        m_Player = GameObject.FindWithTag("Player");
        Debug.Assert(m_Player != null, "Camera Controller could not find a valid Player in the scene");
        
        m_DeltaCenterVec = VPToWPoint(new Vector3(0.5f, 0.5f, 0)) - VPToWPoint(m_DeadZone.center);

        float h = m_Camera.orthographicSize;
        m_CamExtents = new Vector2(h * (Screen.width / Screen.height), h);

        CamPos = PlayerPos;
    }
    
    private void LateUpdate()
    {
        Vector3 tempVec = Vector3.zero;
        Vector2 delta = PlayerPos - VPToWPoint(m_DeadZone.center);
        Vector3 destination = CamPos + delta;

        Vector3 dummy = Vector3.zero;
        tempVec = Vector3.SmoothDamp(transform.position, destination, ref dummy, m_DampDurationMS / 1000f);

        if(HasDeadZone)
        {
            // Check player in Dead Zone
            if (delta.x > m_DeadZone.extents.x)
            {
                tempVec.x = PlayerPos.x - m_DeadZone.extents.x + m_DeltaCenterVec.x;
            }
            if (delta.x < -m_DeadZone.extents.x)
            {
                tempVec.x = PlayerPos.x + m_DeadZone.extents.x + m_DeltaCenterVec.x;
            }

            if (delta.y > m_DeadZone.extents.y)
            {
                tempVec.y = PlayerPos.y - m_DeadZone.extents.y + m_DeltaCenterVec.y;
            }
            if (delta.y < -m_DeadZone.extents.y)
            {
                tempVec.y = PlayerPos.y + m_DeadZone.extents.y + m_DeltaCenterVec.y;
            }
        }

        if (WorldBound)
        {
            //Clamp to World
            //TODO check world bounds better
            tempVec.x = Mathf.Clamp(tempVec.x, -m_WorldBounds.extents.x + m_CamExtents.x, m_WorldBounds.extents.x - m_CamExtents.x);
            tempVec.y = Mathf.Clamp(tempVec.y, -m_WorldBounds.extents.y + m_CamExtents.y, m_WorldBounds.extents.y - m_CamExtents.y);
        }

        CamPos = tempVec;
    }

    private Vector2 VPToWPoint(Vector2 worldPoint)
    {
        return m_Camera.ViewportToWorldPoint(worldPoint);
    }

    public void OnLevelLoaded(LevelComponent lvl)
    {
        m_WorldBounds = lvl.GetTotalBounds();
    }
}
