using UnityEngine;

public class PlayerDetectionComponent : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        //Detected an interactive, register it in the Player
        Debug.Log("Detected collision with " + col);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        //Left the interactive AOE, unregister it
        Debug.Log("Exiting collision with " + col);
    }
}
