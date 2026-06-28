using UnityEngine;

public class HeroSlot : MonoBehaviour
{
    public bool isOccupied = false;
    public GameObject currentHero;

    [Header("Giao diện")]
    public GameObject highlightCircle; // Kéo thả vòng tròn phát sáng vào đây

    // THAY ĐỔI: Hàm nhận thêm tham số level
    public void PlaceHero(GameObject heroPrefab, int level)
    {
        currentHero = Instantiate(heroPrefab, transform.position, Quaternion.identity);
        currentHero.transform.SetParent(transform);
        isOccupied = true;
        HideHighlight();

        // Xử lý chống nhảy vật lý cũ của cậu giữ nguyên
        Rigidbody2D rb = currentHero.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
            Destroy(rb);
        }
        // 🚧 THÊM VÀO ĐÂY: Tắt Collider2D để tướng không làm bia đỡ đạn
        Collider2D col = currentHero.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // --- ĐOẠN CODE MỚI: Tìm script chiến đấu của Tướng để kích hoạt Level ---
        HeroCombat combatScript = currentHero.GetComponent<HeroCombat>();
        if (combatScript != null)
        {
            combatScript.ApplyLevelStats(level); // Bảo con tướng tự tăng chỉ số
        }
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