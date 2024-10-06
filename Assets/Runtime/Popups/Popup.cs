using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class Popup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private FriendManager friendManager;
    private int randomFriendIdx;

    private RectTransform parent;
    private RectTransform rectTransform;
    private new Camera camera;

    private Vector2 idleOffset = Vector2.zero;
    private Vector2 bounceOffset = Vector2.zero;

    private CancellationToken cancellationToken;

    public void Initialize(FriendManager friendManager, int randomFriendIdx)
    {
        this.friendManager = friendManager;
        this.randomFriendIdx = randomFriendIdx;
    
        parent = transform.parent as RectTransform;
        rectTransform = transform as RectTransform;
        camera = Camera.main;

        cancellationToken = this.GetCancellationTokenOnDestroy();

        gameObject.SetActive(false);
        IdleBounce().Forget();
    }

    public async UniTask FadeIn()
    {
        gameObject.SetActive(true);
        await LMotion.Create(0f, 1f, 1f).BindToCanvasGroupAlpha(canvasGroup).ToUniTask(cancellationToken);
    }

    public async UniTask FadeOut()
    {
        await LMotion.Create(1f, 0f, 1f).BindToCanvasGroupAlpha(canvasGroup).ToUniTask(cancellationToken);
        Destroy(gameObject);
    }

    public async UniTask IdleBounce()
        => await LMotion.Create(-2f, 2f, 1f / 2)
            .WithEase(Ease.InOutSine)
            .WithLoops(int.MaxValue, LoopType.Yoyo)
            .Bind(it => idleOffset.y = it)
            .ToUniTask(cancellationToken);

    // TODO: Hook to positive mood
    public async UniTask HappyBounce()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0, 3)), cancellationToken: cancellationToken);

            await LMotion.Create(0, 25f, 1f / 5)
                .WithEase(Ease.OutBack)
                .Bind(it => bounceOffset.y = it)
                .ToUniTask(cancellationToken);
            
            await LMotion.Create(25f, 0f, 3f / 4)
                .WithEase(Ease.OutBounce)
                .Bind(it => bounceOffset.y = it)
                .ToUniTask(cancellationToken);
        }
    }

    // TODO: Hook to negative mood
    public async UniTask SadBounce()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0, 3)), cancellationToken: cancellationToken);

            await LMotion.Create(0f, 10f, 1f / 5f)
                .WithEase(Ease.OutCirc)
                .Bind(it => bounceOffset.x = Mathf.Sin(Time.timeSinceLevelLoad * 10) * it)
                .ToUniTask(cancellationToken);

            await LMotion.Create(10f, 0f, 1.5f)
                .WithEase(Ease.OutCirc)
                .Bind(it => bounceOffset.x = Mathf.Sin(Time.timeSinceLevelLoad * 10) * it)
                .ToUniTask(cancellationToken);
        }
    }

    private void LateUpdate()
    {
        var friendPosition = friendManager.RandomFriends[randomFriendIdx].position;
        friendPosition.y = Screen.height - friendPosition.y;

        rectTransform.anchoredPosition = friendPosition + idleOffset + bounceOffset;
    }
}
