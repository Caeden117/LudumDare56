using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [SerializeField]
    private GoldManager goldManager;
    [SerializeField]
    private TMP_Text goldLabel;

    private decimal smoothedGold = 0m;

    private void Update() {
        smoothedGold = Utils.TemporalLerp(smoothedGold, goldManager.Gold, 0.1);
        goldLabel.text = goldManager.FormatGold(smoothedGold);
    }
}
