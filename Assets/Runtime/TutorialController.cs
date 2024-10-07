using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Serializable]
    public struct TutorialSegment
    {
        public string Text;
        public float Delay;
        public int TargetID;
        public Vector2 TargetLerp;
    }

    [SerializeField] private TutorialSegment[] segments;
    [SerializeField] private FriendManager friendManager;
    [SerializeField] private RectTransform[] targets;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private CanvasGroup canvasGroup;

    private RectTransform rectTransform;

    private int segmentIdx = 0;

    private async UniTask Start()
    {
        rectTransform = transform as RectTransform;

        LMotion.Create(0f, 1f, 1f)
            .WithEase(Ease.InOutSine)
            .BindToCanvasGroupAlpha(canvasGroup)
            .ToUniTask()
            .Forget();

        for (var i = 0; i < segments.Length; i++)
        {
            segmentIdx = i;

            var segment = segments[i];

            tutorialText.text = segment.Text;
            tutorialText.color = new Color(0, 0, 0, 0);

            var preferredSize = new Vector2(tutorialText.preferredWidth, tutorialText.preferredHeight);
            var currentSize = rectTransform.sizeDelta;

            await LMotion.Create(0f, 1f, 1f)
                .WithEase(Ease.OutCubic)
                .Bind(t => rectTransform.sizeDelta = Vector2.Lerp(currentSize, preferredSize, t));

            await LMotion.Create(0f, 1f, 1f)
                .WithEase(Ease.OutSine)
                .Bind(t => tutorialText.color = new Color(0, 0, 0, t));

            await UniTask.Delay(TimeSpan.FromSeconds(segment.Delay));

            await LMotion.Create(1f, 0f, 1f)
                .WithEase(Ease.OutSine)
                .Bind(t => tutorialText.color = new Color(0, 0, 0, t));
        }

        var finalSize = rectTransform.sizeDelta;
        
        LMotion.Create(0f, 1f, 1f)
                .WithEase(Ease.InCubic)
                .Bind(t => rectTransform.sizeDelta = Vector2.Lerp(finalSize, Vector2.zero, t))
                .ToUniTask()
                .Forget();

        LMotion.Create(1f, 0f, 1f)
            .WithEase(Ease.InOutSine)
            .BindToCanvasGroupAlpha(canvasGroup)
            .ToUniTask()
            .Forget();
    }

    private void LateUpdate()
    {
        var segment = segments[segmentIdx];

        var targetPosition = segment.TargetID switch
        {
            0 => friendManager.FriendCount > 0
                ? new Vector2(friendManager.RandomFriends[0].position.x, Screen.height - friendManager.RandomFriends[0].position.y)
                : 0.5f * new Vector2(Screen.width, Screen.height),
            //_ => targets[segment.TargetID - 1].anchoredPosition
            _ => (Vector2)targets[segment.TargetID - 1].position
        };

        targetPosition = new Vector2(
            Mathf.Lerp(Screen.width / 2f, targetPosition.x, segment.TargetLerp.x),
            Mathf.Lerp(Screen.height / 2f, targetPosition.y, segment.TargetLerp.y));

        rectTransform.anchoredPosition = Utils.TemporalLerp(rectTransform.anchoredPosition, targetPosition, 0.1f);
    }
}
