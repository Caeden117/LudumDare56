using UnityEngine;

public class FreeUpgradesToggle : MonoBehaviour
{
    [SerializeField] private TimerProgression timerProgression;

    public void UpdateToggle(bool value) => timerProgression.DebugFreeUpgrade = value;
}
