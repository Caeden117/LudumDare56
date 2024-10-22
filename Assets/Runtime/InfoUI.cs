using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoUI : MonoBehaviour {
    private CanvasGroup canvasGroup;
    [SerializeField]
    private FriendManager friendManager;
    [SerializeField]
    private GoldManager goldManager;
    [SerializeField]
    private TMP_Text friendCountText;
    [SerializeField]
    private List<RectTransform> moodBars;
    [SerializeField]
    private List<TMP_Text> moodValueLabels;

    private void Start() {
        canvasGroup = GetComponent<CanvasGroup>();

        for (int i = 0; i < moodValueLabels.Count; i++) {
            if (i >= goldManager.moodGoldValues.Count) {
                break;
            }

            moodValueLabels[i].text = "+" + goldManager.moodGoldValues[i].ToString();
        }
    }

    private void Update() {
        if (canvasGroup.alpha <= 0.0f) {
            return;
        }

        if (friendManager.FriendCount == 1) {
            friendCountText.text = "1 FRIEND!";
        } else {
            friendCountText.text = $"{friendManager.FriendCount:N0} FRIENDS!";
        }

        int mostPopularMood = 0;
        for (int i = 0; i < friendManager.MoodStats.Length; i++) {
            mostPopularMood = Mathf.Max(mostPopularMood, friendManager.MoodStats[i]);
        }

        if (mostPopularMood > 0) {
            for (int i = 0; i < moodBars.Count; i++) {
                if (i >= friendManager.MoodStats.Length) {
                    break;
                }

                float moodPortion = ((float) friendManager.MoodStats[i]) / mostPopularMood;
                float currentWidth = moodBars[i].sizeDelta.x;
                float targetWidth = moodPortion * 150.0f;
                float newWidth = Utils.TemporalLerp(currentWidth, targetWidth, 0.1f);
                moodBars[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            }
        }
    }
}
