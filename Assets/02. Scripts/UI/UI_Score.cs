using UnityEngine;
using TMPro; // Score : 0 텍스트를 제어합니다.

// 인게임 점수 누적, 아이템 점수 추가, 스코어 UI 텍스트 출력을 제어하는 class입니다.
public class UI_Score : MonoBehaviour
{
    // 다른 스크립트에서 UI_Score.Instance로 바로 접근할 수 있게 하는 싱글톤입니다.
    public static UI_Score Instance { get; private set; }

    [Header("텍스트 오브젝트 연결")]
    public TextMeshProUGUI scoreText;

    [Header("설정")]
    public float scoreSpeed = 10f;    // 초당 점수가 오르는 속도입니다.

    private float currentScore = 0f;
    private bool isGameActive = true;  // 게임 오버 시 점수를 멈추기 위한 플래그입니다.

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 인스펙터에서 텍스트 컴포넌트를 연결하지 않았을 경우를 대비한 자동 예외 처리입니다.
        if (scoreText == null)
        {
            scoreText = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        // 게임이 진행 중일 때만 점수가 오르게 합니다.
        if (isGameActive)
        {
            // 매 프레임마다 시간에 비례해서 점수를 누적합니다.
            currentScore += Time.deltaTime * scoreSpeed;

            // 화면에 갱신합니다.
            UpdateScoreText();
        }
    }

    // 점수를 화면에 그리는 함수입니다.
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score : " + Mathf.FloorToInt(currentScore).ToString();
        }
    }

    // 아이템 획득 등 외부 요인에 의해 추가 점수를 가산할 때 외부에서 호출하는 함수입니다.
    public void AddScore(int amount)
    {
        if (isGameActive)
        {
            currentScore += amount;
            UpdateScoreText();
        }
    }

    // 플레이어 사망 시 호출되어 점수 자동 누적을 중단시키는 함수입니다.
    public void StopScore()
    {
        isGameActive = false;
    }

    // 다른 스크립트에서 현재까지 획득한 최종 점수를 정수형으로 반환받기 위한 함수입니다.
    public int GetScore()
    {
        return Mathf.FloorToInt(currentScore);
    }
}