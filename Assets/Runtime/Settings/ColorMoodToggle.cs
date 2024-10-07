using UnityEngine;

public class ColorMoodToggle : MonoBehaviour
{
    [SerializeField] private FriendManager friendManager;

    public void UpdateToggle(bool value) => friendManager.DebugDrawMood = value;
}
