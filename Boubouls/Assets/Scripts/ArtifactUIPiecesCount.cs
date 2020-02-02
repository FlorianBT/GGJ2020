using UnityEngine;
using UnityEngine.UI;

public class ArtifactUIPiecesCount : MonoBehaviour
{
    public Image m_Image;
    public Text m_Text;

    private void Awake()
    {
        Debug.Assert(m_Image != null, "MISSING IMAGE REF in Artifact UI");
        Debug.Assert(m_Text != null, "MISSING TEXT REF in Artifact UI");
    }

    private void Start()
    {
        m_Text.text = "0";
    }

    public void SetCount(int v)
    {
        m_Text.text = Mathf.Abs(v).ToString();
    }

    public void AlignOn(ArtifactComponent artifact, Vector2 offset)
    {
        Vector2 interPos = artifact.transform.position;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(interPos + offset);
        GetComponent<RectTransform>().anchoredPosition = screenPos;
    }

    public void SetVisibility(bool flag)
    {
        gameObject.SetActive(flag);
    }
}
