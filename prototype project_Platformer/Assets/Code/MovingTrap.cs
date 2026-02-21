using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] waypoints;
    public float speed = 3f;
    public bool loop = true;

    [Header("Activation")]
    public bool isActivated = false; // เริ่มต้นเป็น False (ไม่ทำงาน)

    private int currentWaypointIndex = 0;

    void Update()
    {
        // ถ้ายังไม่ถูกกระตุ้น (Activated) หรือไม่มีจุดหมาย ก็ไม่ต้องเดิน
        if (!isActivated || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                if (loop) currentWaypointIndex = 0;
                else isActivated = false;
            }
        }
    }

    // ฟังก์ชันสำหรับเรียกจากภายนอกเพื่อสั่งให้เริ่มทำงาน
    public void ActivateTrap()
    {
        isActivated = true;
    }
}