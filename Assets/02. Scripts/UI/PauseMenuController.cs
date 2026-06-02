using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI highScoreText;

    public void ClickPauseButton()
    {
        pausePanel.SetActive(true); 
        Time.timeScale = 0f;
    }

    public void ClickResumeButton()
    {
        pausePanel.SetActive(false); 
        Time.timeScale = 1f; 
    }
    public void ClickHomeButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true); 
        Time.timeScale = 0f; 

        int currentScore = 0;
        if (UI_Score.Instance != null)
        {
            currentScore = UI_Score.Instance.GetScore();
        }

        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > savedHighScore)
        {
            savedHighScore = currentScore;
            PlayerPrefs.SetInt("HighScore", savedHighScore);
            PlayerPrefs.Save();
        }

        if (highScoreText != null)
        {
            highScoreText.text = "Best Score : " + savedHighScore;
        }
    }

    public void ClickRestartButton()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ClickQuitButton()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}