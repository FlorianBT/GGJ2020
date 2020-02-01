using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class ControlIconComponent : MonoBehaviour
{
    Image m_Icon;
    public Image m_Filler;

    private void Start()
    {
        m_Icon = GetComponent<Image>();
        if(m_Filler == null)
        {
            Debug.LogWarning("Missing filler on ControlIcon " + name, this);
        }
    }

    public void SetVisibility(bool flag)
    {
        m_Icon.enabled = flag;
        if (m_Filler != null)
            m_Filler.enabled = flag;
    }

    public void AlignOn(InteractiveComponent inter, Vector2 offset)
    {
        Vector2 interPos = inter.transform.position;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(interPos + offset);
        m_Icon.rectTransform.anchoredPosition = screenPos;
    }

    public void Fill(float ratio)
    {
        if (m_Filler == null) return;
        ratio = Mathf.Clamp01(ratio);
        m_Filler.fillAmount = ratio;
    }
}
