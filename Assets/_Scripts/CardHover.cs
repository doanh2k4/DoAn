using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Bắt buộc để điều khiển LayoutElement

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private LayoutElement layoutElement;
    private Canvas canvas;

    private Vector3 originalScale;
    private float originalWidth;
    private int originalSortingOrder;

    [Header("Cấu hình Hiệu ứng nổi")]
    public float hoverScale = 1.2f; // Phóng to lên 20%
    public float speed = 15f;       // Tốc độ chuyển động mượt

    private bool isHovering = false;

    void Start()
    {
        // 1. Lưu lại tỉ lệ scale gốc ban đầu của thẻ
        originalScale = transform.localScale;

        // 2. Lấy kích thước bề rộng thực tế của thẻ bài
        RectTransform rectTransform = GetComponent<RectTransform>();
        originalWidth = rectTransform.rect.width;

        // 3. Tự động tìm hoặc nạp LayoutElement để xử lý dạt dòng
        layoutElement = GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = gameObject.AddComponent<LayoutElement>();
        }
        layoutElement.preferredWidth = originalWidth;

        // 4. Lấy component Canvas để hỗ trợ đè lên tầng trên cùng
        canvas = GetComponent<Canvas>();
    }

    void Update()
    {
        // Tính toán kích thước mục tiêu
        Vector3 targetScale = isHovering ? originalScale * hoverScale : originalScale;
        float targetWidth = isHovering ? originalWidth * hoverScale : originalWidth;

        // MẸO PHỐI HỢP: Vừa to scale trực quan, vừa nới rộng layout để dạt bạn bên cạnh
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
        layoutElement.preferredWidth = Mathf.Lerp(layoutElement.preferredWidth, targetWidth, Time.deltaTime * speed);
    }

    // Khi chuột lướt VÀO thẻ bài
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        // Đưa thẻ lên tầng trên cùng để đè lên các thẻ bên cạnh, tránh bị che khuất viền
        if (canvas != null)
        {
            originalSortingOrder = canvas.sortingOrder;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 50; // Cho số to hẳn lên để nổi lên trên cùng
        }
    }

    // Khi chuột lướt RA KHỎI thẻ bài
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        // Trả lại tầng hiển thị cũ cho thẻ bài
        if (canvas != null)
        {
            canvas.overrideSorting = false;
            canvas.sortingOrder = originalSortingOrder;
        }
    }
}