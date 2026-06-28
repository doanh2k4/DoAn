using UnityEngine;
using TMPro; // BẮT BUỘC THÊM THƯ VIỆN NÀY ĐỂ SỬA CHỮ

public class SettingsMenu : MonoBehaviour
{
    [Header("Bảng Menu Settings")]
    public GameObject settingsPanel;

    [Header("Chữ trên Nút Bấm")]
    public TextMeshProUGUI bgmText; // Chữ của nút Nhạc nền
    public TextMeshProUGUI sfxText; // Chữ của nút Hiệu ứng

    private void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Cập nhật chữ chuẩn ngay từ lúc mới mở game
        UpdateBGMText();
        UpdateSFXText();
    }

    public void OpenSettings()
    {
        // Khi mở bảng lên, check lại xem nhạc đang bật hay tắt để hiện chữ cho đúng
        UpdateBGMText();
        UpdateSFXText();

        if (settingsPanel != null) settingsPanel.SetActive(true);
        Time.timeScale = 0f;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSound();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSound();
    }

    public void ToggleNhacNen()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.bgmSource != null)
        {
            AudioManager.Instance.bgmSource.mute = !AudioManager.Instance.bgmSource.mute;

            UpdateBGMText(); // 🔄 GỌI HÀM ĐỔI CHỮ NGAY LẬP TỨC
            AudioManager.Instance.PlayButtonSound();
        }
    }

    public void ToggleHieuUng()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.sfxSource != null)
        {
            AudioManager.Instance.sfxSource.mute = !AudioManager.Instance.sfxSource.mute;

            UpdateSFXText(); // 🔄 GỌI HÀM ĐỔI CHỮ NGAY LẬP TỨC
            AudioManager.Instance.PlayButtonSound();
        }
    }

    public void QuitToMainMenu()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSound();
        if (GameManager.Instance != null) GameManager.Instance.GoToMainMenu();
    }

    // ========================================================
    // CÁC HÀM PHỤ TRỢ ĐỂ ĐỔI CHỮ (LOGIC BẬT/TẮT)
    // ========================================================

    private void UpdateBGMText()
    {
        // Kiểm tra an toàn để không bị văng lỗi vàng
        if (bgmText == null || AudioManager.Instance == null || AudioManager.Instance.bgmSource == null) return;

        // Nếu loa nhạc đang bị Mute (Tắt) -> Hiện chữ "Bật" để mời gọi bấm
        if (AudioManager.Instance.bgmSource.mute)
            bgmText.text = "Bật nhạc nền";
        else
            bgmText.text = "Tắt nhạc nền";
    }

    private void UpdateSFXText()
    {
        if (sfxText == null || AudioManager.Instance == null || AudioManager.Instance.sfxSource == null) return;

        if (AudioManager.Instance.sfxSource.mute)
            sfxText.text = "Bật âm hiệu ứng";
        else
            sfxText.text = "Tắt âm hiệu ứng";
    }
}