using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [SerializeField]
    private GoldManager goldManager;
    [SerializeField]
    private TMP_Text goldLabel;

    private void Update() {
        goldLabel.text = goldManager.FormatGold(goldManager.Gold);
    }
}
