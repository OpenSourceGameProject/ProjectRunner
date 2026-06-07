using UnityEngine;
using UnityEngine.SceneManagement;


// 메인 메뉴의 UI 상호작용, 인게임 씬 전환 및 플랫폼별 게임 종료를 제어하는 class입니다.
public class MainMenuController : MonoBehaviour
{
    public void OnClickPlay()
    {
        SceneManager.LoadScene("GameScene");
    }

    // 종료 버튼 클릭 시 호출되며 플랫폼 환경에 맞춰 게임을 종료합니다.
    public void OnClickQuit()
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