using UnityEngine;

public class CameraControllerComponent : MonoBehaviour
{
    public Bounds m_DeadZone;
    //public Bounds m_Bounds;
    public int m_DampDurationMS = 750;

    private GameObject m_Player;
    private Camera m_Camera;

    private Vector3 m_DeltaCenterVec;

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

        m_Player = GameObject.FindWithTag("Player");
        Debug.Assert(m_Player != null, "Camera Controller could not find a valid Player in the scene");
        
        CamPos = PlayerPos;

        m_DeltaCenterVec = VPToWPoint(new Vector3(0.5f, 0.5f, 0)) - VPToWPoint(m_DeadZone.center);
    }
    
    private void LateUpdate()
    {
        Vector3 tempVec = Vector3.zero;
        Vector2 delta = PlayerPos - VPToWPoint(m_DeadZone.center);
        Vector3 destination = CamPos + delta;

        Vector3 dummy = Vector3.zero;
        tempVec = Vector3.SmoothDamp(transform.position, destination, ref dummy, m_DampDurationMS / 1000f);

        float halfDZWidth = m_DeadZone.extents.x * 0.5f;
        if (delta.x > halfDZWidth)
        {
            tempVec.x = PlayerPos.x - halfDZWidth + m_DeltaCenterVec.x;
        }
        if (delta.x < -halfDZWidth)
        {
            tempVec.x = PlayerPos.x + halfDZWidth + m_DeltaCenterVec.x;
        }

        float halfDZHeight = m_DeadZone.extents.y * 0.5f;
        if (delta.y > halfDZHeight)
        {
            tempVec.y = PlayerPos.y - halfDZHeight + m_DeltaCenterVec.y;
        }
        if (delta.y < -halfDZHeight)
        {
            tempVec.y = PlayerPos.y + halfDZHeight + m_DeltaCenterVec.y;
        }

        /*
        if (isBoundHorizontal)
        {
            tempVec.x = Mathf.Clamp(tempVec.x, leftBound + horzExtent, rightBound - horzExtent);
        }

        if (isBoundVertical)
        {
            tempVec.y = Mathf.Clamp(tempVec.y, lowerBound + vertExtent, upperBound - vertExtent);
        }
        */
        CamPos = tempVec;
    }

    private Vector2 VPToWPoint(Vector2 worldPoint)
    {
        return m_Camera.ViewportToWorldPoint(worldPoint);
    }
}
