using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI playButtonText; // Kéo chữ của Nút Play vào đây

    void Start()
    {
        if (DataManager.Instance != null)
        {
            // Hiển thị tiền
            goldText.text = "Vàng: " + DataManager.Instance.gameData.currentGold.ToString();

            // Lấy kỷ lục hiện tại
            int highestWave = DataManager.Instance.gameData.highestClearedWave;

            // Kiểm tra: Nếu chưa chơi (Kỷ lục = 0) thì báo "Bắt đầu"
            // Nếu đã thắng Level 1 -> Cấp tiếp theo là 2 -> Báo "Tiếp tục Level 2"
            if (highestWave == 0)
            {
                playButtonText.text = "BẮT ĐẦU";
            }
            else
            {
                playButtonText.text = $"TIẾP TỤC LEVEL {highestWave + 1}";
            }
        }
    }

    public void PlayGame()
    {
        // Tính toán Level tiếp theo cần chơi
        int nextLevel = 1;
        if (DataManager.Instance != null)
        {
            nextLevel = DataManager.Instance.gameData.highestClearedWave + 1;
        }

        // Gửi số Level này qua màn chơi thông qua PlayerPrefs
        PlayerPrefs.SetInt("SelectedLevel", nextLevel);

        // Load sang màn chiến đấu
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Đang thoát game...");

        // 1. Lệnh này sẽ chạy khi cậu Build ra game thật (.exe, .apk)
        Application.Quit();

        // 2. Lệnh này giúp dừng chế độ Play ngay trong Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}