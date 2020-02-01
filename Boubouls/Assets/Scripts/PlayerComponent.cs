using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerComponent : MonoBehaviour
{
    public float m_Speed = 50.0f;
    public float m_RunSpeed = 110.0f;
    public float m_JumpForce = 250;
    public Vector2 m_Velocity = new Vector2();
    public bool m_OnGround = true;
    public Vector2 m_GroundRight = new Vector2(1.0f, 0.0f);
    public uint m_AllowedJumpCount = 2;
    public uint m_CurrentJumpCount = 0;

    private Rigidbody2D m_Rigidbody2D = null;
    private bool m_IsRunning = false;

    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
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
    }

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        m_OnGround = true;
        m_CurrentJumpCount = 0;
        m_GroundRight = collision2D.gameObject.transform.right;
    }
}
