using UnityEngine;

public class RedEnvelope : MonoBehaviour
{
    public enum EnvelopeType { Small, Big } // สร้างตัวเลือกใน Inspector

    [Header("Settings")]
    public EnvelopeType type = EnvelopeType.Small;
    public int smallValue = 100;
    public int bigValue = 50000;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // เช็คว่าผู้เล่นมาชนไหม
        if (collision.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        // 1. เล่นเสียงตามประเภทของอั่งเปา
        if (AudioManager.instance != null)
        {
            if (type == EnvelopeType.Small)
                AudioManager.instance.PlaySFX(AudioManager.instance.collectSmallSound);
            else
                AudioManager.instance.PlaySFX(AudioManager.instance.collectBigSound);
        }

        // 2. ส่งคะแนนไปที่ GameManager (แก้ไขจุดนี้)
        if (GameManager.instance != null)
        {
            int scoreToAdd = (type == EnvelopeType.Small) ? smallValue : bigValue;
            GameManager.instance.AddScore(scoreToAdd);
        }

        // 3. ถ้าเป็นอั่งเปาใหญ่ ให้สั่งชนะเกม
        if (type == EnvelopeType.Big)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.WinGame();
            }
        }

        Destroy(gameObject);
    }

}