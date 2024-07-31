using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScrollHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ScrollRect scrollRect;
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Matikan scrolling saat pointer di atas button
        scrollRect.vertical = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Aktifkan scrolling saat pointer keluar dari button
        scrollRect.vertical = true;
    }
}
