using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // <-- BẮT BUỘC THÊM ĐỂ DÙNG TEXT UI

[System.Serializable]
public class SpawnData
{
    public int levelIndex;
    public int waveIndex;
    public int enemyIndex;
    public int amount;
    public float interval;
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Cài đặt Màn Chơi (Level)")]
    public int currentLevelToPlay = 1;

    [Header("File Cấu Hình Excel (CSV)")]
    public TextAsset csvFile;

    [Header("Cài đặt tài nguyên")]
    public GameObject[] enemyPrefabs;
    public Transform[] lanes;

    [Header("Giao diện Thông Báo")]
    public TextMeshProUGUI waveNotificationText; // <-- Biến chứa UI Text

    [Header("Trạng thái Wave hiện tại")]
    public int currentWave = 1;
    private int maxWaveInCurrentLevel = 1;
    private bool isSpawning = false;

    private Dictionary<int, List<SpawnData>> currentLevelWaves = new Dictionary<int, List<SpawnData>>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 1. THÊM DÒNG NÀY: Mở gói hàng để nhận số Level hiện tại (mặc định là 1)
        currentLevelToPlay = PlayerPrefs.GetInt("SelectedLevel", 1);
        Debug.Log($"<color=cyan>ĐANG TẢI DỮ LIỆU TỪ FILE CSV CHO LEVEL: {currentLevelToPlay}</color>");

        // Giấu thông báo đi lúc mới vào game
        if (waveNotificationText != null) waveNotificationText.gameObject.SetActive(false);

        // 2. Tự động nó sẽ dùng currentLevelToPlay vừa nhận để lọc CSV!
        LoadWaveDataFromCSV();
        StartCoroutine(LevelGameplayRoutine());
    }

    void LoadWaveDataFromCSV()
    {
        if (csvFile == null) return;
        string[] lines = csvFile.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        maxWaveInCurrentLevel = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');
            if (columns.Length < 5) continue;

            SpawnData data = new SpawnData();
            data.levelIndex = int.Parse(columns[0].Trim());
            data.waveIndex = int.Parse(columns[1].Trim());
            data.enemyIndex = int.Parse(columns[2].Trim());
            data.amount = int.Parse(columns[3].Trim());
            data.interval = float.Parse(columns[4].Trim());

            if (data.levelIndex == currentLevelToPlay)
            {
                if (!currentLevelWaves.ContainsKey(data.waveIndex))
                {
                    currentLevelWaves[data.waveIndex] = new List<SpawnData>();
                }
                currentLevelWaves[data.waveIndex].Add(data);

                if (data.waveIndex > maxWaveInCurrentLevel)
                    maxWaveInCurrentLevel = data.waveIndex;
            }
        }
    }

    IEnumerator LevelGameplayRoutine()
    {
        // ================= THÊM ĐOẠN NÀY ĐỂ HIỆN LEVEL =================
        if (waveNotificationText != null)
        {
            // Hiện chữ "LEVEL [số]" với màu xanh dương cho khác biệt
            yield return StartCoroutine(ShowNotificationRoutine($"LEVEL {currentLevelToPlay}", 2f, Color.cyan));

            // Đợi thêm 1 chút cho người chơi định thần trước khi vào Wave 1
            yield return new WaitForSeconds(0.5f);
        }
        // ===============================================================

        currentWave = 1;

        // Vòng lặp chạy từ Wave 1 đến Wave max
        while (currentWave <= maxWaveInCurrentLevel)
        {
            if (waveNotificationText != null)
                StartCoroutine(ShowNotificationRoutine($"WAVE {currentWave} BẮT ĐẦU!", 2f, Color.red));

            isSpawning = true;
            if (currentLevelWaves.ContainsKey(currentWave))
            {
                foreach (SpawnData spawnGroup in currentLevelWaves[currentWave])
                {
                    StartCoroutine(SpawnGroupRoutine(spawnGroup));
                }
            }

            yield return new WaitForSeconds(2f);
            isSpawning = false;

            while (isSpawning || GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
            {
                yield return new WaitForSeconds(1f);
            }

            if (waveNotificationText != null)
                StartCoroutine(ShowNotificationRoutine($"DỌN SẠCH WAVE {currentWave}!", 2f, Color.green));

            currentWave++;
            if (currentWave <= maxWaveInCurrentLevel) yield return new WaitForSeconds(3f);
        }

        // ================= ĐOẠN NÀY QUAN TRỌNG NHẤT =================
        // 1. Chờ hiệu ứng chữ Chiến Thắng hiện xong (chú ý có chữ yield return)
        if (waveNotificationText != null)
        {
            yield return StartCoroutine(ShowNotificationRoutine($"CHIẾN THẮNG LEVEL {currentLevelToPlay}!", 3f, Color.yellow));
        }

        // 2. CHỮ MỜ ĐI XONG THÌ BẬT BẢNG VICTORY VÀ LƯU DATA!
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Victory(); // Gọi điện sang GameManager!
        }
        else
        {
            Debug.LogError("LỖI KẾT NỐI: Không tìm thấy GameManager để báo cáo chiến thắng!");
        }
    }

    // ================= HIỆU ỨNG HIỂN THỊ CHỮ MƯỢT MÀ =================
    IEnumerator ShowNotificationRoutine(string message, float duration, Color textColor)
    {
        waveNotificationText.text = message;
        waveNotificationText.gameObject.SetActive(true);

        float fadeTime = 0.3f; // Thời gian chớp lên/mờ đi (0.3s)
        float elapsed = 0f;

        // FADE IN (Sáng dần)
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            waveNotificationText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }

        // CHỜ (Giữ nguyên chữ trên màn hình)
        yield return new WaitForSeconds(duration);

        // FADE OUT (Mờ dần)
        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            waveNotificationText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }

        waveNotificationText.gameObject.SetActive(false);
    }
    // =================================================================

    IEnumerator SpawnGroupRoutine(SpawnData data)
    {
        for (int i = 0; i < data.amount; i++)
        {
            SpawnSingleEnemy(data.enemyIndex);
            yield return new WaitForSeconds(data.interval);
        }
    }

    void SpawnSingleEnemy(int enemyIndex)
    {
        if (enemyIndex >= enemyPrefabs.Length) return;
        int randomLaneIndex = Random.Range(0, lanes.Length);
        Transform selectedLane = lanes[randomLaneIndex];

        GameObject newEnemy = Instantiate(enemyPrefabs[enemyIndex]);
        EnemyMovement moveScript = newEnemy.GetComponent<EnemyMovement>();
        if (moveScript != null) moveScript.SetupLane(selectedLane);
    }
}