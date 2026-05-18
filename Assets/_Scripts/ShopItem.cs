using UnityEngine;
using UnityEngine.EventSystems; // Bắt buộc phải có dòng này để UI nhận diện chuột

public class ShopItem : MonoBehaviour, IPointerDownHandler
{
    public int heroIndex; // 0=Hoa, 1=Kim, 2=Moc, 3=Tho, 4=Thuy

    // Khi người chơi bấm chuột xuống cái Thẻ
    public void OnPointerDown(PointerEventData eventData)
    {
        PlacementManager.Instance.StartDragging(heroIndex);
    }
}