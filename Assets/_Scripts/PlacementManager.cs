using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    [Header("Danh sách 5 Tướng xịn")]
    public GameObject[] heroPrefabs;

    private GameObject selectedHeroPrefab;
    private GameObject draggingPreview;
    private HeroSlot hoveredSlot;

    void Awake() { Instance = this; }

    public void StartDragging(int heroIndex)
    {
        selectedHeroPrefab = heroPrefabs[heroIndex];
        draggingPreview = Instantiate(selectedHeroPrefab);

        // 1. Tắt não (Script)
        foreach (MonoBehaviour script in draggingPreview.GetComponents<MonoBehaviour>())
        {
            script.enabled = false;
        }

        // 2. DIỆT TẬN GỐC LỖI NHẢY: Đóng băng vật lý ngay lập tức!
        Rigidbody2D rb = draggingPreview.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false; // Tắt sạch tính toán va chạm

        // Tắt luôn Animator để bóng ma đứng im
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
        if (selectedHeroPrefab == null || draggingPreview == null || Mouse.current == null) return;

        // Vị trí chuột
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mousePos.z = 0;
        draggingPreview.transform.position = mousePos;

        SetDraggingPreviewVisibility(true);

        // TUYỆT CHIÊU: Bắn Laser xuyên thấu mọi thứ (RaycastAll)
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        HeroSlot currentHitSlot = null;

        // Lục tìm trong danh sách các vật bị đâm trúng
        foreach (RaycastHit2D hit in hits)
        {
            HeroSlot slot = hit.collider.GetComponent<HeroSlot>();
            if (slot != null)
            {
                currentHitSlot = slot; // Đã tìm thấy Slot!
                break; // Tìm thấy rồi thì dừng lại
            }
        }

        ManageHighlights(currentHitSlot);

        if (currentHitSlot != null && currentHitSlot.isOccupied == true)
        {
            SetDraggingPreviewVisibility(false);
        }

        // Nhả chuột
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            TryPlaceHero(currentHitSlot);
        }
    }

    void TryPlaceHero(HeroSlot slotToPlace)
    {
        // Nếu lúc thả chuột đang đè lên một Slot trống
        if (slotToPlace != null && slotToPlace.isOccupied == false)
        {
            slotToPlace.PlaceHero(selectedHeroPrefab);
        }

        // Dọn dẹp
        if (hoveredSlot != null)
        {
            hoveredSlot.HideHighlight();
            hoveredSlot = null;
        }
        Destroy(draggingPreview);
        selectedHeroPrefab = null;
    }
}