using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [Header("Giao diện Thông báo")]
    public TextMeshProUGUI warningText;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        // Khởi tạo Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Bất cứ file nào cũng có thể gọi hàm này và truyền câu chửi vào
    public void ShowWarning(string message)
    {
        if (warningText == null) return;

        // Cập nhật nội dung và hiện chữ lên
        warningText.text = message;
        warningText.gameObject.SetActive(true);

        // Dừng hiệu ứng cũ (nếu người chơi bấm liên tục) để chạy hiệu ứng mới
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        // 1. Hiện rõ ràng màu đỏ chót trong 1 giây đầu
        warningText.color = new Color(1f, 0f, 0f, 1f);
        yield return new WaitForSeconds(1f);

        // 2. Hiệu ứng mờ dần (Fade out) trong 0.5 giây tiếp theo
        float fadeDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            warningText.color = new Color(1f, 0f, 0f, alpha); // Giảm alpha xuống từ từ
            yield return null;
        }

        // 3. Mờ hẳn thì tắt luôn để tiết kiệm tài nguyên
        warningText.gameObject.SetActive(false);
    }
}