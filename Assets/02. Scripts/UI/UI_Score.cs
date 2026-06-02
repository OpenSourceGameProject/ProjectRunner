using UnityEngine;
using TMPro; // Score: 0 텍스트를 제어

public class UI_Score : MonoBehaviour
{
    // 다른 스크립트(플레이어나 아이템)에서 UI_Score.Instance로 바로 접근할 수 있게 하는 싱글톤
    public static UI_Score Instance { get; private set; }

    [Header("텍스트 오브젝트 연결")]
    public TextMeshProUGUI scoreText;

    [Header("설정")]
    public float scoreSpeed = 10f;    // 초당 점수가 오르는 속도

    private float currentScore = 0f;
    private bool isGameActive = true;  // 게임 오버 시 점수를 멈추기 위한 플래그

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
        if (scoreText == null)
        {
            scoreText = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        // 게임이 진행 중일 때만 점수가 오르게 함
        if (isGameActive)
        {
            // 매 프레임마다 시간에 비례해서 점수 누적
            currentScore += Time.deltaTime * scoreSpeed;

            // 화면에 갱신
            UpdateScoreText();
        }
    }

    // 점수를 화면에 그리는 함수
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score : " + Mathf.FloorToInt(currentScore).ToString();
        }
    }
    public void AddScore(int amount)
    {
        if (isGameActive)
        {
            currentScore += amount;
            UpdateScoreText();
        }
    }
    public void StopScore()
    {
        isGameActive = false;
    }

    // [추가] 최고 점수 기능 -> 에러처리
    public int GetScore()
    {
        return Mathf.FloorToInt(currentScore);
    }
}