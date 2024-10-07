using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

public class NextProgressionUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GoldManager goldManager;
    [SerializeField] private TextMeshProUGUI text;

    public void Show()
        => LMotion.Create(0f, 1f, 1f)
            .WithEase(Ease.InOutSine)
            .BindToCanvasGroupAlpha(canvasGroup);

    public void Hide()
        => LMotion.Create(1f, 0f, 1f)
            .WithEase(Ease.InOutSine)
            .BindToCanvasGroupAlpha(canvasGroup);

    public void UpdateCost(decimal cost)
        => text.text = $"{goldManager.FormatGold(cost)}<sprite=0 tint=1>";

    private void Update()
        => transform.eulerAngles = Mathf.Sin(Time.timeSinceLevelLoad)* 5f * Vector3.forward;
}
