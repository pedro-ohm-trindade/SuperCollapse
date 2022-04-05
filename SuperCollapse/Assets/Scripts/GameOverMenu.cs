using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOverMenu : MonoBehaviour
{
    public TextMeshProUGUI message;
    public TextMeshProUGUI score;

    private void Start() {
        score.text += Scoring.getScore();
        if (Scoring.isWin()) {
            message.text = "You Win!";
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}