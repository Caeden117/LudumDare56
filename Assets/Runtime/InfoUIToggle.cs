using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class InfoUIToggle : MonoBehaviour {
    private RectTransform rectTransform;
    [SerializeField] private Button button;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnButtonPress() {
        ButtonPressAsync().Forget();
    }

    private async UniTask ButtonPressAsync() {
        button.interactable = false;
        bool currentlyVisible = canvasGroup.alpha > 0.5f;
        canvasGroup.interactable = !currentlyVisible;
        canvasGroup.blocksRaycasts = !currentlyVisible;
        LMotion.Create(1.0f, 0.9f, 0.1f)
            .WithEase(Ease.OutQuad)
            .Bind(it => rectTransform.localScale = Vector3.one * it).ToUniTask().Forget();
        float targetAlpha = currentlyVisible ? 0.0f : 1.0f;
        await LMotion.Create(canvasGroup.alpha, targetAlpha, 0.1f).BindToCanvasGroupAlpha(canvasGroup);
        LMotion.Create(0.9f, 1.0f, 0.1f)
            .WithEase(Ease.InQuad)
            .Bind(it => rectTransform.localScale = Vector3.one * it).ToUniTask().Forget();
        button.interactable = true;
    }
}
