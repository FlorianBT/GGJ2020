using UnityEngine;

public class SwitchComponent : InteractiveComponent
{
    public override void Interact()
    {
        Debug.Log("Interacting with " + name);
    }
}
