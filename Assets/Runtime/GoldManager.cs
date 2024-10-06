using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoldManager : MonoBehaviour {
    [SerializeField]
    private FriendManager friendManager;

    public decimal Gold { get; set; } = 0;

    [SerializeField]
    private float goldCooldown = 1.0f;
    private float goldTimer = 0.0f;

    [SerializeField]
    public List<long> moodGoldValues;

    private void Update() {
        goldTimer -= Time.deltaTime;
        if (goldTimer <= 0.0f) {
            goldTimer += goldCooldown;

            int mostPopularMood = friendManager.MoodStats.Max();

            if (mostPopularMood > 0) {
                for (int i = 0; i < moodGoldValues.Count; i++) {
                    if (i >= friendManager.MoodStats.Length) {
                        break;
                    }

                    Gold += moodGoldValues[i] * friendManager.MoodStats[i];
                }
            }
        }
    }

    public string FormatGold(decimal gold) {
        decimal whole = Math.Floor(gold);

        // No Math.Log10 for decimal types, this'll do for our purposes
        int log10 = 0;
        decimal temp = whole;
        while (temp >= 10) {
            temp /= 10;
            ++log10;
        }

        // I forget how much precision decimal has exactly but this should definitely cover it
        string suffix = log10 switch {
            <= 2 => "",
            <= 5 => "K",
            <= 8 => "M",
            <= 11 => "B",
            <= 14 => "T",
            <= 17 => "Qa",
            <= 20 => "Qi",
            <= 23 => "Sx",
            <= 26 => "Sp",
            <= 29 => "O",
            <= 32 => "U",
            _ => "?"
        };

        string str = whole.ToString();
        if (str.Length < 4) {
            return str;
        }

        str = str.Substring(0, 4);
        int decimalPos = 1 + log10 % 3;

        return str.Substring(0, decimalPos) + "." + str.Substring(decimalPos, str.Length - decimalPos) + suffix;
    }
}