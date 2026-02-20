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
}