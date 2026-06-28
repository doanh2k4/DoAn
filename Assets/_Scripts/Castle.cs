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

    private float waterHealTimer = 0f; // Đồng hồ đếm thời gian hồi máu hệ Thủy

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

    private void Update()
    {
        // ================= LOGIC HỒI MÁU HỆ THỦY MỚI =================
        waterHealTimer += Time.deltaTime;
        if (waterHealTimer >= 1f) // Cứ mỗi 1 giây quét 1 lần
        {
            waterHealTimer = 0f;
            ApplyWaterHeroHeal();
        }
    }

    private void ApplyWaterHeroHeal()
    {
        HeroCombat[] allHeroes = FindObjectsOfType<HeroCombat>();
        float highestMultiplier = 0f;

        // Tìm xem trên sân có bao nhiêu tướng Thủy, lấy đứa mạnh nhất
        foreach (HeroCombat hero in allHeroes)
        {
            if (hero.element == ElementType.Thuy)
            {
                if (hero.damageMultiplier > highestMultiplier)
                {
                    highestMultiplier = hero.damageMultiplier;
                }
            }
        }

        // Nếu có ít nhất 1 tướng Thủy trên sân
        if (highestMultiplier > 0f)
        {
            // Hồi 1% máu gốc * hệ số sức mạnh của tướng mạnh nhất
            // Nghĩa là 5 ông Lv1 cũng chỉ bơm bằng 1 ông Lv1. Nhưng 1 ông Lv3 sẽ bơm mạnh x2.2 lần!
            float healAmount = maxHealth * 0.01f * highestMultiplier;
            Heal(healAmount);
        }
    }

    public void TakeDamage(float amount)
    {
        // 📢 THÊM DÒNG NÀY: Phát tiếng gạch vỡ khi Lâu đài bị đấm
        if (AudioManager.Instance != null && AudioManager.Instance.castleHitSound != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.castleHitSound);
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

    public void Heal(float amount)
    {
        if (currentHealth >= maxHealth) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (hpSlider != null) hpSlider.value = currentHealth;

        if (healParticlePrefab != null)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y -= 1.0f;
            GameObject particleInstance = Instantiate(healParticlePrefab, spawnPos, Quaternion.identity);
            Destroy(particleInstance, 2.0f);
        }

        // Tớ khuyên cậu NÊN TẮT dòng Debug này đi (thêm // ở đầu), vì bây giờ nó hồi mỗi giây 1 lần, 
        // để Debug.Log nó sẽ làm rác bảng Console của cậu rất nhanh!
        // Debug.Log($"<color=green>Lâu đài được hồi {amount} máu! Hiện tại: {currentHealth}</color>");
    }
}