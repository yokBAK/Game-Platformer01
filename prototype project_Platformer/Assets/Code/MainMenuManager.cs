using UnityEngine;
using UnityEngine.SceneManagement; // จำเป็นสำหรับการเปลี่ยน Scene

public class MainMenuManager : MonoBehaviour
{
    // ฟังก์ชันสำหรับปุ่ม Play
    public void PlayGame()
    {
        // โหลดฉากที่ชื่อ "Game Play" (ตรวจสอบชื่อ Scene ใน Build Settings ให้ตรงกัน)
        SceneManager.LoadScene("Game Play");
    }

    // ฟังก์ชันสำหรับปุ่ม Quit
    public void QuitGame()
    {
        Debug.Log("Quit Game!"); // แสดงใน Console เพื่อให้รู้ว่ากดติด (ใน Editor เกมจะไม่ปิดจริง)
        Application.Quit();     // ปิดแอปพลิเคชัน (ทำงานเมื่อ Build เป็นไฟล์ .exe หรือ .apk แล้ว)
    }
}