using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private bool isDragging = false;
    public void OnDrag(PointerEventData eventData) {
        if (isDragging) {
            transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0.0f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject) {
            isDragging = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        isDragging = false;
    }
}
