using UnityEngine;
using System;
using System.IO;

[System.Serializable]
public class GameData
{
    public string username = "Player_7280";
    public float currentGold = 50f;
    public int castleLevel = 1;
    public int highestClearedWave = 0;
    public string lastLoginTime = "";
    public int[] elementLevels = new int[] { 1, 1, 1, 1, 1 };
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public GameData gameData;
    private string filePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            filePath = Application.persistentDataPath + "/savefile.json";
            LoadLocalData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveLocalData()
    {
        gameData.lastLoginTime = DateTime.Now.ToString();
        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(filePath, json);
        Debug.Log("Đã lưu dữ liệu Offline: " + json);
    }

    public void LoadLocalData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(json);

            if (!string.IsNullOrEmpty(gameData.lastLoginTime))
            {
                if (DateTime.TryParse(gameData.lastLoginTime, out DateTime lastLogin))
                {
                    TimeSpan timeOffline = DateTime.Now - lastLogin;
                    double hoursOffline = timeOffline.TotalHours;

                    if (hoursOffline > 0.1f && gameData.highestClearedWave > 0)
                    {
                        // Giới hạn tối đa 8 giờ
                        if (hoursOffline > 8.0) hoursOffline = 8.0;

                        // Lấy vàng thưởng mỗi giờ từ WaveManager (dựa vào level cao nhất đã clear)
                        int goldPerHour = 0;
                        if (WaveManager.levelGoldReward != null && WaveManager.levelGoldReward.TryGetValue(gameData.highestClearedWave, out goldPerHour))
                        {
                            int idleGold = (int)(hoursOffline * goldPerHour);
                            gameData.currentGold += idleGold;
                            Debug.Log($"<color=yellow>AFK {hoursOffline:F1} giờ. Nhận {idleGold} vàng từ Level {gameData.highestClearedWave} (thưởng {goldPerHour}/giờ)</color>");
                        }
                        else
                        {
                            // Fallback công thức cũ (phòng khi chưa có dữ liệu CSV)
                            int idleGold = (int)(hoursOffline * gameData.highestClearedWave * 50);
                            gameData.currentGold += idleGold;
                            Debug.LogWarning($"Không tìm thấy vàng thưởng cho Level {gameData.highestClearedWave}, dùng công thức cũ: {idleGold} vàng");
                        }

                        SaveLocalData(); // lưu lại luôn sau khi cộng tiền idle
                    }
                }
            }
            Debug.Log("Đã tải dữ liệu thành công cho: " + gameData.username);
        }
        else
        {
            Debug.Log("Không tìm thấy file save. Tạo dữ liệu mới.");
            gameData = new GameData();
            SaveLocalData();
        }
    }

    private void OnApplicationQuit()
    {
        SaveLocalData();
    }
}