using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public MicInputManager micManager;
    public Slider micProgressBar;
    public Transform groundCheck;      // ลาก Object GroundCheck มาใส่
    public LayerMask groundLayer;      // เลือก Layer "Ground"

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Mic Jump Settings")]
    public float sensitivity = 100f;
    public float loudnessThreshold = 0.5f;
    public float jumpForce = 10f;
    public float checkRadius = 0.2f;   // รัศมีในการเช็คพื้น

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool isGrounded;           // ตัวแปรเช็คว่าอยู่ที่พื้นไหม
    private bool isDead = false;       // ตัวแปรเช็คว่าตายหรือยัง

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
    }

    void OnEnable() => inputActions.Player.Enable();
    void OnDisable() => inputActions.Player.Disable();

    void Update()
    {
        if (isDead) return; // ถ้าตายแล้ว ไม่ต้องทำอะไรต่อ (หยุดการเคลื่อนที่)

        CheckGroundStatus();
        HandleMovement();
        HandleMicJump();
    }

    void CheckGroundStatus()
    {
        // สร้างวงกลมเล็กๆ ที่เท้าเพื่อเช็คว่าชนกับ Layer "Ground" หรือไม่
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

        if (micProgressBar != null)
        {
            micProgressBar.value = boostedLoudness;
        }

        // เงื่อนไข: เสียงต้องดังถึงจุดที่กำหนด AND ต้องอยู่ที่พื้น AND ต้องยังไม่ตาย
        if (boostedLoudness > loudnessThreshold && isGrounded && !isDead)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // 1. ปิดระบบฟิสิกส์ปกติและการควบคุม
        rb.linearVelocity = Vector2.zero;

        // 2. ปิด Collider เพื่อให้ร่วงทะลุพื้น (ตกแมพ)
        GetComponent<Collider2D>().enabled = false;

        // 3. ดีดตัวละครขึ้นฟ้าเล็กน้อยก่อนร่วง (Death Jump)
        rb.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);

        Debug.Log("Player Jump-Died!");

        // 4. รอสักครู่ค่อยรีสตาร์ท (เพิ่มเวลาให้นานขึ้นเพื่อให้เห็นท่าตาย)
        Invoke("RestartLevel", 2.5f);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // วาดวงกลมเช็คพื้นในหน้า Scene (เพื่อให้เราเห็นว่าจุดเช็คอยู่ตรงไหน)
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}