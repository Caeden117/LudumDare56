using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        audioMixer.GetFloat("Volume", out var vol);
        slider.SetValueWithoutNotify(Utils.DecibelsToLinear(vol));
    }

    public void OnChangeVolume(float vol) => audioMixer.SetFloat("Volume", Utils.LinearToDecibels(vol));
}
