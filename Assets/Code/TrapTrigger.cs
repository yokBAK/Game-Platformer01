using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public MovingTrap targetTrap; // ลากกับดักที่ต้องการให้ทำงานมาใส่ที่นี่

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ถ้าผู้เล่นเดินเข้ามาในโซน
        if (collision.CompareTag("Player"))
        {
            if (targetTrap != null)
            {
                targetTrap.ActivateTrap(); // สั่งให้กับดักเริ่มเดิน

                // ถ้าอยากให้เหยียบแล้วทำงานครั้งเดียว ให้ลบจุดตรวจจับทิ้งไปเลย
                // Destroy(gameObject); 
            }
        }
    }
}