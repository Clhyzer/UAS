using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);  // Offset kamera dari target
    private float smoothTime = 0.25f;  // Kecepatan transisi kamera
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private Transform target;  // Target kamera, misalnya player

    // Batasan untuk kamera (bisa diatur sesuai kebutuhan)
    public float leftLimit = -5f;   // Batas kiri
    public float rightLimit = 5f;   // Batas kanan
    public float topLimit = 5f;     // Batas atas
    public float bottomLimit = -5f; // Batas bawah

    void Update()
    {
        // Posisi target ditambah dengan offset
        Vector3 targetPosition = target.position + offset;

        // Pembatasan posisi kamera agar tidak melewati batas yang ditentukan
        float clampedX = Mathf.Clamp(targetPosition.x, leftLimit, rightLimit);
        float clampedY = Mathf.Clamp(targetPosition.y, bottomLimit, topLimit);

        // Tentukan posisi kamera dengan pembatasan
        Vector3 desiredPosition = new Vector3(clampedX, clampedY, targetPosition.z);

        // SmoothDamp untuk pergerakan kamera yang halus
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }
}
