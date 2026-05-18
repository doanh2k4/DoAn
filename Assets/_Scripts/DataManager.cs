using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public PlayerData gameData; // Biến chứa toàn bộ dữ liệu đang chơi

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ bộ não này sống qua mọi màn chơi
            LoadLocalData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveLocalData()
    {
        // Biến toàn bộ cục gameData thành chuỗi văn bản (JSON)
        string jsonData = JsonUtility.ToJson(gameData);

        // Lưu chuỗi đó vào ổ cứng điện thoại/PC
        PlayerPrefs.SetString("GameDataSave", jsonData);
        PlayerPrefs.Save();

        Debug.Log("Đã lưu dữ liệu Offline: " + jsonData);

        // TODO (Sau này làm): Push cái chuỗi jsonData này lên Firebase tại đây
    }

    private void LoadLocalData()
    {
        string jsonData = PlayerPrefs.GetString("GameDataSave", "");

        if (string.IsNullOrEmpty(jsonData))
        {
            // Lần đầu chơi game, tạo mới
            gameData = new PlayerData();
            Debug.Log("Tạo dữ liệu mới cho: " + gameData.username);
        }
        else
        {
            // Chơi từ lần 2, dịch chuỗi văn bản ngược lại thành cấu trúc gameData
            gameData = JsonUtility.FromJson<PlayerData>(jsonData);
            Debug.Log("Đã tải dữ liệu thành công cho: " + gameData.username);
        }
    }

    private void OnApplicationQuit()
    {
        gameData.lastLoginTime = System.DateTime.Now.ToString();
        SaveLocalData();
    }
}