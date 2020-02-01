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

    private void Awake()
    {
        m_InteractivesInRange = new List<InteractiveComponent>();
    }

    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateSpriteRenderer();
    }

    void UpdateSpriteRenderer()
    {
        if (m_SpriteRenderer == null)
            return;

        float horizontalVelocity = Mathf.Abs(m_Rigidbody2D.velocity.x);
        m_Animator.SetFloat("Speed", horizontalVelocity);

        if (Mathf.Abs(m_Rigidbody2D.velocity.x) > Mathf.Epsilon)
        {
            if (Vector3.Dot(m_Rigidbody2D.velocity, Vector3.right) >= 0.0f)
            {
                m_SpriteRenderer.flipX = false;
            }
            else
            {
                m_SpriteRenderer.flipX = true;
            }
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

    public void OnFire(InputValue value)
    {
        m_IsRunning = value.isPressed;
        if (m_AimingDir != Vector2.zero)
        {
            Debug.Log("Fire!");
            Rigidbody2D projectile = Instantiate<Rigidbody2D>(m_ProjectileGameObject, m_MuzzleDummy.transform.position, Quaternion.identity);
            projectile.AddForce(m_AimingDir.normalized * m_ProjectileForce, ForceMode2D.Impulse);
        }

    }

    public void OnInteract(InputValue value)
    {
        Debug.Log("Interact");
    }

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        m_OnGround = true;
        m_CurrentJumpCount = 0;
        m_GroundRight = collision2D.gameObject.transform.right;
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
