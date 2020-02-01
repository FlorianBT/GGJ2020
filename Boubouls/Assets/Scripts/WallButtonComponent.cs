using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class WallButtonComponent : MonoBehaviour
{
    public UnityEvent OnPressedEvent;

    private Collider2D m_Collider2D;
    private bool m_Pressed = false;

    private void Start()
    {
        m_Collider2D = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BouboulAIComponent b = collision.gameObject.GetComponent<BouboulAIComponent>();
        if(b != null && !m_Pressed)
        {
            OnPressedEvent.Invoke();
            b.Explode(); //TODO button's inverse direction
            m_Pressed = true;
            //TODO push button back
        }
    }
}
