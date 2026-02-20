using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public MicInputManager micManager;
    public Slider micProgressBar;
    public Transform groundCheck;      // จุดเช็คพื้นที่เท้า
    public LayerMask groundLayer;      // Layer ที่เป็นพื้น

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Mic Jump Settings")]
    public float sensitivity = 100f;       // ตัวคูณขยายเสียง
    public float loudnessThreshold = 0.5f; // ค่าที่เริ่มกระโดด (ครึ่งหลอด)
    public float jumpForce = 10f;          // แรงกระโดดพื้นฐาน
    public float checkRadius = 0.2f;       // รัศมีวงกลมเช็คพื้น

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isDead = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
    }

    void OnEnable() => inputActions.Player.Enable();
    void OnDisable() => inputActions.Player.Disable();

    void Update()
    {
        if (isDead) return;

        CheckGroundStatus();
        HandleMovement();
        HandleMicJump();
    }

    void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    void HandleMovement()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    void HandleMicJump()
    {
        float rawLoudness = micManager.GetLoudnessFromMic();
        float boostedLoudness = rawLoudness * sensitivity;

        // แสดงผล UI (จำกัดไว้แค่ 0-1 เพื่อไม่ให้หลอดบั๊ก)
        if (micProgressBar != null)
        {
            micProgressBar.value = Mathf.Clamp(boostedLoudness, 0, 1);
        }

        // เงื่อนไขการกระโดด: ถึงเกณฑ์ 0.5 + อยู่ที่พื้น + ยังไม่ตาย
        // ปลดล็อคความสูง: ใช้ boostedLoudness จริงคูณแรงกระโดด
        if (boostedLoudness >= loudnessThreshold && isGrounded && !isDead)
        {
            Jump(boostedLoudness);
        }
    }

    void Jump(float forceMultiplier)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * (jumpForce * forceMultiplier), ForceMode2D.Impulse);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // ท่าตายแบบกวนๆ: เด้งขึ้นแล้วร่วงทะลุพื้น
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        rb.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);

        Invoke("RestartLevel", 1.2f);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyPatrol enemy = collision.gameObject.GetComponent<EnemyPatrol>();

            // เช็คว่าเหยียบจากข้างบน และศัตรูตัวนี้ยอมให้เหยียบไหม
            if (enemy != null && enemy.canBeStomped)
            {
                // เช็คตำแหน่ง Y ว่าเท้าเราอยู่สูงกว่าหัวศัตรูไหม
                if (transform.position.y > collision.transform.position.y + 0.5f)
                {
                    enemy.StompDeath(); // เรียกท่าตาย

                    // ให้ตัวผู้เล่นเด้งขึ้น
                    Rigidbody2D rb = GetComponent<Rigidbody2D>();
                    if (rb != null) rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
                }
            }
        }
    }
}