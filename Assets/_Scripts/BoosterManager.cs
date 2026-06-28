using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoosterManager : MonoBehaviour
{
    [Header("Giá Tiền Kỹ Năng")]
    public int healCost = 200;
    public int nukeCost = 1000;
    public int freezeCost = 500;

    [Header("Hiệu ứng Mưa Sao Băng")]
    [Tooltip("Kéo một UI Image màu trắng che full màn hình vào đây để làm flash")]
    public Image flashScreen;

    // ================= 1. BƠM MÁU =================
    public void UseHeal()
    {
        if (GameManager.Instance.HasEnoughGold(healCost))
        {
            GameManager.Instance.SpendGold(healCost);
            // THÊM DÒNG NÀY: Phát tiếng Bơm máu
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.healSound);

            if (Castle.Instance != null)
            {
                // Chỉ cần gọi hàm Heal, Lâu đài sẽ tự lo chuyện chớp sáng
                Castle.Instance.Heal(50);
                Debug.Log("<color=green>Đã dùng Bơm Máu!</color>");
            }
        }
        else
        {
            // Đã bọc else chuẩn chỉnh, thông báo không đủ tiền sẽ chạy đúng
            if (NotificationManager.Instance != null) NotificationManager.Instance.ShowWarning("Không đủ Vàng dùng kỹ năng!");
            // 📢 THÊM DÒNG NÀY: Tiếng báo lỗi hết tiền
            if (AudioManager.Instance != null && AudioManager.Instance.notEnoughGoldSound != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.notEnoughGoldSound);
        }
    }

    // ================= 2. MƯA SAO BĂNG =================
    public void UseNuke()
    {
        if (GameManager.Instance.HasEnoughGold(nukeCost))
        {
            GameManager.Instance.SpendGold(nukeCost);
            // THÊM DÒNG NÀY VÀO TRONG USE NUKE: Phát tiếng Mưa sao băng nổ
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.nukeSound);

            // Tìm toàn bộ quái và gây 100 sát thương
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                EnemyMovement em = enemy.GetComponent<EnemyMovement>();
                if (em != null) em.TakeDamage(100f);
            }

            // Gọi hiệu ứng Rung camera và Chớp màn hình
            StartCoroutine(NukeFlashRoutine());
            StartCoroutine(CameraShakeRoutine(0.4f, 0.3f));
            Debug.Log("<color=red>Đã dùng Mưa Sao Băng (NUKE)!</color>");
        }
        else
        {
            if (NotificationManager.Instance != null) NotificationManager.Instance.ShowWarning("Không đủ Vàng dùng kỹ năng!");
            // 📢 THÊM DÒNG NÀY: Tiếng báo lỗi hết tiền
            if (AudioManager.Instance != null && AudioManager.Instance.notEnoughGoldSound != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.notEnoughGoldSound);
        }
    }

    IEnumerator NukeFlashRoutine()
    {
        if (flashScreen != null)
        {
            flashScreen.gameObject.SetActive(true);
            flashScreen.color = new Color(1f, 0f, 0f, 0.5f); // Chớp màu đỏ mờ
            yield return new WaitForSeconds(0.1f);
            flashScreen.color = new Color(1f, 1f, 1f, 0.8f); // Chớp màu trắng sáng
            yield return new WaitForSeconds(0.1f);
            flashScreen.gameObject.SetActive(false); // Tắt đi
        }
    }

    // ================= SỬA LỖI HIỆU NĂNG Ở ĐÂY =================
    IEnumerator CameraShakeRoutine(float duration, float magnitude)
    {
        // 1. Chặn lỗi đỏ màn hình nếu không có Camera
        if (Camera.main == null) yield break;

        // 2. Cache Transform lại để không bắt CPU tìm Camera mỗi frame
        Transform camTransform = Camera.main.transform;
        Vector3 originalPos = camTransform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-1f, 1f) * magnitude;
            float y = originalPos.y + Random.Range(-1f, 1f) * magnitude;

            // Dùng biến cache thay vì Camera.main
            camTransform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Trả camera về chỗ cũ bằng biến cache
        camTransform.localPosition = originalPos;
    }

    // ================= 3. ĐÓNG BĂNG =================
    public void UseFreeze()
    {
        if (GameManager.Instance.HasEnoughGold(freezeCost))
        {
            GameManager.Instance.SpendGold(freezeCost);
            // THÊM DÒNG NÀY VÀO TRONG USE FREEZE: Phát tiếng Đóng băng
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.freezeSound);

            // Tìm quái và trói chân 3 giây
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                EnemyMovement em = enemy.GetComponent<EnemyMovement>();
                if (em != null)
                {
                    em.ApplyRoot(3f);
                    StartCoroutine(FreezeColorFX(enemy, 3f));
                }
            }
            Debug.Log("<color=cyan>Đã dùng Đóng Băng Thời Gian!</color>");
        }
        else
        {
            if (NotificationManager.Instance != null) NotificationManager.Instance.ShowWarning("Không đủ Vàng dùng kỹ năng!");
            // 📢 THÊM DÒNG NÀY: Tiếng báo lỗi hết tiền
            if (AudioManager.Instance != null && AudioManager.Instance.notEnoughGoldSound != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.notEnoughGoldSound);
        }
    }

    IEnumerator FreezeColorFX(GameObject enemyObj, float duration)
    {
        SpriteRenderer sr = enemyObj.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Color oldColor = sr.color;
            sr.color = Color.cyan; // Nhuộm quái thành màu Xanh Ngọc

            yield return new WaitForSeconds(duration);

            // Hết giờ đóng băng, nếu quái chưa chết thì trả lại màu cũ
            if (sr != null) sr.color = oldColor;
        }
    }
}