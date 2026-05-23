using UnityEngine;
using UnityEngine.UI;

public class UI_Hearts : MonoBehaviour
{
    // 어디서나 이 하트 시스템에 접근할 수 있게 만드는 싱글톤(치트키) 설정
    public static UI_Hearts Instance { get; private set; }

    // 유니티 인스펙터에서 꽂아줄 하트 이미지 5개 배열
    public Image[] hearts;

    // 현재 남아있는 하트 개수 (처음엔 5개로 시작)
    private int currentHearts;

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentHearts = hearts.Length; // 하트 개수를 5개로 세팅
    }

    // 플레이어가 장애물에 부딪히면 호출될 함수 (하트 하나씩 끄기)
    public void TakeDamage()
    {
        if (currentHearts > 0)
        {
            currentHearts--; // 하트 개수 1 감소

            // 감소된 인덱스에 있는 하트 오브젝트를 화면에서 비활성화(끄기)
            if (hearts[currentHearts] != null)
            {
                hearts[currentHearts].gameObject.SetActive(false);
            }

            Debug.Log($"💔 하트가 감소했습니다! 남은 하트: {currentHearts}");

            if (currentHearts <= 0)
            {
                Debug.Log("💀 GAME OVER! 목숨을 모두 잃었습니다.");
                // 게임 오버 창 띄우는 코드 넣기
            }
        }
    }
}