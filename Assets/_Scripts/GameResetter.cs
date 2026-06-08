using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameResetter : MonoBehaviour
{
    public void ResetAllData()
    {
        // 1. Xóa hết dữ liệu cũ
        PlayerPrefs.DeleteAll();

        // 2. TẠO LẠI KÉT SẮT MỚI VỚI VỐN 50 VÀNG
        GameData newData = new GameData();
        newData.currentGold = 50f; // <--- ÉP CỨNG NÓ LÀ 50 NGAY ĐÂY

        // 3. Ghi đè file JSON mới với 50 vàng
        string json = JsonUtility.ToJson(newData);
        string filePath = Application.persistentDataPath + "/savefile.json";
        File.WriteAllText(filePath, json);

        // 4. Cập nhật luôn cho DataManager nếu đang mở game
        if (DataManager.Instance != null)
        {
            DataManager.Instance.gameData = newData;
        }

        Debug.Log("Đã Reset sạch! Vàng khởi nghiệp: 50.");
        SceneManager.LoadScene(0);
    }
}