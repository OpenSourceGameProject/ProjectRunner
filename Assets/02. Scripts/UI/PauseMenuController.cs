using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위한 코드
using UnityEngine.UI; // [추가] 최고 기록을 위한 텍스트 UI 제어 코드
using TMPro; // [추가] 최고 기록 텍스트매시프로 제어를 위해 추가

public class PauseMenuController : MonoBehaviour
{
    // 유니티 인스펙터에서 PausePanel을 연결할 구멍
    public GameObject pausePanel;

    // 유니티 인스펙터에서 GameOverPanel을 연결할 구멍
    public GameObject gameOverPanel;

    // [추가] 유니티 인스펙터에서 HighScoreText를 연결할 구멍
    public TextMeshProUGUI highScoreText;

    // 일시정지 버튼 클릭 시 실행
    public void ClickPauseButton()
    {
        pausePanel.SetActive(true); // 팝업창 켜기
        Time.timeScale = 0f;        // 게임 시간을 0으로 만들어 멈춤
    }

    // 계속하기 버튼 클릭 시 실행
    public void ClickResumeButton()
    {
        pausePanel.SetActive(false); // 팝업창 끄기
        Time.timeScale = 1f;         // 게임 시간을 다시 1(정상)로 돌림
    }

    // 홈 버튼 클릭 시 실행
    public void ClickHomeButton()
    {
        Time.timeScale = 1f;         // 중요: 멈춘 시간을 풀고 이동해야 다음 게임이 안 멈춤!
        SceneManager.LoadScene("StartScene"); // 아까 만든 시작 화면 씬 이름
    }

    // 캐릭터가 죽었을 때 게임 오버 창을 띄우는 함수
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true); // 게임 오버 창 켜기
        Time.timeScale = 0f;           // 게임 멈추기


        // [추가] - 최고 점수 로직
        // 1. 현재 판에서 얻은 점수 가져오기
        int currentScore = 0;
        if (UI_Score.Instance != null)
        {
            currentScore = UI_Score.Instance.GetScore();
        }

        // 2. 저장되어 있던 기존 최고 점수 불러오기 (저장된 게 없으면 0)
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);

        // 3. 만약 이번 점수가 기존 최고 점수보다 높다면 신기록 저장
        if (currentScore > savedHighScore)
        {
            savedHighScore = currentScore;
            PlayerPrefs.SetInt("HighScore", savedHighScore);
            PlayerPrefs.Save(); // 하드디스크에 데이터 완전 박제
        }

        // 4. UI 텍스트에 최고 점수 반영하기
        if (highScoreText != null)
        {
            highScoreText.text = "Best Score : " + savedHighScore;
        }
    }

    // 게임 오버 창에서 [다시 시작] 버튼 클릭 시 실행
    public void ClickRestartButton()
    {
        Time.timeScale = 1f;           // 멈춘 시간을 다시 정상으로 풀기

        // 현재 열려있는 씬(UI_MergedScene)의 이름을 가져와서 처음부터 다시 로드!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 시작 화면에서 Quit 버튼을 눌렀을 때 작동하는 함수
    public void ClickQuitButton()
    {
        Debug.Log("게임 종료 버튼 클릭됨!");

        // 1. 실제 빌드된 게임(PC, 모바일 등)에서 완전 종료시키는 코드
        Application.Quit();

        // 2. [추가 꿀팁] 유니티 에디터 창에서 재생(▶) 버튼을 자동으로 꺼지게 만드는 코드
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}