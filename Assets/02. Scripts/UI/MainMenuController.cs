using UnityEngine;
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� �ڵ�

public class MainMenuController : MonoBehaviour
{
    public void OnClickPlay()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickQuit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}