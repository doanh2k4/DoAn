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

            if (Castle.Instance != null)
            {
                Castle.Instance.Heal(50);
                StartCoroutine(HealFXRoutine());
                Debug.Log("<color=green>Đã dùng Bơm Máu!</color>");
            }
        }
        //else Debug.Log("Không đủ tiền Bơm Máu!");
        { if (NotificationManager.Instance != null) NotificationManager.Instance.ShowWarning("Không đủ Vàng dùng kỹ năng!"); }
    }

    IEnumerator HealFXRoutine()
    {
        // Lấy hình ảnh của Lâu đài, đổi sang màu xanh lá và phình to ra
        SpriteRenderer castleSprite = Castle.Instance.GetComponent<SpriteRenderer>();
        if (castleSprite == null) castleSprite = Castle.Instance.GetComponentInChildren<SpriteRenderer>();

        if (castleSprite != null) castleSprite.color = Color.green;
        Castle.Instance.transform.localScale = Vector3.one * 1.15f;

        yield return new WaitForSeconds(0.2f); // Giữ trong 0.2 giây

        // Trả lại bình thường
        if (castleSprite != null) castleSprite.color = Color.white;
        Castle.Instance.transform.localScale = Vector3.one;
    }

    // ================= 2. MƯA SAO BĂNG =================
    public void UseNuke()
    {
        if (GameManager.Instance.HasEnoughGold(nukeCost))
        {
            GameManager.Instance.SpendGold(nukeCost);

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
        //else Debug.Log("Không đủ tiền gọi Mưa Sao Băng!");
        { if (NotificationManager.Instance != null) NotificationManager.Instance.ShowWarning("Không đủ Vàng dùng kỹ năng!"); }
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

    IEnumerator CameraShakeRoutine(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-1f, 1f) * magnitude;
            float y = originalPos.y + Random.Range(-1f, 1f) * magnitude;
            Camera.main.transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.localPosition = originalPos; // Trả camera về chỗ cũ
    }

    // ================= 3. ĐÓNG BĂNG =================
    public void UseFreeze()
    {
        if (GameManager.Instance.HasEnoughGold(freezeCost))
        {
            GameManager.Instance.SpendGold(freezeCost);

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
        //else Debug.Log("Không đủ tiền Đóng băng!");
        { if (NotificationManager.Instance != null) NotificationManager.Instance.ShowWarning("Không đủ Vàng dùng kỹ năng!"); }
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