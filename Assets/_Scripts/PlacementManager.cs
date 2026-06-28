using UnityEngine;
using UnityEngine.InputSystem; // BẮT BUỘC ĐỂ DÙNG POINTER

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    [Header("Danh sách 5 Tướng xịn")]
    public GameObject[] heroPrefabs;

    private GameObject selectedHeroPrefab;
    private GameObject draggingPreview;
    private HeroSlot hoveredSlot;

    private int currentHeroLevel;
    private int currentHeroCost;

    void Awake() { Instance = this; }

    public void StartDragging(int heroIndex, int cost, int level)
    {
        // Dọn sạch bóng ma cũ nếu người chơi lỡ chạm đúp để nâng cấp
        if (draggingPreview != null) Destroy(draggingPreview);

        // KIỂM TRA TIỀN TRƯỚC KHI CHO KÉO
        if (GameManager.Instance != null && !GameManager.Instance.HasEnoughGold(cost))
        {
            if (NotificationManager.Instance != null)
                NotificationManager.Instance.ShowWarning("Không đủ Vàng để gọi lính!");

            if (AudioManager.Instance != null && AudioManager.Instance.notEnoughGoldSound != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.notEnoughGoldSound);

            return; // Hết tiền thì chặn luôn
        }

        currentHeroCost = cost;
        currentHeroLevel = level;

        selectedHeroPrefab = heroPrefabs[heroIndex];
        draggingPreview = Instantiate(selectedHeroPrefab);

        foreach (MonoBehaviour script in draggingPreview.GetComponents<MonoBehaviour>())
        {
            script.enabled = false;
        }

        Rigidbody2D rb = draggingPreview.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        Animator anim = draggingPreview.GetComponent<Animator>();
        if (anim != null) anim.enabled = false;
    }

    private void SetDraggingPreviewVisibility(bool visible)
    {
        if (draggingPreview == null) return;
        foreach (SpriteRenderer sr in draggingPreview.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = visible;
        }
    }

    private void ManageHighlights(HeroSlot currentHitSlot)
    {
        if (hoveredSlot != null && hoveredSlot != currentHitSlot)
        {
            hoveredSlot.HideHighlight();
        }
        if (currentHitSlot != null && currentHitSlot.isOccupied == false)
        {
            currentHitSlot.ShowHighlight();
        }
        hoveredSlot = currentHitSlot;
    }

    void Update()
    {
        // 1. HỦY KÉO KHI BẤM CHUỘT PHẢI (Chỉ dùng cho lúc test trên PC)
        if (selectedHeroPrefab != null && draggingPreview != null)
        {
            // Phải check Mouse.current != null để không bị lỗi liệt cảm ứng khi build lên điện thoại
            if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (hoveredSlot != null)
                {
                    hoveredSlot.HideHighlight();
                    hoveredSlot = null;
                }

                Destroy(draggingPreview);
                selectedHeroPrefab = null;
                return;
            }
        }

        // 2. KÉO LÍNH BẰNG NGÓN TAY/CHUỘT
        // SỬA Ở ĐÂY: Đổi Mouse thành Pointer để điện thoại hiểu được
        if (selectedHeroPrefab == null || draggingPreview == null || Pointer.current == null) return;

        // Đọc vị trí ngón tay chạm trên màn hình
        Vector2 pointerScreenPosition = Pointer.current.position.ReadValue();
        Vector3 pointerPos = Camera.main.ScreenToWorldPoint(pointerScreenPosition);
        pointerPos.z = 0;
        draggingPreview.transform.position = pointerPos;

        SetDraggingPreviewVisibility(true);

        RaycastHit2D[] hits = Physics2D.RaycastAll(pointerPos, Vector2.zero);
        HeroSlot currentHitSlot = null;

        foreach (RaycastHit2D hit in hits)
        {
            HeroSlot slot = hit.collider.GetComponent<HeroSlot>();
            if (slot != null)
            {
                currentHitSlot = slot;
                break;
            }
        }

        ManageHighlights(currentHitSlot);

        if (currentHitSlot != null && currentHitSlot.isOccupied == true)
        {
            SetDraggingPreviewVisibility(false);
        }

        // 3. THẢ LÍNH XUỐNG
        // SỬA Ở ĐÂY: Nhận biết thao tác nhấc ngón tay lên khỏi màn hình (hoặc nhả chuột)
        if (Pointer.current.press.wasReleasedThisFrame)
        {
            TryPlaceHero(currentHitSlot);
        }
    }

    void TryPlaceHero(HeroSlot slotToPlace)
    {
        if (slotToPlace != null && slotToPlace.isOccupied == false)
        {
            if (GameManager.Instance != null && GameManager.Instance.HasEnoughGold(currentHeroCost))
            {
                GameManager.Instance.SpendGold(currentHeroCost);
                slotToPlace.PlaceHero(selectedHeroPrefab, currentHeroLevel);

                if (AudioManager.Instance != null && AudioManager.Instance.btnClickSound != null)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.btnClickSound);
            }
        }

        if (hoveredSlot != null)
        {
            hoveredSlot.HideHighlight();
            hoveredSlot = null;
        }
        Destroy(draggingPreview);
        selectedHeroPrefab = null;
    }
}