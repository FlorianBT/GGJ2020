using UnityEngine;

public class BouboulComponent : MonoBehaviour
{
    public bool Collected { get; private set; }
    public void Collect()
    {
        Debug.Log("COLLECTING BOUBOUL : " + name);
        Collected = true;
    }

    private void Awake()
    {
        Collected = false;
    }
}
