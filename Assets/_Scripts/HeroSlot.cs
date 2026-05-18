using UnityEngine;

public class HeroSlot : MonoBehaviour
{
    public bool isOccupied = false;
    public GameObject currentHero;

    [Header("Giao diện")]
    public GameObject highlightCircle; // Kéo thả vòng tròn phát sáng vào đây

    public void PlaceHero(GameObject heroPrefab)
    {
        // 1. Đẻ ra con Tướng tại vị trí của Slot (Logic cũ)
        currentHero = Instantiate(heroPrefab, transform.position, Quaternion.identity);
        currentHero.transform.SetParent(transform);
        isOccupied = true;
        HideHighlight();

        // --- TUYỆT CHIÊU XỬ LÝ DỨT ĐIỂM BỆNH NHẢY NHẢY ---
        // 2. Lấy Rigidbody2D của con tướng vừa đẻ ra
        Rigidbody2D rb = currentHero.GetComponent<Rigidbody2D>();

        // 3. Nếu có Rigidbody, hãy XÓA BỎ NÓ HOÀN TOÀN!
        if (rb != null)
        {
            // Tắt mô phỏng vật lý ngay lập tức trong khung hình này
            rb.simulated = false;

            // Xóa component khỏi object (thực hiện vào cuối khung hình)
            Destroy(rb);

            // Debug cho chắc ăn
            Debug.Log("<color=green>Đã xóa Rigidbody2D của Hero để chống nhảy!</color>");
        }
        // --------------------------------------------------
    }

    // Hàm gọi để bật sáng
    public void ShowHighlight()
    {
        // Chỉ bật sáng nếu ô này chưa có ai đứng
        if (!isOccupied && highlightCircle != null)
        {
            highlightCircle.SetActive(true);
        }
    }

    // Hàm gọi để tắt sáng
    public void HideHighlight()
    {
        if (highlightCircle != null)
        {
            highlightCircle.SetActive(false);
        }
    }
}