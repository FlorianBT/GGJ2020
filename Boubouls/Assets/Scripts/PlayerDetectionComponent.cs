using UnityEngine;

[RequireComponent(typeof(PlayerComponent))]
public class PlayerDetectionComponent : MonoBehaviour
{
    public PlayerComponent m_Player;

    private void Start()
    {
        m_Player = GetComponent<PlayerComponent>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Detected an interactive, register it in the Player
        Debug.Log("Detected collision with " + col);

        InteractiveComponent comp = col.gameObject.GetComponent<InteractiveComponent>();
        if (comp != null)
        {
            m_Player.RegisterInteractive(comp);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        //Left the interactive AOE, unregister it
        Debug.Log("Exiting collision with " + col);

        InteractiveComponent comp = col.gameObject.GetComponent<InteractiveComponent>();
        if (comp != null)
        {
            m_Player.ForgetInteractive(comp);
        }
    }
}
