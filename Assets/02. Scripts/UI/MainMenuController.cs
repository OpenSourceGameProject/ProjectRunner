using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위한 코드

public class MainMenuController : MonoBehaviour
{
    // [BlackPlayButton] 누르면 실행될 함수
    public void OnClickPlay()
    {
        // 이동하고 싶은 본 게임 씬
        SceneManager.LoadScene("UI_MergedScene");
    }

    // [BlackQuitButton] 누르면 실행될 함수
    public void OnClickQuit()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 테스트할 때 잘 눌렸는지 콘솔창에 띄워줍니다.
        Debug.Log("게임 종료 버튼이 클릭되었습니다.");
#else
        // 실제 게임을 빌드해서 출시했을 때 게임이 꺼집니다.
        Application.Quit();
#endif
    }
}