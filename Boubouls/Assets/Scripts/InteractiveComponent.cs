using UnityEngine;

public abstract class InteractiveComponent : MonoBehaviour
{
    public bool Used
    {
        get; private set;
    }

    public abstract void Interact();
}