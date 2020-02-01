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

    public U m_OnEnter;
    public U m_OnExit;

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
        if (m_OnEnter == null)
            m_OnEnter = default;
        if (m_OnExit == null)
            m_OnExit = default;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        T comp = col.GetComponent<T>();
        if(comp != null && m_OnEnter != null)
        {
            Debug.Log("[" + name + "] Target " + comp + " entered trigger");
            InvokeWithDynVal(m_OnEnter, comp);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        T comp = col.GetComponent<T>();
        if (comp != null && m_OnExit != null)
        {
            Debug.Log("[" + name + "] Target " + comp + " exited trigger");
            InvokeWithDynVal(m_OnExit, comp);
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
