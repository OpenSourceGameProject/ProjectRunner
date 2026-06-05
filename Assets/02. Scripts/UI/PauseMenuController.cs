using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

// 인게임 내 일시정지, 게임오버, 점수 데이터 관리를 제어하는 class입니다.
public class PauseMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    [Header("Score UI")]
    public TextMeshProUGUI highScoreText; // 최고 점수를 표시할 TMP 텍스트입니다.

    private void Update()
    {
        // 게임오버 패널이 활성화된 상태라면 키보드 조작(ESC)을 완전히 차단합니다.
        if (gameOverPanel != null && gameOverPanel.activeSelf)
        {
            return;
        }

        // ESC 키 입력 시 일시정지 토글을 처리합니다.
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pausePanel.activeSelf)
            {
                ClickResumeButton();
            }
            else
            {
                ClickPauseButton();
            }
        }
    }

    // 일시정지 버튼 클릭 또는 ESC 입력 시 호출되며 게임을 일시정지합니다.
    public void ClickPauseButton()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // 계속하기 버튼 클릭 또는 ESC 입력 시 호출되며 게임을 재개합니다.
    public void ClickResumeButton()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // 홈(메인 메뉴) 버튼 클릭 시 호출되며 시작 화면으로 이동합니다.
    public void ClickHomeButton()
    {
        // 빌드 환경 예외 처리: 시간 축을 먼저 완벽히 복구한 뒤 정상 흐름에서 씬을 전환합니다.
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    // 플레이어 사망 시 호출되며 게임오버 처리 및 최고 점수를 갱신합니다.
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        int currentScore = 0;

        // 점수 매니저 싱글톤 인스턴스가 존재할 경우에 현재 판의 최종 점수를 획득합니다.
        if (UI_Score.Instance != null)
        {
            currentScore = UI_Score.Instance.GetScore();
        }

        // PlayerPrefs 로컬 스토리지에서 기존 최고 점수를 로드합니다.
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);

        // 현재 점수가 기존 최고 점수를 초과했을 경우, 신기록을 저장합니다.
        if (currentScore > savedHighScore)
        {
            savedHighScore = currentScore;
            PlayerPrefs.SetInt("HighScore", savedHighScore);
            PlayerPrefs.Save();
        }


        // 최종 최고 점수를 UI 텍스트에 컴포넌트에 반영합니다.
        if (highScoreText != null)
        {
            highScoreText.text = "Best Score : " + savedHighScore;
        }
    }

    // 다시 시작 버튼 클릭 시 호출되며 현재 인게임 씬을 처음부터 재로드합니다.
    public void ClickRestartButton()
    {
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    // 종료 버튼 클릭 시 호출되며 플랫폼 환경에 맞춰 게임을 종료합니다.
    public void ClickQuitButton()
    {
        // 빌드된 게임 환경에서 앱을 종료합니다.
        Application.Quit();

        // 유니티 에디터에서 실행한 경우의 예외 처리
        #if UNITY_EDITOR
        // 에디터의 재생 모드를 강제로 비활성화하여 종료 상태로 바꿉니다.
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

    }

}