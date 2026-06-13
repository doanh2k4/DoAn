using UnityEngine;
using UnityEngine.UI;
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

    [Header("Hiệu ứng Hồi máu (MỚI)")]
    [Tooltip("Kéo Prefab Particle System hồi máu (màu xanh nhẹ, bay lên) vào đây")]
    public GameObject healParticlePrefab;

    // XÓA BỎ các biến liên quan đến spriteRenderer và coroutine color cũ

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (hpSlider != null)
            hpSlider.value = currentHealth;

        if (damageFlash != null)
            StartCoroutine(FlashRoutine());

        Debug.Log($"<color=red>Lâu đài bị tấn công! Trừ {amount} máu. Còn lại: {currentHealth}</color>");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("<color=yellow>LÂU ĐÀI ĐÃ SẬP! GAME OVER!</color>");
            if (GameManager.Instance != null) GameManager.Instance.GameOver();
        }
    }

    IEnumerator FlashRoutine()
    {
        if (damageFlash != null)
        {
            float elapsedTime = 0f;
            float targetAlpha = flashColor.a;

            while (elapsedTime < flashDuration)
            {
                elapsedTime += Time.deltaTime;
                float currentAlpha = Mathf.Lerp(0f, targetAlpha, elapsedTime / flashDuration);
                damageFlash.color = new Color(flashColor.r, flashColor.g, flashColor.b, currentAlpha);
                yield return null;
            }

            elapsedTime = 0f;

            while (elapsedTime < flashDuration)
            {
                elapsedTime += Time.deltaTime;
                float currentAlpha = Mathf.Lerp(targetAlpha, 0f, elapsedTime / flashDuration);
                damageFlash.color = new Color(flashColor.r, flashColor.g, flashColor.b, currentAlpha);
                yield return null;
            }

            damageFlash.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
        }
    }

    // ================= HÀM HỒI MÁU VÀ HIỆU ỨNG HẠT BAY LÊN =================
    public void Heal(float amount)
    {
        if (currentHealth >= maxHealth) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (hpSlider != null) hpSlider.value = currentHealth;

        // === KÍCH HOẠT HIỆU ỨNG HẠT (PARTICLE) ===
        if (healParticlePrefab != null)
        {
            // 1. Xác định vị trí chân lâu đài.
            // transform.position mặc định thường là TÂM của object.
            Vector3 spawnPos = transform.position;

            // 2. Điều chỉnh Y xuống một chút để nó xuất phát từ chân.
            // Cậu có thể tùy chỉnh số 1.0f này tùy theo kích thước lâu đài của cậu.
            spawnPos.y -= 1.0f;

            // 3. Tạo ra hiệu ứng
            GameObject particleInstance = Instantiate(healParticlePrefab, spawnPos, Quaternion.identity);

            // 4. Tự hủy hiệu ứng sau khi chạy xong (ví dụ sau 2 giây) để tránh rác RAM
            Destroy(particleInstance, 2.0f);
        }

        Debug.Log($"<color=green>Lâu đài được hồi {amount} máu! Hiện tại: {currentHealth}</color>");
    }
}