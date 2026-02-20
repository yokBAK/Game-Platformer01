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
        int scoreToAdd = (type == EnvelopeType.Small) ? smallValue : bigValue;

        // เรียกใช้ GameManager ให้บวกคะแนน
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(scoreToAdd);
        }

        Destroy(gameObject);
    }

}