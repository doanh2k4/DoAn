using UnityEngine;
using UnityEngine.InputSystem; // BẮT BUỘC PHẢI CÓ DÒNG NÀY

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private Vector3 dragOrigin;

    [Header("Giới hạn kéo ngang (Map Bounds)")]
    public float minX = -10f;
    public float maxX = 10f;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        PanCameraHorizontal();
    }

    private void PanCameraHorizontal()
    {
        // Kiểm tra xem có chuột không để tránh lỗi
        if (Mouse.current == null) return;

        // Bấm CHUỘT PHẢI (vừa chạm xuống)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            dragOrigin = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        // Đang giữ CHUỘT PHẢI và kéo
        if (Mouse.current.rightButton.isPressed)
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // Tính toán vị trí X mới
            float newX = cam.transform.position.x + difference.x;

            // Chặn không cho kéo vượt quá giới hạn 2 bên
            float clampedX = Mathf.Clamp(newX, minX, maxX);

            // Cập nhật vị trí Camera: Trục X thay đổi, trục Y và Z giữ nguyên như cũ
            cam.transform.position = new Vector3(clampedX, cam.transform.position.y, cam.transform.position.z);
        }
    }
}