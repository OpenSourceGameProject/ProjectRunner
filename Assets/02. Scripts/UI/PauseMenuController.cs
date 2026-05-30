using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위한 코드

public class PauseMenuController : MonoBehaviour
{
    // 유니티 인스펙터에서 PausePanel을 연결할 구멍
    public GameObject pausePanel;

    // [일시정지] 버튼 클릭 시 실행
    public void ClickPauseButton()
    {
        pausePanel.SetActive(true); // 팝업창 켜기
        Time.timeScale = 0f;        // 게임 시간을 0으로 만들어 멈춤
    }

    // [계속하기] 버튼 클릭 시 실행
    public void ClickResumeButton()
    {
        pausePanel.SetActive(false); // 팝업창 끄기
        Time.timeScale = 1f;         // 게임 시간을 다시 1(정상)로 돌림
    }

    // [홈으로] 버튼 클릭 시 실행
    public void ClickHomeButton()
    {
        Time.timeScale = 1f;         // 중요: 멈춘 시간을 풀고 이동해야 다음 게임이 안 멈춤!
        SceneManager.LoadScene("StartScene"); // 아까 만든 시작 화면 씬 이름
    }
}