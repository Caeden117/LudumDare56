using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsWheelRotate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isHovering = false;

    private async UniTask Start()
    {
        while (true)
        {
            await UniTask.WaitUntil(() => isHovering);

            await LMotion.Create(0, 45f, 1f)
                .WithEase(Ease.InCubic)
                .Bind(it => transform.eulerAngles = it * Vector3.back);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => isHovering = true;

    public void OnPointerExit(PointerEventData eventData) => isHovering = false;
}
