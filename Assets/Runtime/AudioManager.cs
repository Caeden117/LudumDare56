using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourcePrefab;
    [SerializeField] private FriendManager friendManager;
    [SerializeField] private Vector2 meowDelayRange;
    [SerializeField] private float meowTimeVariance;
    [SerializeField] private float meowPitchVariance;

    [Space, SerializeField] private AudioClip[] happyClips;
    [SerializeField] private AudioClip[] neutralAudioClips;
    [SerializeField] private AudioClip[] unhappyClips;

    private async UniTask Start()
    {
        await UniTask.WaitUntil(() => friendManager.FriendCount > 0);

        while (true)
        {
            // uncomment for constant delay
            //var delay = Random.Range(0.5f, 5f);

            // Mathematical function: 1 - (log(x) / 7)
            var delayNormalized = 1 - (Mathf.Log(friendManager.FriendCount, 10) / 7f);
            var delayRemapped = Utils.MapRange(delayNormalized, 0, 1, meowDelayRange.y, meowDelayRange.x);
            var delayRandomized = Mathf.Max(delayRemapped + (Random.Range(-meowTimeVariance, meowTimeVariance) * delayRemapped), meowDelayRange.y);
            await UniTask.Delay(TimeSpan.FromSeconds(delayRandomized));

            var randomFriendIdx = Random.Range(0, friendManager.RandomFriends.Length);
            var randomFriend = friendManager.RandomFriends[randomFriendIdx];

            var position = randomFriend.position;
            var positionNormalized = position / new Vector2(Screen.width, Screen.height);
            var positionCentered = positionNormalized - new Vector2(0.5f, 0.5f);
            positionCentered.y *= -1;
            var newPosition = 10 * positionCentered;

            var mood = randomFriend.mood - 127;
            var clipLibrary = mood switch
            {
                >= 64 => happyClips,
                <= -64 => unhappyClips,
                _ => neutralAudioClips
            };

            var newAudioSource = Instantiate(audioSourcePrefab, transform.parent);
            newAudioSource.clip = clipLibrary[Random.Range(0, clipLibrary.Length)];
            newAudioSource.transform.localPosition = (Vector3)newPosition;
            newAudioSource.pitch = 1 + Random.Range(-meowPitchVariance, meowPitchVariance);
            newAudioSource.Play();

            WaitThenDestroy(newAudioSource).Forget();
        }
    }

    private async UniTask WaitThenDestroy(AudioSource source)
    {
        await UniTask.WaitWhile(() => source.isPlaying);

        Destroy(source.gameObject);
    }
}
