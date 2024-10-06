using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Popup : MonoBehaviour
{
    enum MoodCategory {
        UNSET,
        UNHAPPY,
        SLIGHTLY_UNHAPPY,
        SLIGHTLY_HAPPY,
        HAPPY
    };

    [SerializeField] private CanvasGroup canvasGroup;

    [Space, SerializeField] private Image moodImage;
    [SerializeField] private Image outlineImage;
    [SerializeField] private Sprite[] happySprites;
    [SerializeField] private Color happyColor;
    [SerializeField] private Sprite[] slightlyHappySprites;
    [SerializeField] private Color slightlyHappyColor;
    [SerializeField] private Sprite[] slightlyUnhappySprites;
    [SerializeField] private Color slightlyUnhappyColor;
    [SerializeField] private Sprite[] unhappySprites;
    [SerializeField] private Color unhappyColor;

    private FriendManager friendManager;
    private int randomFriendIdx;

    private RectTransform parent;
    private RectTransform rectTransform;
    private new Camera camera;

    private Vector2 idleOffset = Vector2.zero;
    private Vector2 bounceOffset = Vector2.zero;

    private MoodCategory moodCategory = MoodCategory.UNSET;
    private MoodCategory lastMoodCategory = MoodCategory.UNSET;
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

        lastMoodCategory = MoodCategory.UNSET;
        moodCategory = MoodCategory.UNSET;
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
        while (!cancellationToken.IsCancellationRequested && moodCategory == MoodCategory.HAPPY)
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
        while (!cancellationToken.IsCancellationRequested && moodCategory == MoodCategory.UNHAPPY)
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

    private void LateUpdate() {
        var friend = friendManager.RandomFriends[randomFriendIdx];
        var mood = friend.mood - 127;
        moodCategory = mood switch {
            >= 64 => MoodCategory.HAPPY,
            >= 0 => MoodCategory.SLIGHTLY_HAPPY,
            >= -64 => MoodCategory.SLIGHTLY_UNHAPPY,
            < -64 => MoodCategory.UNHAPPY
        };

        if (moodCategory != lastMoodCategory) {
            var moodSprites = moodCategory switch {
                MoodCategory.HAPPY => happySprites,
                MoodCategory.SLIGHTLY_HAPPY => slightlyHappySprites,
                MoodCategory.SLIGHTLY_UNHAPPY => slightlyUnhappySprites,
                MoodCategory.UNHAPPY => unhappySprites,
                _ => null
            };
            moodImage.sprite = moodSprites[Random.Range(0, moodSprites.Length)];

            var moodColor = moodCategory switch {
                MoodCategory.HAPPY => happyColor,
                MoodCategory.SLIGHTLY_HAPPY => slightlyHappyColor,
                MoodCategory.SLIGHTLY_UNHAPPY => slightlyUnhappyColor,
                MoodCategory.UNHAPPY => unhappyColor,
                _ => Color.black
            };
            outlineImage.color = moodColor;

            if (moodCategory == MoodCategory.HAPPY) {
                HappyBounce().Forget();
            }
            else if (moodCategory == MoodCategory.UNHAPPY) {
                SadBounce().Forget();
            }

            lastMoodCategory = moodCategory;
        }

        var friendPosition = friend.position;
        friendPosition.y = Screen.height - friendPosition.y;

        rectTransform.anchoredPosition = friendPosition + idleOffset + bounceOffset + new Vector2(0.5f, 0.5f);
    }
}
