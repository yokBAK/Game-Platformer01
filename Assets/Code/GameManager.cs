using UnityEngine;
using TMPro; // สำคัญ: ต้องใช้ TextMeshPro เพื่อความคมชัด

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // ทำให้สคริปต์อื่นเรียกใช้ง่ายๆ

    public TextMeshProUGUI scoreText; // ลาก Text จาก UI มาใส่ในนี้
    private int currentScore = 0;

    void Awake()
    {
        // สร้างระบบ Singleton เพื่อให้เรียกใช้ผ่าน GameManager.instance ได้เลย
        if (instance == null) instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString("N0"); // N0 คือใส่คอมม่า เช่น 50,000
        }
    }
}