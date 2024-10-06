using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourcePrefab;
    [SerializeField] private FriendManager friendManager;

    [Space, SerializeField] private AudioClip[] happyClips;
    [SerializeField] private AudioClip[] neutralAudioClips;
    [SerializeField] private AudioClip[] unhappyClips;

    private async UniTask Start()
    {
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0.5f, 5f)));

            var randomFriendIdx = Random.Range(0, friendManager.RandomFriends.Length);
            var randomFriend = friendManager.RandomFriends[randomFriendIdx];

            var position = randomFriend.position;
            var positionNormalized = position / new Vector2(Screen.width, Screen.height);
            var positionCentered = positionNormalized - new Vector2(0.5f, 0.5f);
            positionCentered.y *= -1;
            var newPosition = 10 * positionCentered;

            var mood = (int)randomFriend.mood;
            var clipLibrary = mood switch
            {
                >= 64 => happyClips,
                <= -64 => unhappyClips,
                _ => neutralAudioClips
            };

            var newAudioSource = Instantiate(audioSourcePrefab, transform.parent);
            newAudioSource.clip = clipLibrary[Random.Range(0, clipLibrary.Length)];
            newAudioSource.transform.localPosition = new Vector3(newPosition.x, newPosition.y, 0);
            newAudioSource.Play();

            await UniTask.WaitWhile(() => newAudioSource.isPlaying);

            Destroy(newAudioSource.gameObject);
        }
    }
}
