using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeSceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button playButton;

    private int score, highScore;

    private void Start()
    {
        playButton.onClick.AddListener(LoadScene);
        if (GamePlayAttemptStat.gamePlayAttempted && PlayerPrefs.HasKey("Score"))
        {
            score = PlayerPrefs.GetInt("Score");
            if (PlayerPrefs.HasKey("HighScore"))
            {
                highScore = PlayerPrefs.GetInt("HighScore");
                if (score > highScore)
                {
                    highScore = score;
                    PlayerPrefs.SetInt("HighScore", highScore);
                }
            }
            else
            {
                highScore = score;
                PlayerPrefs.SetInt("HighScore", score);
            }

            scoreText.gameObject.SetActive(true);
            scoreText.text = "Score: " + score.ToString();
            PlayerPrefs.Save();

        }
        else if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
            scoreText.gameObject.SetActive(false);
        }
        else
        {
            highScore = 0;
            scoreText.gameObject.SetActive(false);
        }
        highScoreText.gameObject.SetActive(true);
        highScoreText.text = "HighScore: " + highScore.ToString();
    }
    private void LoadScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
