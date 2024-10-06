using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoUI : MonoBehaviour {
    [SerializeField]
    private FriendManager friendManager;
    [SerializeField]
    private TMP_Text friendCountText;
    [SerializeField]
    private List<RectTransform> moodBars;

    private void Update() {
        if (friendManager.FriendCount == 1) {
            friendCountText.text = "1 FRIEND!";
        } else {
            friendCountText.text = friendManager.FriendCount.ToString("N0") + " FRIENDS!";
        }

        int mostPopularMood = friendManager.MoodStats.Max();

        if (mostPopularMood > 0) {
            for (int i = 0; i < moodBars.Count; i++) {
                if (i >= friendManager.MoodStats.Length) {
                    break;
                }

                float moodPortion = ((float) friendManager.MoodStats[i]) / mostPopularMood;
                float currentWidth = moodBars[i].sizeDelta.x;
                float targetWidth = moodPortion * 230.0f;
                float newWidth = Mathf.Lerp(currentWidth, targetWidth, 1.0f - Mathf.Pow(1.0f - (0.1f), Time.deltaTime * 60.0f));
                moodBars[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            }
        }
    }
}
