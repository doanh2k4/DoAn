using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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
    public static Dictionary<int, int> levelGoldReward = new Dictionary<int, int>();

    [Header("Cài đặt Màn Chơi (Level)")]
    public int currentLevelToPlay = 1;

    [Header("File Cấu Hình Excel (CSV)")]
    public TextAsset csvFile;

    [Header("Cài đặt tài nguyên")]
    public GameObject[] enemyPrefabs;
    public Transform[] lanes;

    [Header("Giao diện Thông Báo")]
    public TextMeshProUGUI waveNotificationText;

    [Header("Trạng thái Wave hiện tại")]
    public int currentWave = 1;
    private int maxWaveInCurrentLevel = 1;
    private int activeSpawnGroups = 0;

    private Dictionary<int, List<SpawnData>> currentLevelWaves = new Dictionary<int, List<SpawnData>>();

    void Awake()
    {
        Instance = this;
        currentLevelToPlay = PlayerPrefs.GetInt("SelectedLevel", 1);
        LoadWaveDataFromCSV(); // Load dữ liệu CSV ngay lập tức để DataManager có thể đọc được
    }

    void Start()
    {
        if (waveNotificationText != null) waveNotificationText.gameObject.SetActive(false);

        if (currentLevelWaves.Count == 0)
        {
            Debug.LogError($"Level {currentLevelToPlay} không tồn tại! Đưa người chơi về Main Menu.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            return;
        }

        StartCoroutine(LevelGameplayRoutine());
    }

    void LoadWaveDataFromCSV()
    {
        if (csvFile == null) return;
        string[] lines = csvFile.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        maxWaveInCurrentLevel = 0;
        levelGoldReward.Clear(); // Xóa dữ liệu cũ

        int totalLevelsInGame = 1;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');
            if (columns.Length < 5) continue; // Cần ít nhất 5 cột cơ bản

            SpawnData data = new SpawnData();
            data.levelIndex = int.Parse(columns[0].Trim());
            data.waveIndex = int.Parse(columns[1].Trim());
            data.enemyIndex = int.Parse(columns[2].Trim());
            data.amount = int.Parse(columns[3].Trim());
            data.interval = float.Parse(columns[4].Trim());

            // Đọc cột thứ 6: GoldReward (nếu có)
            if (columns.Length >= 6)
            {
                int goldReward = int.Parse(columns[5].Trim());
                if (!levelGoldReward.ContainsKey(data.levelIndex))
                    levelGoldReward.Add(data.levelIndex, goldReward);
                else
                    levelGoldReward[data.levelIndex] = goldReward; // ghi đè (các dòng cùng level nên có giá trị giống nhau)
            }

            if (data.levelIndex > totalLevelsInGame) totalLevelsInGame = data.levelIndex;

            if (data.levelIndex == currentLevelToPlay)
            {
                if (!currentLevelWaves.ContainsKey(data.waveIndex))
                    currentLevelWaves[data.waveIndex] = new List<SpawnData>();
                currentLevelWaves[data.waveIndex].Add(data);

                if (data.waveIndex > maxWaveInCurrentLevel)
                    maxWaveInCurrentLevel = data.waveIndex;
            }
        }

        PlayerPrefs.SetInt("TotalLevelsInGame", totalLevelsInGame);
    }

    IEnumerator LevelGameplayRoutine()
    {
        if (waveNotificationText != null)
        {
            yield return StartCoroutine(ShowNotificationRoutine($"LEVEL {currentLevelToPlay}", 2f, Color.cyan));
            yield return new WaitForSeconds(0.5f);
        }

        currentWave = 1;

        while (currentWave <= maxWaveInCurrentLevel)
        {
            if (waveNotificationText != null)
                StartCoroutine(ShowNotificationRoutine($"WAVE {currentWave} BẮT ĐẦU!", 2f, Color.red));

            if (currentLevelWaves.ContainsKey(currentWave))
            {
                foreach (SpawnData spawnGroup in currentLevelWaves[currentWave])
                {
                    StartCoroutine(SpawnGroupRoutine(spawnGroup));
                }
            }

            yield return new WaitUntil(() => activeSpawnGroups == 0 && GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

            if (waveNotificationText != null)
                StartCoroutine(ShowNotificationRoutine($"DỌN SẠCH WAVE {currentWave}!", 2f, Color.green));

            currentWave++;
            if (currentWave <= maxWaveInCurrentLevel) yield return new WaitForSeconds(3f);
        }

        if (waveNotificationText != null)
        {
            yield return StartCoroutine(ShowNotificationRoutine($"CHIẾN THẮNG LEVEL {currentLevelToPlay}!", 3f, Color.yellow));
        }

        if (GameManager.Instance != null)
            GameManager.Instance.Victory();
        else
            Debug.LogError("LỖI KẾT NỐI: Không tìm thấy GameManager để báo cáo chiến thắng!");
    }

    IEnumerator ShowNotificationRoutine(string message, float duration, Color textColor)
    {
        waveNotificationText.text = message;
        waveNotificationText.gameObject.SetActive(true);

        float fadeTime = 0.3f;
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            waveNotificationText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(duration);

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

    IEnumerator SpawnGroupRoutine(SpawnData data)
    {
        activeSpawnGroups++;

        for (int i = 0; i < data.amount; i++)
        {
            SpawnSingleEnemy(data.enemyIndex);
            yield return new WaitForSeconds(data.interval);
        }

        activeSpawnGroups--;
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