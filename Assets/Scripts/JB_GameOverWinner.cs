using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class JB_GameOverWinner : MonoBehaviour
{
    public TextMeshProUGUI winnerDisplayText;
    private string winnerName;

    private void OnLevelWasLoaded(int level)
    {
        winnerName = PlayerPrefs.GetString("Winner");
        winnerDisplayText.text = winnerName + " wins!";

    }

    public void LoadMyScene(int i)
    {
        SceneManager.LoadScene(i);
    }
}
