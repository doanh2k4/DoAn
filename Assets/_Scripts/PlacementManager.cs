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

    private int currentHeroLevel;
    private int currentHeroCost;

    void Awake() { Instance = this; }

    // ĐÃ SỬA: Hàm nhận chính xác 3 tham số (heroIndex, cost, level) từ ShopItem truyền sang
    public void StartDragging(int heroIndex, int cost, int level)
    {
        currentHeroCost = cost; // Ghi nhớ giá tiền
        currentHeroLevel = level; // ĐÃ SỬA: Lấy đúng biến 'level' từ tham số truyền vào

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
        if (selectedHeroPrefab == null || draggingPreview == null || Mouse.current == null) return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mousePos.z = 0;
        draggingPreview.transform.position = mousePos;

        SetDraggingPreviewVisibility(true);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
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

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            TryPlaceHero(currentHitSlot);
        }
    }

    void TryPlaceHero(HeroSlot slotToPlace)
    {
        if (slotToPlace != null && slotToPlace.isOccupied == false)
        {
            slotToPlace.PlaceHero(selectedHeroPrefab, currentHeroLevel);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SpendGold(currentHeroCost);
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