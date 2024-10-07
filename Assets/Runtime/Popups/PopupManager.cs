using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FriendManager;
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
    [Space, SerializeField] private GameObject heartParticlePrefab;

    private HashSet<int> existingPopups = new();

    private async UniTask Start()
    {
        await RandomDelay(new Vector2(3f, 5f));

        friendManager.SelectNewRandomFriends();

        SpawnHeartParticlesLoop().Forget();

        // Manually spawn a first popup
        var popup = SpawnNewPopup();
        await popup.FadeIn();
        await RandomDelay(popupLifetimeRange);
        await popup.FadeOut();

        SpawnPopupsLoop().Forget();
    }

    public Popup SpawnNewPopup()
    {
        var randomFriendIdx = 0;
        do
        {
            randomFriendIdx = Random.Range(0, Mathf.Min(friendManager.FriendCount, friendManager.RandomFriends.Length));
        } while (!existingPopups.Add(randomFriendIdx));

        var newPopup = Instantiate(popupPrefab, popupParent);
        newPopup.Initialize(friendManager, randomFriendIdx);
        return newPopup;
    }

    private async UniTask SpawnPopupsLoop() {
        while (true) {
            await SpawnPopupsAsync();
        }
    }

    private async UniTask SpawnHeartParticlesLoop() {
        while (true) {
            await SpawnHeartParticlesAsync();
        }
    }

    private async UniTask SpawnPopupsAsync()
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

    private async UniTask SpawnHeartParticlesAsync() {
        for (int i = 0; i < Mathf.Min(friendManager.FriendCount, friendManager.RandomFriends.Length); i++) {
            Friend friend = friendManager.RandomFriends[i];
            if (Utils.FloatDist(friend.position.x, Input.mousePosition.x) <= 100
                && Utils.FloatDist(friend.position.y, Screen.height - Input.mousePosition.y) <= 100) {
                HeartParticleLifetimeInternal(friend).Forget();
            }
        }

        await RandomDelay(new Vector2(1.5f, 4.5f));
    }

    private async UniTask HeartParticleLifetimeInternal(Friend friend) {
        GameObject instance = Instantiate(heartParticlePrefab, popupParent);
        HeartParticle heartParticle = instance.GetComponent<HeartParticle>();
        var friendPosition = friend.position;
        friendPosition.y = Screen.height - friendPosition.y;
        heartParticle.transform.position = friendPosition + new Vector2(0.0f, 32.0f);
        await heartParticle.FadeAnimation();
    }

    // TODO: this should maybe be an extension method
    private UniTask RandomDelay(Vector2 range) => UniTask.Delay(TimeSpan.FromSeconds(Random.Range(range.x, range.y)));
}
