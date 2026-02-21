using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public MicInputManager micManager;
    public Slider micProgressBar;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject winUI;           // ลากหน้าจอ Win_UI มาใส่ที่นี่

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Mic Jump Settings")]
    public float sensitivity = 100f;
    public float loudnessThreshold = 0.5f;
    public float jumpForce = 10f;
    public float checkRadius = 0.2f;

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isDead = false;
    private bool hasWon = false;       // เช็คว่าชนะหรือยัง

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
    }

    void OnEnable() => inputActions.Player.Enable();
    void OnDisable() => inputActions.Player.Disable();

    void Update()
    {
        if (isDead || hasWon) return; // ถ้าชนะแล้วให้หยุดควบคุม

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

        if (micProgressBar != null)
        {
            micProgressBar.value = Mathf.Clamp(boostedLoudness, 0, 1);
        }

        if (boostedLoudness >= loudnessThreshold && isGrounded)
        {
            Jump(boostedLoudness);
        }
    }

    void Jump(float forceMultiplier)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * (jumpForce * forceMultiplier), ForceMode2D.Impulse);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.jumpSound);
        }
    }

    // ฟังก์ชันเมื่อชนะเกม
    public void WinGame()
    {
        if (hasWon || isDead) return;
        hasWon = true;

        // 1. เล่นเสียง Victory จาก AudioManager
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.collectBigSound);
        }

        // 2. แสดงหน้าจอ Win UI
        if (winUI != null)
        {
            winUI.SetActive(true);
        }

        // 3. หยุดเวลาในเกม (Option)
        Time.timeScale = 0f;
    }

    public void Die()
    {
        if (isDead || hasWon) return;
        isDead = true;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.deathSound);
        }

        rb.linearVelocity = Vector2.zero;
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;
        rb.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);

        Invoke("RestartLevel", 2.8f);
    }

    void RestartLevel()
    {
        Time.timeScale = 1f; // คืนค่าเวลาก่อนโหลดฉากใหม่
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // เช็คว่าเก็บอั่งเปาใหญ่ (ตั้ง Tag ของอั่งเปาใหญ่ว่า "Finish" หรือ "BigItem")
        if (other.CompareTag("Finish"))
        {
            WinGame();
            Destroy(other.gameObject); // เก็บแล้วหายไป
        }

        if (other.CompareTag("Spike")) Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike")) Die();

        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyPatrol enemy = collision.gameObject.GetComponent<EnemyPatrol>();
            if (enemy != null)
            {
                if (enemy.canBeStomped && transform.position.y > collision.transform.position.y + 0.5f)
                {
                    enemy.StompDeath();
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
                }
                else Die();
            }
        }
    }
}