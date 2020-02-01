using UnityEngine;
using System.Collections.Generic;

public class PlayerComponent : MonoBehaviour
{
    public float speed = 5f;
    
    private List<InteractiveComponent> m_InteractivesInRange;

    private void Awake()
    {
        m_InteractivesInRange = new List<InteractiveComponent>();
    }

    private void Update()
    {
        float xDiff = 0f;
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            xDiff -= speed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            xDiff += speed;
        }
        transform.Translate(new Vector3(xDiff * Time.deltaTime, 0f, 0f));

        if (Input.GetKey(KeyCode.Space))
        {
            foreach(InteractiveComponent i in m_InteractivesInRange)
            {
                i.Interact();
            }
        }
    }

    public void RegisterInteractive(InteractiveComponent inter)
    {
        Debug.Log("Adding interactive " + inter);
        if(!m_InteractivesInRange.Contains(inter))
            m_InteractivesInRange.Add(inter);
    }

    public void ForgetInteractive(InteractiveComponent inter)
    {
        Debug.Log("Removing interactive " + inter);
        m_InteractivesInRange.Remove(inter);
    }
}
