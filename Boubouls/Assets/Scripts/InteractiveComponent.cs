using UnityEngine;

public abstract class InteractiveComponent : MonoBehaviour
{
    public float holdDuration = 2.5f;

    public bool Used
    {
        get; protected set;
    }

    public abstract void Interact(float elapsedTime);
}