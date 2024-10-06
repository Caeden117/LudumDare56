using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class TimerProgression : MonoBehaviour
{
    private const float PROGRESSION_TIMER_IN_MINUTES = 0.1f;

    private RectTransform rectTransform;
    [SerializeField] private FriendManager friendManager;
    [SerializeField] private Sprite maxSprite;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progressionImage;
    [SerializeField] private RectTransform buttonRect;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup.alpha = 0.0f;
        TimerAsync().Forget();
    }

    private async UniTask TimerAsync()
    {
        LMotion.Create(1f, 0f, 0.5f)
            .WithEase(Ease.OutQuad)
            .Bind(it => progressionImage.fillAmount = it)
            .ToUniTask()
            .Forget();

        await LMotion.Create(canvasGroup.alpha, 0.5f, 1f).BindToCanvasGroupAlpha(canvasGroup);

        await LMotion.Create(0f, 1f, PROGRESSION_TIMER_IN_MINUTES * 60).Bind(it => progressionImage.fillAmount = it);

        button.interactable = true;

        LMotion.Create(0f, 360f, 1.5f)
            .WithEase(Ease.OutCirc)
            .Bind(it => buttonRect.eulerAngles = it * Vector3.forward)
            .ToUniTask()
            .Forget();
        LMotion.Create(0.5f, 1f, 1.5f)
            .WithEase(Ease.OutCirc)
            .BindToCanvasGroupAlpha(canvasGroup)
            .ToUniTask()
            .Forget();
    }

    public void OnButtonPress()
    {
        button.interactable = false;
        ButtonPressAsync().Forget();
        if (friendManager.AddNewFriends()) {
            TimerAsync().Forget();
        } else {
            buttonImage.sprite = maxSprite;
        }
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
