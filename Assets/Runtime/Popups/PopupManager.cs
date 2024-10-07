using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
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

    private HashSet<int> existingPopups = new();

    private async UniTask Start()
    {
        await RandomDelay(new Vector2(3f, 5f));

        // Manually spawn a first popup
        var popup = SpawnNewPopup();
        await popup.FadeIn();
        await RandomDelay(popupLifetimeRange);
        await popup.FadeOut();

        while (true)
        {
            await LoopAsync();
        }
    }

    public Popup SpawnNewPopup()
    {
        var randomFriendIdx = 0;
        do
        {
            randomFriendIdx = Random.Range(0, friendManager.RandomFriends.Length);
        } while (!existingPopups.Add(randomFriendIdx));

        var newPopup = Instantiate(popupPrefab, popupParent);
        newPopup.Initialize(friendManager, randomFriendIdx);
        return newPopup;
    }

    private async UniTask LoopAsync()
    {
        await RandomDelay(delayRange);

        existingPopups.Clear();
        friendManager.SelectNewRandomFriends();

        var numPopups = Mathf.Min(Random.Range(popupNumberRange.x, popupNumberRange.y), friendManager.FriendCount);
        
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
