using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class SquishyButton : MonoBehaviour {
    private RectTransform rectTransform;
    [SerializeField] private Button button;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnButtonPress() {
        ButtonPressAsync().Forget();
    }

    private async UniTask ButtonPressAsync() {
        await LMotion.Create(1.0f, 0.9f, 0.1f)
            .WithEase(Ease.OutQuad)
            .Bind(it => rectTransform.localScale = Vector3.one * it);
        LMotion.Create(0.9f, 1.0f, 0.1f)
            .WithEase(Ease.InQuad)
            .Bind(it => rectTransform.localScale = Vector3.one * it).ToUniTask().Forget();
    }
}
