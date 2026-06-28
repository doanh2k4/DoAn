using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // BẮT BUỘC PHẢI CÓ THƯ VIỆN NÀY ĐỂ TƯƠNG TÁC FILE

public class ResetManager : MonoBehaviour
{
    public void ResetAllData()
    {
        // 1. Mò đúng địa chỉ cái Két sắt JSON
        string filePath = Application.persistentDataPath + "/savefile.json";

        // 2. Nếu thấy file đó tồn tại thì XÓA SỔ NÓ
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Đã tiêu hủy file savefile.json");
        }

        // 3. Xóa luôn PlayerPrefs (phòng hờ cài đặt âm thanh, level vặt vãnh)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // 4. Bóp cổ DataManager và GameManager cũ đang ngậm 15 Vàng trong RAM
        if (DataManager.Instance != null) Destroy(DataManager.Instance.gameObject);
        if (GameManager.Instance != null) Destroy(GameManager.Instance.gameObject);

        Debug.Log("<color=green>ĐÃ RESET GAME TỪ GỐC RỄ!</color>");

        // 5. Load lại Main Menu. 
        // Khi Load lại, DataManager mới đẻ ra sẽ thấy mất file JSON 
        // -> Nó tự tạo file mới với GameData mặc định là 50 Vàng!
        SceneManager.LoadScene(0);
    }
}