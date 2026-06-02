using UnityEngine;
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� �ڵ�

public class MainMenuController : MonoBehaviour
{
    public void OnClickPlay()
    {
        SceneManager.LoadScene("UI_MergedScene");
    }

    public void OnClickQuit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}