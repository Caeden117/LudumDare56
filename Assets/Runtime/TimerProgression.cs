using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class TimerProgression : MonoBehaviour {
    [SerializeField]
    private float minimumUnlockDuration = 5.0f;

    [SerializeField]
    private bool debugFreeUpgrade;

    private RectTransform rectTransform;
    [SerializeField] private FriendManager friendManager;
    [SerializeField] private GoldManager goldManager;
    [SerializeField] private Sprite maxSprite;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progressionImage;
    [SerializeField] private RectTransform buttonRect;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;

    private bool cooldown = false;

    private decimal[] costSteps = {
        10m, 50m, 250m,
        1000m, 2000m, 5000m,
        10000m, 20000m, 50000m,
        100000m, 200000m, 500000m,
        1000000m, 2000000m, 5000000m,
        10000000m, 20000000m, 50000000m,
        100000000m, 200000000m, 500000000m,
        1000000000m
    };

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup.alpha = 0.0f;
        TimerAsync(minimumUnlockDuration).Forget();
    }

    private void Update() {
        button.interactable = !cooldown
            && (debugFreeUpgrade || goldManager.Gold >= costSteps[friendManager.FriendCountStep])
            && friendManager.FriendCountStep < costSteps.Length - 1;

        var currentFill = (float)(goldManager.Gold / costSteps[friendManager.FriendCountStep]);
        progressionImage.fillAmount = Utils.TemporalLerp(progressionImage.fillAmount, currentFill, 0.1f);
    }

    private async UniTask TimerAsync(float duration) {
        cooldown = true;

        // Animate a reset (0 progress, faded)
        LMotion.Create(1f, 0f, 0.5f)
            .WithEase(Ease.OutQuad)
            .Bind(it => progressionImage.fillAmount = it)
            .ToUniTask()
            .Forget();
        await LMotion.Create(canvasGroup.alpha, 0.5f, 1f).BindToCanvasGroupAlpha(canvasGroup);

        // Wait until conditions are met
        //await LMotion.Create(0f, 1f, duration).Bind(it => progressionImage.fillAmount = it);
        while (goldManager.Gold < costSteps[friendManager.FriendCountStep] && !debugFreeUpgrade)
        {
            await UniTask.Yield();
        }

        // Speen
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

        cooldown = false;
    }

    public void OnButtonPress() {
        ButtonPressAsync().Forget();
        if (!debugFreeUpgrade) {
            goldManager.Gold -= costSteps[friendManager.FriendCountStep];
        }
        float duration = friendManager.AddNewFriends();
        if (duration > 0.0f) {
            TimerAsync(Mathf.Max(minimumUnlockDuration, duration)).Forget();
        }
        else {
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
