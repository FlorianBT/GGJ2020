using UnityEngine;

public class ArtifactPieceComponent : MonoBehaviour
{
    public int m_CollectTransitionDurationMS = 750;
    public float m_RotationSpeed = 180f;
    public float m_BounceHeight = 2f;

    private PlayerComponent m_Target = null;
    private float m_CollectTime = 0f;
     
    private void Update()
    {
        if(m_Target != null)
        {
            float elapsed = Time.time - m_CollectTime;
            float ratio = elapsed / (m_CollectTransitionDurationMS / 1000f);
            transform.position = Vector3.Lerp(transform.position, m_Target.transform.position, ratio);

            if(Vector3.Distance(transform.position, m_Target.transform.position) < 0.2f) //cheating anim
            {
                enabled = false;
                m_Target.OnCollectedPiece(this);
                Destroy(gameObject);
            }
        }
        else
        {
            transform.Rotate(Vector3.back, m_RotationSpeed * Time.deltaTime);
            transform.localPosition = new Vector3(transform.localPosition.x, (1f + Mathf.Sin(Time.time)) * m_BounceHeight, transform.localPosition.z);
        }
    }

    public void OnPickedUp(PlayerComponent player)
    {
        m_Target = player;
        m_CollectTime = Time.time;
    }
}
