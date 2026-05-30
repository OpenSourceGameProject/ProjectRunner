using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위한 코드

public class PauseMenuController : MonoBehaviour
{
    // 유니티 인스펙터에서 PausePanel을 연결할 구멍
    public GameObject pausePanel;

    // 유니티 인스펙터에서 GameOverPanel을 연결할 구멍
    public GameObject gameOverPanel;

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
    }

    // 게임 오버 창에서 [다시 시작] 버튼 클릭 시 실행
    public void ClickRestartButton()
    {
        Time.timeScale = 1f;           // 멈춘 시간을 다시 정상으로 풀기

        // 현재 열려있는 씬(UI_MergedScene)의 이름을 가져와서 처음부터 다시 로드!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}