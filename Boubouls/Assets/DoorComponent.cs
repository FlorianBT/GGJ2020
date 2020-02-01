using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DoorComponent : MonoBehaviour
{
    public Vector2 m_SlideDir = Vector2.zero;
    public float m_SlideDuration = 2f;
    public bool m_StartOpen = false;

    public bool Opened { get; private set; }
    public bool Transitioning {
        get { return m_StartTransition >= 0f; }
    }

    private float m_StartTransition = -1f;
    private Vector2 m_ClosedPos = Vector2.zero;
    private Vector2 m_OpenedPos = Vector2.zero;
    private SpriteRenderer m_SpriteRenderer;

    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        m_ClosedPos = transform.position;
        Vector2 displacement = m_SlideDir;
        displacement.Scale(m_SpriteRenderer.bounds.size);
        m_OpenedPos = m_ClosedPos + displacement;

        if (m_StartOpen)
        {
            Toggle(true);
        }
    }

    void Update()
    {
        if(Transitioning)
        {
            float elapsed = Time.time - m_StartTransition;
            transform.position = CalculatePosAt(elapsed / m_SlideDuration, !Opened);
            if(elapsed >= m_SlideDuration)
            {
                m_StartTransition = -1f;
                Opened = !Opened;
            }
        }
    }

    public void Toggle(bool instant = false)
    {
        Debug.Log("Toggle");
        if (Transitioning) return;

        if(instant)
        {
            transform.position = CalculatePosAt(1f, !Opened);
            Opened = !Opened;
            m_StartTransition = -1f;
        }
        else
        {
            Debug.Log("m_StartTransition set to " + Time.time);
            m_StartTransition = Time.time;
        }
    }

    private Vector2 CalculatePosAt(float ratio, bool opening)
    {
        ratio = Mathf.Clamp01(ratio);
        Vector2 initialPos = opening ? m_ClosedPos : m_OpenedPos;
        Vector2 finalPos = opening ? m_OpenedPos : m_ClosedPos;
        return Vector2.Lerp(initialPos, finalPos, ratio);
    }
}
