using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class HideUIButton : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    public void Hide() => HideAsync().Forget();

    private async UniTask HideAsync()
    {
        await LMotion.Create(1f, 0f, 2f)
            .WithEase(Ease.InOutSine)
            .BindToCanvasGroupAlpha(canvasGroup);

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
