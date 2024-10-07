using UnityEngine;

public class TinyFriendToggle : MonoBehaviour
{
    [SerializeField] private FriendManager friendManager;

    public void UpdateToggle(bool value) => friendManager.Tiny = value;
}
