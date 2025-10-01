using UnityEngine;
using UnityEngine.InputSystem; // 新版 Input System

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Move / Jump")]
    public float speed = 5f;
    public float jumpForce = 8f;

    [Header("Multi Jump")]
    public int maxJumps = 2;           // ← 兩段跳 = 2
    public bool resetYVelocityOnJump = true; // 第二段跳前把垂直速度歸零，手感更快

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    private int jumpsRemaining;

    [Header("Acceleration")]
    public float acceleration = 10f;   // 加速率
    public float deceleration = 15f;   // 減速率
    public float maxRunSpeed = 8f;     // 最大移動速度
    private float currentSpeed = 0f;   // 當前水平速度

    [Header("Death / Respawn")]
    public float deathY = -10f;      // 低於這高度視為掉出關卡
    public Vector3 respawnPoint;     // 重生點（預設為起始位置）

    void Start()
    {
        respawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
        ResetJumps();
    }

    void Update()
    {
        // 掉出關卡
        if (transform.position.y < deathY)
        {
            HandleDeath();
        }
    }

    void FixedUpdate()
    {
        float targetSpeed = moveInput * maxRunSpeed;
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            // 按方向鍵：逐步加速到 maxRunSpeed
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // 鬆開：逐步減速到 0
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    // === Input System: Send Messages ===
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<float>();
    }

    public void OnJump(InputValue value)
    {
        // Send Messages 這裡在按下時 value.isPressed = true，放開時為 false
        if (value.isPressed)
        {
            TryJump();
        }
    }

    // === Jump Core ===
    private void TryJump()
    {
        if (jumpsRemaining > 0)
        {
            if (resetYVelocityOnJump)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpsRemaining--;
        }
    }

    private void ResetJumps()
    {
        jumpsRemaining = maxJumps;
    }

    // === Ground Check（用 Ground Tag）===
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Rooftop"))
        {
            isGrounded = true;
            ResetJumps(); // 一碰到地面就重置可跳次數
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Rooftop"))
        {
            isGrounded = false;
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Rooftop"))
        {
            isGrounded = true;
        }
    }

    private void HandleDeath()
    {
        var hud = UIHUD.I;
        if (hud != null)
        {
            hud.LoseLife(1);
            if (hud.Lives > 0)
            {
                // 重生
                var rb2d = GetComponent<Rigidbody2D>();
                if (rb2d) rb2d.linearVelocity = Vector2.zero;
                transform.position = respawnPoint;
            }
            else
            {
                // 遊戲結束：停用控制（或 Time.timeScale = 0）
                var rb2d = GetComponent<Rigidbody2D>();
                if (rb2d) rb2d.linearVelocity = Vector2.zero;
                enabled = false;
            }
        }
        else
        {
            // 沒有 HUD：直接重生備援
            var rb2d = GetComponent<Rigidbody2D>();
            if (rb2d) rb2d.linearVelocity = Vector2.zero;
            transform.position = respawnPoint;
        }
    }
}