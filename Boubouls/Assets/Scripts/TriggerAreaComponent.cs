using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TriggerAreaEvent<T> : UnityEvent<T> where T : MonoBehaviour{}

[RequireComponent(typeof(Collider2D))]
public class TriggerAreaComponent<T,U> : MonoBehaviour 
    where T : MonoBehaviour 
    where U : UnityEvent<T>
{
    public Collider2D m_Collider = null;

    public U OnEnterEvent;
    public U OnExitEvent;

    private void Awake()
    {
        if (m_Collider == null)
        {
            Debug.LogError("Collider should not be null on gameObject " + gameObject.name);
            return;
        }

        if (!m_Collider.isTrigger)
        {
            m_Collider.isTrigger = true;
            Debug.LogWarning("TriggerArea object [" + name + "] collider has isTrigger set to false !");
        }
    }
    private void Start()
    {
        if (OnEnterEvent == null)
            OnEnterEvent = default;
        if (OnExitEvent == null)
            OnExitEvent = default;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        T comp = col.GetComponent<T>();
        if(comp != null && OnEnterEvent != null)
        {
            Debug.Log("[" + name + "] Target " + comp + " entered trigger");
            InvokeWithDynVal(OnEnterEvent, comp);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        T comp = col.GetComponent<T>();
        if (comp != null && OnExitEvent != null)
        {
            Debug.Log("[" + name + "] Target " + comp + " exited trigger");
            InvokeWithDynVal(OnExitEvent, comp);
        }
    }

    private static void InvokeWithDynVal(U e, T val) {
        // stupid hack !
        // see https://answers.unity.com/questions/917623/how-to-fire-unityevent-with-parameters-passed-prog.html
        for (int i = 0; i < e.GetPersistentEventCount(); i++)
        {
            ((MonoBehaviour)e.GetPersistentTarget(i)).SendMessage(e.GetPersistentMethodName(i), val);
        }
    }
}
