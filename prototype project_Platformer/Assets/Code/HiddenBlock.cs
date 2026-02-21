using UnityEngine;

public class HiddenBlock : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isRevealed = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // เริ่มต้นเกมให้บล็อกล่องหน (ปรับค่า Alpha เป็น 0)
        Color c = spriteRenderer.color;
        c.a = 0f;
        spriteRenderer.color = c;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ถ้าถูกเปิดเผยไปแล้ว ไม่ต้องทำอะไรซ้ำ
        if (isRevealed) return;

        // เช็คว่าคนที่ชนคือ Player หรือไม่
        if (collision.gameObject.CompareTag("Player"))
        {
            // เช็คทิศทาง: ถ้า Player ชนจากด้านล่าง (ทิศทางขึ้นข้างบน)
            // เราจะเช็คว่าตำแหน่ง Y ของ Player ต่ำกว่าตัวบล็อก
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f) // Normal ชี้ขึ้น = โดนชนจากด้านล่าง
                {
                    RevealBlock();
                    break;
                }
            }
        }
    }

    void RevealBlock()
    {
        isRevealed = true;

        // ทำให้บล็อกปรากฏออกมา
        Color c = spriteRenderer.color;
        c.a = 1f;
        spriteRenderer.color = c;

        // ใส่ท่าเด้งขึ้นเล็กน้อยเพื่อให้ดูมีน้ำหนัก (Optional)
        StartCoroutine(BounceEffect());
    }

    private System.Collections.IEnumerator BounceEffect()
    {
        Vector3 startPos = transform.position;
        Vector3 upPos = startPos + new Vector3(0, 0.2f, 0);

        float elapsed = 0;
        float duration = 0.1f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, upPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos; // กลับมาที่เดิม
    }
}