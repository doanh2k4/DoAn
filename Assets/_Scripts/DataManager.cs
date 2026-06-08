using UnityEngine;
using System;
using System.IO;

// ĐÂY CHÍNH LÀ THẰNG GAMEDATA BỊ THIẾU KHIẾN UNITY BÁO LỖI NÀY:
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
        // Kiến trúc Singleton chuẩn để DataManager bất tử
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
        // Chốt sổ giờ giấc ngay trước khi cất vào két
        gameData.lastLoginTime = System.DateTime.Now.ToString();

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

            // ================= TÍNH TOÁN TIỀN IDLE TREO MÁY =================
            if (!string.IsNullOrEmpty(gameData.lastLoginTime))
            {
                System.DateTime lastLogin;
                if (System.DateTime.TryParse(gameData.lastLoginTime, out lastLogin))
                {
                    System.TimeSpan timeOffline = System.DateTime.Now - lastLogin;
                    double hoursOffline = timeOffline.TotalHours;

                    // Điều kiện: Off trên 6 phút (0.1 giờ) và phải từng thắng ít nhất 1 Level
                    if (hoursOffline > 0.1f && gameData.highestClearedWave > 0)
                    {
                        // Giới hạn chống hack/treo quá lâu: Tối đa nhận 24 giờ
                        if (hoursOffline > 24.0) hoursOffline = 24.0;

                        // Công thức: Giờ AFK * Kỷ lục * Hệ số (50)
                        int idleGold = (int)(hoursOffline * gameData.highestClearedWave * 50);
                        gameData.currentGold += idleGold;

                        Debug.Log($"<color=yellow>AFK {hoursOffline:F1} giờ. Nhận {idleGold} vàng từ Kỷ lục Level {gameData.highestClearedWave}!</color>");

                        // Cộng tiền xong phải lưu đè lại vào két ngay cho chắc ăn
                        SaveLocalData();
                    }
                }
            }
            // ================================================================

            Debug.Log("Đã tải dữ liệu thành công cho: " + gameData.username);
        }
        else
        {
            Debug.Log("Không tìm thấy file save. Tạo dữ liệu mới.");
            gameData = new GameData();
            SaveLocalData(); // Tạo file gốc luôn nếu user mới chơi lần đầu
        }
    }

    // Tự động lưu khi tắt game trên điện thoại hoặc bấm tắt Play trên Unity
    private void OnApplicationQuit()
    {
        SaveLocalData();
    }
}