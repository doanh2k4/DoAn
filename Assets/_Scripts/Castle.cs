using UnityEngine;

public class Castle : MonoBehaviour
{
    public static Castle Instance { get; private set; }

    [Header("Chỉ số Lâu đài")]
    public float maxHealth = 1000f; // Máu tối đa
    public float currentHealth;

    private void Awake()
    {
        // Đảm bảo chỉ có 1 Lâu đài trên bản đồ
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentHealth = maxHealth; // Hồi đầy máu khi bắt đầu game
    }

    // Hàm này sẽ được gọi khi quái đâm vào hoặc bắn trúng
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // In ra Console để dễ Test
        Debug.Log($"<color=red>Lâu đài bị tấn công! Trừ {amount} máu. Còn lại: {currentHealth}</color>");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("<color=yellow>LÂU ĐÀI ĐÃ SẬP! GAME OVER WAVE NÀY!</color>");
            // (Bài sau chúng ta sẽ móc nối code để lùi Wave khi sập Lâu đài)
        }
    }
}