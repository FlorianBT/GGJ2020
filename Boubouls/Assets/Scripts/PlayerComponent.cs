using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerComponent : MonoBehaviour
{
    public float m_Speed = 50.0f;
    public float m_RunSpeed = 110.0f;
    public float m_JumpForce = 250.0f;
    public uint m_AllowedJumpCount = 2;
    public uint m_CurrentJumpCount = 0;

    public Rigidbody2D m_ProjectileGameObject = null;
    public float m_ProjectileForce = 500.0f;
    public GameObject m_MuzzleDummy;

    public SpriteRenderer m_SpriteRenderer = null;
    public Animator m_Animator = null;

    public Vector2 m_AimingDir { get; private set; } = new Vector2();
    private Vector2 m_MousePosition = new Vector2();
    private Rigidbody2D m_Rigidbody2D = null;

    public ParticleSystem m_dirtParticleSystem = null;
    public Transform m_DirtParticleLeftPos = null;
    public Transform m_DirtParticleRightPos = null;

    public float m_BouboulResearchRadius = 2.0f;

    public int PiecesOwned { get; private set; }

    [Header("Debug")]
    [SerializeField]
    private bool m_IsRunning = false;
    [SerializeField]
    private bool m_OnGround = true;
    [SerializeField]
    private Vector2 m_GroundRight = new Vector2(1.0f, 0.0f);
    [SerializeField]
    private Vector2 m_Velocity = new Vector2();

    private List<InteractiveComponent> m_InteractivesInRange;
    private float m_InteractStart = -1f;

    private void Awake()
    {
        m_InteractivesInRange = new List<InteractiveComponent>();
    }

    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Reset()
    {
        FlipRight();
    }

    void UpdateSpriteRenderer()
    {
        float horizontalVelocity = Mathf.Abs(m_Rigidbody2D.velocity.x);
        m_Animator.SetFloat("Speed", horizontalVelocity);

        if (horizontalVelocity > Mathf.Epsilon)
        {
            if (Vector3.Dot(m_Rigidbody2D.velocity.normalized, Vector3.right) >= Mathf.Epsilon)
            {
                m_SpriteRenderer.flipX = false;
                FlipRight();
            }
            else
            {
                FlipLeft();
            }

        }

        if (horizontalVelocity >= 0.5f && m_OnGround)
        {
            if (!m_dirtParticleSystem.isPlaying)
            {
                m_dirtParticleSystem.Play();
            }
        }
        else
        {
            m_dirtParticleSystem.Stop();
        }
    }

    public float GetCurrentSpeed()
    {
        if (m_IsRunning)
        {
            return m_RunSpeed;
        }
        else
        {
            return m_Speed;
        }

    }

    private void Update()
    {
        UpdateSpriteRenderer();

        if (ClosestInteractive != null && m_InteractStart >= 0f)
        {
            ClosestInteractive.Interact(Time.time - m_InteractStart);
        }

        m_InteractivesInRange.RemoveAll((inter) => { return inter.Used; });
    }

    void FlipLeft()
    {
        m_SpriteRenderer.flipX = true;
        m_dirtParticleSystem.transform.position = m_DirtParticleRightPos.transform.position;
        m_dirtParticleSystem.transform.rotation = m_DirtParticleRightPos.transform.rotation;
    }

    void FlipRight()
    {
        m_SpriteRenderer.flipX = false;
        m_dirtParticleSystem.transform.position = m_DirtParticleLeftPos.transform.position;
        m_dirtParticleSystem.transform.rotation = m_DirtParticleLeftPos.transform.rotation;
    }

    void FixedUpdate()
    {
        if (m_Velocity != Vector2.zero)
        {
            m_Rigidbody2D.AddForce(m_Velocity * GetCurrentSpeed());
        }
        
        if (Mathf.Abs(m_Rigidbody2D.velocity.x) > 5.0f)
        {
            m_Rigidbody2D.velocity = new Vector2(Mathf.Sign(m_Rigidbody2D.velocity.x) * 5.0f, m_Rigidbody2D.velocity.y);
        }

        if (Mathf.Abs(m_Rigidbody2D.velocity.y) > 8.0f)
        {
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, Mathf.Sign(m_Rigidbody2D.velocity.y) * 8.0f);
        }
        
    }

    public void OnMove(InputValue value)
    {
        float moveFactor = value.Get<Vector2>().x;
        m_Velocity = m_GroundRight * moveFactor;
        Debug.DrawRay(transform.position, m_Velocity, Color.red);
    }

    public void OnLook(InputValue value)
    {
        m_AimingDir = value.Get<Vector2>();
    }

    public void OnTeleportDebug(InputValue value)
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(m_MousePosition);
        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }

    public void OnMousePosition(InputValue value)
    {
        m_MousePosition = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (m_OnGround || m_CurrentJumpCount < m_AllowedJumpCount)
        {
            Jump();
        }
    }

    void Jump()
    {
        m_CurrentJumpCount++;
        m_Rigidbody2D.AddForce(transform.up * m_JumpForce);
        m_OnGround = false;
    }

    public Vector2 GetMuzzlePosition()
    {
        return m_MuzzleDummy.transform.position;
    }

    public void OnFire(InputValue value)
    {
        m_IsRunning = value.isPressed;
        if (m_AimingDir != Vector2.zero)
        {
            List<Collider2D> colliders = new List<Collider2D>(5);
            ContactFilter2D contactFilter2D = default;
            int bouboulLayer = LayerMask.NameToLayer("Bouboul");
            contactFilter2D.NoFilter();
            int collidersFound = Physics2D.OverlapCircle(GetMuzzlePosition(), m_BouboulResearchRadius, contactFilter2D, colliders);
            BouboulAIComponent bouboul = null;
            if (collidersFound > 0)
            {
                for (int i = 0; i < colliders.Count; ++i)
                {
                    Collider2D collider = colliders[i];
                    if (collider.gameObject.layer == bouboulLayer)
                    {
                        bouboul = collider.gameObject.GetComponent<BouboulAIComponent>();
                        if (bouboul != null)
                            break;
                    }
                }

                if (bouboul != null)
                {
                    bouboul.m_IsCarried = true;
                    //Rigidbody2D projectile = Instantiate<Rigidbody2D>(m_ProjectileGameObject, GetMuzzlePosition(), Quaternion.identity);
                    bouboul.transform.position = GetMuzzlePosition();
                    bouboul.OnShoot();

                    bouboul.GetComponent<Rigidbody2D>().AddForce(m_AimingDir.normalized * m_ProjectileForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    public void OnInteract(InputValue value)
    {
        m_InteractStart = value.isPressed ? Time.time : -1f;
    }

    public float InteractDuration {
        get { return m_InteractStart >= 0f ? Time.time - m_InteractStart : 0f; }
    }

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        m_OnGround = true;
        m_CurrentJumpCount = 0;
        m_GroundRight = Vector3.right;//collision2D.gameObject.transform.right;
    }
     

    public void RegisterInteractive(InteractiveComponent inter)
    {
        if (m_InteractivesInRange.Contains(inter)) return;

        m_InteractivesInRange.Add(inter);
        m_InteractivesInRange.Sort(
            delegate (InteractiveComponent c1, InteractiveComponent c2)
            {
                float dist1 = Mathf.Abs(c1.transform.position.x - transform.position.x);
                float dist2 = Mathf.Abs(c2.transform.position.x - transform.position.x);
                if (Mathf.Abs(dist1 - dist2) <= Mathf.Epsilon) return 0;
                else if (dist1 > dist2) return 1;
                else return -1;
            }
        );

        UpdateArtifact();
    }

    public void ForgetInteractive(InteractiveComponent inter)
    {
        m_InteractivesInRange.Remove(inter);
    }

    public InteractiveComponent ClosestInteractive {
        get { return (m_InteractivesInRange.Count > 0 ? m_InteractivesInRange[0] : null); }
    }

    public void OnCollectedPiece(ArtifactPieceComponent piece)
    {
        PiecesOwned++;
        UpdateArtifact();
    }

    public void UsePieces()
    {
        PiecesOwned = 0;
    }

    private void UpdateArtifact()
    {
        if (ClosestInteractive != null && ClosestInteractive.GetComponent<ArtifactComponent>() != null)
        {
            ClosestInteractive.GetComponent<ArtifactComponent>().Locked = PiecesOwned < 3;
        }
    }
}
