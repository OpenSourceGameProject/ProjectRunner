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
        Debug.Log("게임 종료 버튼이 클릭되었습니다.");

        // 1. 실제 빌드된 게임(.exe 파일 등)이 완전히 꺼지는 코드
        Application.Quit();

        // 2. [추가] 유니티 에디터에서도 재생(▶) 버튼이 탁 꺼지게 만드는 코드
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}