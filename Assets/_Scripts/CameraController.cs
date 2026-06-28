using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems; // BẮT BUỘC PHẢI THÊM THƯ VIỆN NÀY

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private Vector3 dragOrigin;

    // Thêm một cái "khóa" để biết khi nào được kéo Camera
    private bool isPanning = false;

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
        // Kiểm tra xem có Camera và có Ngón tay/Chuột trên màn hình không
        if (cam == null || Pointer.current == null) return;

        // BƯỚC 1: Vừa chạm ngón tay xuống
        if (Pointer.current.press.wasPressedThisFrame)
        {
            // KIỂM TRA: Nếu ngón tay chạm trúng Giao diện UI (Thẻ lính, Nút bấm...)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                isPanning = false; // Khóa Camera lại, không cho kéo!
                return;
            }

            // Nếu chạm ra vùng đất trống -> Mở khóa, cho phép kéo Camera
            isPanning = true;
            dragOrigin = cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        }

        // BƯỚC 2: Đang giữ ngón tay và vuốt
        // Chỉ chạy code dời Camera nếu cái "khóa" isPanning đang mở
        if (Pointer.current.press.isPressed && isPanning == true)
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());

            // Tính toán vị trí X mới
            float newX = cam.transform.position.x + difference.x;

            // Chặn không cho kéo vượt quá giới hạn 2 bên
            float clampedX = Mathf.Clamp(newX, minX, maxX);

            // Cập nhật vị trí Camera
            cam.transform.position = new Vector3(clampedX, cam.transform.position.y, cam.transform.position.z);
        }

        // BƯỚC 3: Nhấc ngón tay lên (Thả ra) -> Reset cái khóa về trạng thái đóng
        if (Pointer.current.press.wasReleasedThisFrame)
        {
            isPanning = false;
        }
    }
}