using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ตรวจสอบว่าสิ่งที่มาชนมี Tag ว่า "Player" หรือไม่
        if (collision.gameObject.CompareTag("Player"))
        {
            // เรียกฟังก์ชัน Die() ที่เราเขียนไว้ใน PlayerController
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                player.Die();
            }
        }
    }

    // กรณีใช้กับดักที่เป็น Trigger (เช่น หลุม หรือพื้นที่ล่องหน)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().Die();
        }
    }
}