using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ShopItem : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    [Header("Thông tin Tướng")]
    public int heroIndex;
    public int heroCost = 50;
    public int currentLevel = 1;
    public string heroName = "Hỏa Tiễn";

    [Header("Giá Nâng Cấp Thẻ")]
    public int upgradeToLv2Cost = 100;
    public int upgradeToLv3Cost = 200;

    [Header("Giao diện UI")]
    public TextMeshProUGUI costText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nameText;

    [Header("Giao diện Cấp độ (Màu sắc)")]
    public Image topBorder;
    public Outline cardOutline;

    [Tooltip("0: Cấp 1 (Đồng) | 1: Cấp 2 (Bạc) | 2: Cấp 3 (Vàng)")]
    public Color[] levelColors;

    private Coroutine juiceCoroutine;

    void Start()
    {
        if (nameText != null) nameText.text = heroName;
        if (costText != null) costText.text = heroCost.ToString();
        UpdateUIAndColor();
    }

    // Hàm cập nhật chữ, màu viền và màu chữ Level
    void UpdateUIAndColor()
    {
        // 1. THAY ĐỔI: Nếu đạt cấp 3 thì đổi chữ thành LV. MAX
        if (levelText != null)
        {
            if (currentLevel >= 3)
                levelText.text = "LV. MAX";
            else
                levelText.text = "Level " + currentLevel.ToString();
        }

        // 2. Đổi màu đồng bộ Viền, Đường kẻ và Chữ Level
        if (levelColors != null && levelColors.Length >= currentLevel && currentLevel > 0)
        {
            Color rankColor = levelColors[currentLevel - 1];

            if (topBorder != null) topBorder.color = rankColor;
            if (cardOutline != null) cardOutline.effectColor = rankColor;
            if (levelText != null) levelText.color = rankColor; // <-- THÊM: Chữ đổi màu theo cấp
        }
    }

    // THAY THẾ TOÀN BỘ HÀM NÀY:
    public void OnPointerClick(PointerEventData eventData)
    {
        // THAY ĐỔI CỐT LÕI: Kiểm tra số lần chạm liên tiếp (Double Tap)
        if (eventData.clickCount == 2)
        {
            // Logic nâng cấp giữ nguyên hoàn toàn của cậu
            if (currentLevel >= 3) return;

            int neededGold = (currentLevel == 1) ? upgradeToLv2Cost : upgradeToLv3Cost;

            if (GameManager.Instance != null && GameManager.Instance.HasEnoughGold(neededGold))
            {
                GameManager.Instance.SpendGold(neededGold);
                currentLevel++;

                // Cập nhật giá mua lính
                heroCost = Mathf.RoundToInt(heroCost * 2f);
                if (costText != null) costText.text = heroCost.ToString();

                // Chạy hiệu ứng rung lắc và đổi màu
                if (juiceCoroutine != null) StopCoroutine(juiceCoroutine);
                juiceCoroutine = StartCoroutine(UpgradeJuiceRoutine());

                Debug.Log($"<color=cyan>Đã nâng cấp thẻ {heroName} lên Level {currentLevel}! Giá đặt mới: {heroCost} Vàng</color>");
            }
            else
            {
                if (NotificationManager.Instance != null)
                    NotificationManager.Instance.ShowWarning("Không đủ Vàng nâng cấp!");
                // 📢 THÊM DÒNG NÀY: Tiếng báo lỗi hết tiền
                if (AudioManager.Instance != null && AudioManager.Instance.notEnoughGoldSound != null)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.notEnoughGoldSound);
            }
        }
    }

    IEnumerator UpgradeJuiceRoutine()
    {
        Color targetRankColor = levelColors[currentLevel - 1];

        // Lưu lại vị trí và tỷ lệ ban đầu của TOÀN BỘ CÁI THẺ
        Vector3 cardOriginalScale = transform.localScale;
        Quaternion cardOriginalRotation = transform.localRotation;

        float elapsedTime = 0f;
        float duration = 0.3f; // Tăng thời gian lên xíu cho mượt

        if (levelText != null && currentLevel >= 3) levelText.text = "LV. MAX";

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // 1. HIỆU ỨNG NẨY TOÀN BỘ THẺ (Scale)
            float scaleMultiplier = 1f + Mathf.Sin(progress * Mathf.PI) * 0.15f;
            transform.localScale = cardOriginalScale * scaleMultiplier;

            // 2. HIỆU ỨNG RUNG LẮC (Shake)
            float zRotation = Mathf.Sin(progress * Mathf.PI * 6f) * 5f; // Lắc trái phải 5 độ
            transform.localRotation = Quaternion.Euler(0, 0, zRotation);

            // 3. ĐỔI MÀU GIAO DIỆN
            if (levelText != null) levelText.color = Color.Lerp(Color.white, targetRankColor, progress);
            if (cardOutline != null) cardOutline.effectColor = Color.Lerp(Color.white, targetRankColor, progress);

            yield return null;
        }

        // Trả mọi thứ về vị trí cũ một cách gọn gàng
        UpdateUIAndColor();
        transform.localScale = cardOriginalScale;
        transform.localRotation = cardOriginalRotation;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (GameManager.Instance != null && !GameManager.Instance.HasEnoughGold(heroCost))
        {
            if (NotificationManager.Instance != null)
                NotificationManager.Instance.ShowWarning("Không đủ Vàng mua tướng!");

            // 📢 BỔ SUNG: Chèn thêm tiếng báo lỗi vào đây là chuẩn bài!
            if (AudioManager.Instance != null && AudioManager.Instance.notEnoughGoldSound != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.notEnoughGoldSound);

            return;
        }

        if (PlacementManager.Instance != null)
        {
            PlacementManager.Instance.StartDragging(heroIndex, heroCost, currentLevel);
        }
    }
}