using UnityEngine;
using UnityEngine.UI;

public class InventoryUIComponent : MonoBehaviour
{
    public Image m_Icon;
    public Image m_Border;
    public bool m_HideIcon = false;

    public void Awake()
    {
        Debug.Assert(m_Border != null, "Missing border for inventory UI element " + name);
        Debug.Assert(m_Icon != null, "Missing icon for inventory UI element " + name);
    }

    public void Start()
    {
        Reset();
    }

    public void Fill()
    {
        m_Icon.enabled = true;
        m_Border.color = Color.green;
    }

    public void Reset()
    {
        m_Icon.enabled = !m_HideIcon;
        m_Border.color = Color.red;
    }
}
