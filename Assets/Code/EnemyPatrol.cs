using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform wallCheck;      // จุดเช็คกำแพง (วางไว้ข้างหน้าศัตรู)
    public float checkRadius = 0.2f;
    public LayerMask wallLayer;      // เลือก Layer "Ground" หรือกำแพง

    private bool movingLeft = true;  // เริ่มต้นเดินซ้ายตามที่คุณต้องการ
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. สั่งให้เดินตามทิศทางปัจจุบัน
        if (movingLeft)
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(1, 1, 1); // หันหน้าไปทางซ้าย (ปรับตาม Sprite ของคุณ)
        }
        else
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(-1, 1, 1); // หันหลังกลับไปทางขวา
        }

        // 2. เช็คว่าชนกำแพงหรือยัง
        bool hitWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, wallLayer);

        if (hitWall)
        {
            // สลับทิศทาง
            movingLeft = !movingLeft;
            Debug.Log("Enemy hit wall, flipping direction!");
        }
    }

    // วาดวงกลมเช็คกำแพงในหน้า Scene
    private void OnDrawGizmosSelected()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }
    }

    // เสริมการเหยีบยบ
    // --- เพิ่มตัวแปรเหล่านี้ไว้ด้านบนของ Class ---
    [Header("Stomp Settings")]
    public bool canBeStomped = true; // ติ๊กถูกใน Inspector ได้เลย
    private bool isDead = false;

    // --- เพิ่มฟังก์ชันนี้ลงไปใน Class ---
    public void StompDeath()
    {
        if (isDead) return;
        isDead = true;

        // ปิดการชนและหยุดการเคลื่อนที่
        GetComponent<Collider2D>().enabled = false;

        // เริ่มทำท่าตาย (เด้งแล้วจม)
        StartCoroutine(StompAnimation());
    }

    private System.Collections.IEnumerator StompAnimation()
    {
        float duration = 1.0f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        // 1. เด้งขึ้นเล็กน้อย
        float jumpHeight = 0.5f;
        float jumpDuration = 0.2f;
        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float yOffset = Mathf.Sin((elapsed / jumpDuration) * Mathf.PI) * jumpHeight;
            transform.position = startPos + new Vector3(0, yOffset, 0);
            yield return null;
        }

        // 2. จมดินและจางหาย
        Vector3 fallStartPos = transform.position;
        Vector3 fallEndPos = fallStartPos + new Vector3(0, -5f, 0);
        elapsed = 0f;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(fallStartPos, fallEndPos, t);

            if (sprite != null)
            {
                Color c = sprite.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                sprite.color = c;
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}