using UnityEngine;
using UnityEngine.UI;

public class UI_Hearts : MonoBehaviour
{
    // 어디서나 이 하트 시스템에 접근할 수 있게 만드는 싱글톤을 설정합니다.
    public static UI_Hearts Instance { get; private set; }

    // 유니티 인스펙터에서 꽂아줄 하트 이미지 5개를 배열합니다.
    public Image[] hearts;

    // 현재 남아있는 하트 개수입니다.
    private int currentHearts;

    private void Awake()
    {
        // 싱글톤을 초기화시킵니다.
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentHearts = hearts.Length; // 하트 개수를 5개로 세팅합니다.
    }

    // 플레이어가 장애물에 부딪히면 호출될 함수입니다.
    public void TakeDamage()
    {
        if (currentHearts > 0)
        {
            currentHearts--;

            // 감소된 인덱스에 있는 하트 오브젝트를 화면에서 끕니다.
            if (hearts[currentHearts] != null)
            {
                hearts[currentHearts].gameObject.SetActive(false);
            }
            if (currentHearts <= 0)
            {
                PauseMenuController pauseManager = FindAnyObjectByType<PauseMenuController>();
                if (pauseManager != null)
                {
                    pauseManager.ShowGameOver();
                }

                // 시간에 따른 스코어 자동 증가를 멈춥니다.
                if (UI_Score.Instance != null)
                {
                    UI_Score.Instance.StopScore();
                }

                // 유니티 전체 시간을 0으로 만들어서 게임의 물리/스폰/움직임을 모두 멈춥니다.
                Time.timeScale = 0f;
            }
        }
    }
}