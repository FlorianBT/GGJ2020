using UnityEngine;

public class BouboulComponent : MonoBehaviour, ICollectible
{
    public void Collect()
    {
        Debug.Log("COLLECTING " + name);
    }
}
