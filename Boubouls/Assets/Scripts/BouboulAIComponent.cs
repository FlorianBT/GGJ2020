using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BouboulAIComponent : MonoBehaviour
{
    public string m_PlayerTag = "Player";
    public float m_MovementSpeed = 4.0f;

    private PlayerComponent m_Player = null;
    private Rigidbody2D m_Rigidbody = null;
    [SerializeField]
    public SpriteRenderer m_SpriteRenderer = null;
    public Animator m_Animator = null;

    public Vector2 m_Velocity = new Vector2(0.0f, 0.0f);

    public Vector2 m_delayBeforeJump = new Vector2(0.7f, 1.2f);
    public float m_delayBeforeJumpTimer = 0.0f;

    public float m_MaxDistanceWithPlayer = 2.5f;
    public float m_MinDistanceWithPlayer = 0.5f;


    public Vector2 m_JumpForce = new Vector2(130, 170);

    public bool m_IsCarried = false;
    public bool m_isOwned = false;

    [Header("Debug")]
    [SerializeField]
    private bool m_OnGround = false;
    [SerializeField]
    private bool m_IsFollowing = false;
    
    float Map(float value, float min1, float max1, float min2, float max2)
    {
        float clampedValue = Mathf.Min(max1, Mathf.Max(min1, value));
        float cursor = clampedValue / (max1 - min1);
        return min2 + (max2 - min2) * cursor;
    }

    float GetDelayBeforeJump()
    {
        if (m_IsFollowing)
            return 0.0f;
        float currentSqrSpeed = m_Rigidbody.velocity.sqrMagnitude;
        float delayBeforeJumpFactor = Map(currentSqrSpeed, 0.0f, 12.0f, 1.0f, 0.15f);
        return Random.Range(m_delayBeforeJump.x, m_delayBeforeJump.y) * delayBeforeJumpFactor;
    }

    float GetJumpForce()
    {
        float jumpForce = Random.Range(m_JumpForce.x, m_JumpForce.y);
        float jumpForceFactor = Map(m_Rigidbody.velocity.sqrMagnitude, 0.0f, 12.0f, 1.0f, 0.25f);
        return jumpForce * jumpForceFactor;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    void Jump()
    {
        m_OnGround = false;
        Quaternion rotation = Quaternion.identity;
        if (m_isOwned)
        {
            rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(-15.0f, 15.0f));
        }
        m_Rigidbody.AddForce(rotation * transform.up * GetJumpForce());
        if (m_Animator != null)
        {
            m_Animator.SetTrigger("Jump");
        }
    }

    void Update()
    {
        if (m_IsCarried)
            return;

        UpdateSpriteRenderer();
        UpdateTimers();

        if (m_OnGround && m_delayBeforeJumpTimer > GetDelayBeforeJump())
        {
            Jump();
        }

        if (m_isOwned && m_Player != null)
        {
            Vector2 meToPlayer = (m_Player.transform.position - transform.position);
            meToPlayer.y = 0;
            if (!m_IsFollowing && meToPlayer.sqrMagnitude > Mathf.Pow(m_MaxDistanceWithPlayer, 2.0f))
            {
                m_IsFollowing = true;
            }

            if (m_IsFollowing)
            {
                Vector2 dir = meToPlayer.normalized;
                m_Velocity = dir * m_MovementSpeed;
                if (meToPlayer.sqrMagnitude <= Mathf.Pow(m_MinDistanceWithPlayer, 2.0f))
                {
                    m_Rigidbody.velocity = new Vector2(0.0f, m_Rigidbody.velocity.y);
                    m_IsFollowing = false;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_IsCarried)
            return;

        if (m_IsFollowing)
        {
            m_Rigidbody.AddForce(m_Velocity);
        }

        float maxHorizontalVelocity = 4.0f;
        if (Mathf.Abs(m_Rigidbody.velocity.x) > maxHorizontalVelocity)
        {
            m_Rigidbody.velocity = new Vector2(Mathf.Sign(m_Rigidbody.velocity.x) * maxHorizontalVelocity, m_Rigidbody.velocity.y);
        }

    }
    void UpdateTimers()
    {
        m_delayBeforeJumpTimer += Time.deltaTime;
    }

    void UpdateSpriteRenderer()
    {
        if (m_SpriteRenderer == null)
            return;
        if (m_Rigidbody.velocity != Vector2.zero)
        {
            if (Vector3.Dot(m_Rigidbody.velocity, Vector3.right) > 0.0f)
            {
                m_SpriteRenderer.flipX = false;
            }
            else
            {
                m_SpriteRenderer.flipX = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool collisionFromBottom = false;
        if (collision.contacts.Length > 0)
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                ContactPoint2D contact = collision.contacts[0];
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.5)
                {
                    collisionFromBottom = true;
                }
            }
        }

        if (collisionFromBottom)
        {
            m_OnGround = true;
            m_delayBeforeJumpTimer = 0.0f;
            if (m_Animator != null)
            {
                m_Animator.SetTrigger("OnGround");
            }
        }
    }

    public void OnCollectedByPlayer(PlayerComponent playerComponent)
    {
        Debug.Log("Collected By Player");
        m_Player = playerComponent;
        m_isOwned = true;
    }

    public void Explode()
    {
        //TODO spawn death animation
        GameObject.Destroy(gameObject);
    }
}
