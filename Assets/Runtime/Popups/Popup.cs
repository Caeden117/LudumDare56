using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private FriendManager friendManager;
    private int randomFriendIdx;

    private RectTransform parent;
    private RectTransform rectTransform;
    private new Camera camera;

    public void Initialize(FriendManager friendManager, int randomFriendIdx)
    {
        this.friendManager = friendManager;
        this.randomFriendIdx = randomFriendIdx;
    
        parent = transform.parent as RectTransform;
        rectTransform = transform as RectTransform;
        camera = Camera.main;

        gameObject.SetActive(false);
    }

    public async UniTask FadeIn()
    {
        gameObject.SetActive(true);
        await LMotion.Create(0f, 1f, 1f).Bind((a) => canvasGroup.alpha = a).ToUniTask();
    }

    public async UniTask FadeOut()
    {
        await LMotion.Create(1f, 0f, 1f).Bind((a) => canvasGroup.alpha = a).ToUniTask();
        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        var friendPosition = friendManager.RandomFriends[randomFriendIdx].position;
        friendPosition.y = Screen.height - friendPosition.y;

        rectTransform.anchoredPosition = friendPosition;
    }
}
