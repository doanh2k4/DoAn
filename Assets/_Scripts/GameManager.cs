using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Kinh tế")]
    // KHÔNG CÒN BIẾN currentGold Ở ĐÂY NỮA
    public TextMeshProUGUI goldText;

    [Header("Giao diện Thắng / Thua")]
    public GameObject winPanel;
    public GameObject losePanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateGoldUI();

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    // ================= LOGIC TIỀN TỆ MỚI (DÙNG KÉT SẮT GLOBAL) =================

    public bool HasEnoughGold(int amount)
    {
        if (DataManager.Instance != null)
        {
            return DataManager.Instance.gameData.currentGold >= amount;
        }
        return false;
    }

    public void SpendGold(int amount)
    {
        if (DataManager.Instance != null && DataManager.Instance.gameData.currentGold >= amount)
        {
            DataManager.Instance.gameData.currentGold -= amount;
            DataManager.Instance.SaveLocalData(); // Lưu ví ngay sau khi tiêu
            UpdateGoldUI();
        }
    }

    public void AddGold(int amount)
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.gameData.currentGold += amount;
            DataManager.Instance.SaveLocalData(); // Lưu ví ngay sau khi nhặt tiền
            UpdateGoldUI();
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText != null && DataManager.Instance != null)
            goldText.text = "Vàng: " + DataManager.Instance.gameData.currentGold.ToString();
    }

    // ================= XỬ LÝ THẮNG THUA VÀ LƯU DATA =================

    public void GameOver()
    {
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Victory()
    {
        Debug.Log("<color=cyan>--- BẮT ĐẦU CHẠY HÀM VICTORY ---</color>");

        if (winPanel != null) winPanel.SetActive(true);

        if (DataManager.Instance != null && WaveManager.Instance != null)
        {
            int currentLevel = WaveManager.Instance.currentLevelToPlay;

            if (currentLevel > DataManager.Instance.gameData.highestClearedWave)
            {
                DataManager.Instance.gameData.highestClearedWave = currentLevel;
                Debug.Log($"<color=green>ĐÃ LƯU KỶ LỤC MỚI: Level {currentLevel}</color>");
            }

            // VÌ ĐÃ GỘP TIỀN, NÊN CHỈ CÒN TIỀN THƯỞNG QUA MÀN
            int winRewardGold = currentLevel * 100;
            DataManager.Instance.gameData.currentGold += winRewardGold;

            Debug.Log($"<color=orange>Thắng trận! Thưởng thêm: {winRewardGold}. Két sắt hiện có: {DataManager.Instance.gameData.currentGold}</color>");

            DataManager.Instance.SaveLocalData();
            UpdateGoldUI();
        }

        Time.timeScale = 0f;
    }

    // ================= CÁC HÀM ĐỂ GẮN VÀO NÚT BẤM =================

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Load thẳng về Scene số 0 (MainMenu)
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        if (WaveManager.Instance != null)
        {
            int nextLevel = WaveManager.Instance.currentLevelToPlay + 1;
            PlayerPrefs.SetInt("SelectedLevel", nextLevel);
            Debug.Log($"Đang tiến vào Level {nextLevel}...");
        }

        SceneManager.LoadScene(1);
    }
}