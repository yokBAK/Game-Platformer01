using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    void LateUpdate()
    {
        if (target != null)
        {
            // สร้างตำแหน่งเป้าหมายใหม่
            // ใช้ target.position.x สำหรับแกนซ้ายขวา
            // แต่ใช้ transform.position.y (ตำแหน่งปัจจุบันของกล้อง) เพื่อล็อคไม่ให้ขยับขึ้นลง
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, transform.position.y, target.position.z + offset.z);

            // ทำให้การเคลื่อนที่นุ่มนวลเฉพาะในแกนที่อนุญาต
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            transform.position = smoothedPosition;
        }
    }
}