using UnityEngine;
using UnityEngine.EventSystems;

public class WindowInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TransparancyManager transparancyManager;

    public void OnPointerEnter(PointerEventData eventData)
        => transparancyManager.SetInteractable(true);

    public void OnPointerExit(PointerEventData eventData)
        => transparancyManager.SetInteractable(false);
}
