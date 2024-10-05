using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private FriendManager friendManager;

    // I use vector types for better editor experience
    [Space, SerializeField] private Vector2 delayRange;
    [SerializeField] private Vector2Int popupNumberRange;
    [SerializeField] private Vector2 popupSpawnRange;
    [SerializeField] private Vector2 popupLifetimeRange;

    [Space, SerializeField] private Popup popupPrefab;
    [SerializeField] private RectTransform popupParent;

    private async UniTask Start()
    {
        while (true)
        {
            await LoopAsync();
        }
    }

    public Popup SpawnNewPopup()
    {
        var newPopup = Instantiate(popupPrefab, popupParent);
        newPopup.Initialize(friendManager, Random.Range(0, friendManager.RandomFriends.Length));
        return newPopup;
    }

    private async UniTask LoopAsync()
    {
        await RandomDelay(delayRange);

        friendManager.SelectNewRandomFriends();

        var numPopups = Random.Range(popupNumberRange.x, popupNumberRange.y);
        
        // Construct multiple popup tasks
        var tasks = new UniTask[numPopups];
        for (var i = 0; i < numPopups; i++)
        {
            tasks[i] = PopupLifetimeInternal();
        }

        await UniTask.WhenAll(tasks);
    }

    private async UniTask PopupLifetimeInternal()
    {
        var popup = SpawnNewPopup();
        await RandomDelay(popupSpawnRange);
        await popup.FadeIn();
        await RandomDelay(popupLifetimeRange);
        await popup.FadeOut();
    }

    // TODO: this should maybe be an extension method
    private UniTask RandomDelay(Vector2 range) => UniTask.Delay(TimeSpan.FromSeconds(Random.Range(range.x, range.y)));
}
