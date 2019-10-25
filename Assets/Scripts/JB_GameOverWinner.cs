using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JB_GameOverWinner : MonoBehaviour
{
    public TextMeshProUGUI winnerDisplayText;
    public string winnerName;

    private void OnLevelWasLoaded(int level)
    {
        winnerName = PlayerPrefs.GetString("Winner");
        winnerDisplayText.text = winnerName;

    }
}
