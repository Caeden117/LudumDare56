using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData) {
        transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0.0f);
    }
}
