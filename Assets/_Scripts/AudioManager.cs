using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Các Kênh Phát (Loa)")]
    public AudioSource bgmSource; // Loa phát nhạc nền
    public AudioSource sfxSource; // Loa phát hiệu ứng (bùm chéo)

    [Header("Kho Nhạc Nền (BGM)")]
    public AudioClip mainMusic;

    [Header("Kho Hiệu Ứng (SFX)")]
    public AudioClip btnClickSound;
    public AudioClip healSound;
    public AudioClip nukeSound;
    public AudioClip freezeSound;
    public AudioClip enemyDeathSound;
    public AudioClip notEnoughGoldSound;
    [Header("Âm thanh Hệ thống & Chiến đấu")]
    public AudioClip winSound;          // Tiếng Thắng (Victory)
    public AudioClip loseSound;         // Tiếng Thua (Game Over)
    public AudioClip waveWarningSound;  // Tiếng còi báo động / Tù và khi có Wave mới
    public AudioClip castleHitSound;    // Tiếng gạch đá vỡ khi Lâu đài bị đánh
    public AudioClip bowShootSound;     // Tiếng bắn cung của Tướng

    private void Awake()
    {
        // Khởi tạo Singleton bất tử để xài chung cho mọi màn chơi
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Vào game là tự động bật nhạc nền luôn
        if (mainMusic != null)
        {
            bgmSource.clip = mainMusic;
            bgmSource.loop = true; // Nhạc nền thì phải lặp lại
            bgmSource.Play();
        }
    }

    // Hàm dùng chung: File nào muốn phát tiếng cứ gọi hàm này và truyền file nhạc vào
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip); // PlayOneShot giúp phát đè nhiều tiếng cùng lúc không bị ngắt
        }
    }

    // Thêm hàm này để gọi riêng cho các nút bấm UI
    public void PlayButtonSound()
    {
        if (btnClickSound != null)
        {
            PlaySFX(btnClickSound);
        }
    }
}