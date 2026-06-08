using UnityEngine;
using UnityEngine.UI; // Thêm thư viện UI
using System.Collections;

public class Castle : MonoBehaviour
{
    public static Castle Instance { get; private set; }

    [Header("Chỉ số Lâu đài")]
    public float maxHealth = 1000f;
    public float currentHealth;

    [Header("Giao diện")]
    public Slider hpSlider;
    public Image damageFlash;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); // Đỏ mờ
    public float flashDuration = 0.1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentHealth = maxHealth;

        // Cập nhật thanh UI lúc mới bắt đầu
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Cập nhật thanh máu trên màn hình
        if (hpSlider != null) hpSlider.value = currentHealth;

        // Bật màn hình nháy đỏ
        if (damageFlash != null) StartCoroutine(FlashRoutine());

        Debug.Log($"<color=red>Lâu đài bị tấn công! Trừ {amount} máu. Còn lại: {currentHealth}</color>");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("<color=yellow>LÂU ĐÀI ĐÃ SẬP! GAME OVER!</color>");

            // KÍCH HOẠT BẢNG THUA
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    IEnumerator FlashRoutine()
    {
        if (damageFlash != null)
        {
            float elapsedTime = 0f;
            float targetAlpha = flashColor.a; // Độ đậm tối đa cậu chỉnh trên Inspector (ví dụ 0.5)

            // THÌ 1: SÁNG DẦN LÊN (FADE IN)
            while (elapsedTime < flashDuration)
            {
                elapsedTime += Time.deltaTime;
                // Hàm Lerp giúp tính toán độ mờ tăng dần theo thời gian rất mượt
                float currentAlpha = Mathf.Lerp(0f, targetAlpha, elapsedTime / flashDuration);

                damageFlash.color = new Color(flashColor.r, flashColor.g, flashColor.b, currentAlpha);
                yield return null; // Đợi khung hình tiếp theo
            }

            elapsedTime = 0f;

            // THÌ 2: MỜ DẦN ĐI (FADE OUT)
            while (elapsedTime < flashDuration)
            {
                elapsedTime += Time.deltaTime;
                // Hàm Lerp giảm dần độ mờ về 0
                float currentAlpha = Mathf.Lerp(targetAlpha, 0f, elapsedTime / flashDuration);

                damageFlash.color = new Color(flashColor.r, flashColor.g, flashColor.b, currentAlpha);
                yield return null;
            }

            // Đảm bảo ảnh tàng hình hoàn toàn sau khi nháy xong
            damageFlash.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
        }
    }

    // HÀM MỚI: Gọi để hồi máu cho lâu đài (Dùng cho hệ Thủy)
    public void Heal(float amount)
    {
        if (currentHealth >= maxHealth) return; // Máu đầy rồi thì thôi

        currentHealth += amount;

        // Không cho máu vượt quá máu tối đa
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        // Cập nhật thanh UI thanh máu
        if (hpSlider != null) hpSlider.value = currentHealth;

        Debug.Log($"<color=green>Lâu đài được hồi {amount} máu! Hiện tại: {currentHealth}</color>");
    }
}