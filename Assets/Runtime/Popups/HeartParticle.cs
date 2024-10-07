using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HeartParticle : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Image image;

    private float timeOffset = 0.0f;

    private void Start() {
        timeOffset = Random.Range(0.0f, 10.0f);
    }

    public async UniTask FadeAnimation() {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
        await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0.0f, 0.25f)));
        LMotion.Create(0f, 1.0f, 2.0f)
            .WithEase(Ease.OutQuad)
            .Bind(it => image.transform.localPosition = new Vector3(0.0f, it * 64.0f, 0.0f))
            .ToUniTask()
            .Forget();
        await LMotion.Create(0.0f, 1.0f, 0.25f).BindToCanvasGroupAlpha(canvasGroup);
        await LMotion.Create(1.0f, 0.0f, 1.75f).BindToCanvasGroupAlpha(canvasGroup);
        Destroy(gameObject);
    }

    private void Update()
    {
        image.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 15.0f * Mathf.Sin((Time.time + timeOffset) * Mathf.PI));
    }
}
