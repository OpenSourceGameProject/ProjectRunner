using UnityEngine;
using TMPro;

public class ColorLaneJudgeSystem : MonoBehaviour
{
    [Header("References")]
    public CircularRunnerController player;
    public BossMaterialAura bossAura;

    [Header("Judge Settings")]
    [Tooltip("색이 맞지 않을 때 몇 초까지 기다려줄지")]
    public float graceTime = 2f;

    [Tooltip("한 번 피격된 뒤 다시 판정하기 전 대기 시간")]
    public float damageCooldown = 1.5f;

    [Header("UI")]
    public TMP_Text judgeText;

    private float wrongLaneTimer = 0f;
    private float cooldownTimer = 0f;

    private TrackColorType lastBossColor;
    private bool initialized = false;

    // 로그가 매 프레임 반복 출력되는 것을 막기 위한 상태값
    private bool wasCorrectLane = false;
    private bool wasWrongLane = false;

    private void Start()
    {
        if (player == null)
            player = FindAnyObjectByType<CircularRunnerController>();

        if (bossAura == null)
            bossAura = FindAnyObjectByType<BossMaterialAura>();

        if (bossAura != null)
        {
            lastBossColor = bossAura.CurrentColor;
            initialized = true;
        }

        RefreshJudgeText("색상 판정 준비");

        Debug.Log("색상 레인 판정 시스템 시작");
    }

    private void Update()
    {
        if (player == null || bossAura == null)
            return;

        if (!initialized)
        {
            lastBossColor = bossAura.CurrentColor;
            initialized = true;
        }

        cooldownTimer -= Time.deltaTime;

        TrackColorType playerColor = player.CurrentTrackColor;
        TrackColorType targetColor = bossAura.CurrentColor;

        // 보스 색이 바뀌면 새 색에 맞춰 이동할 시간을 다시 줌
        if (targetColor != lastBossColor)
        {
            lastBossColor = targetColor;
            wrongLaneTimer = 0f;

            wasCorrectLane = false;
            wasWrongLane = false;

            RefreshJudgeText("보스 색 변경! " + targetColor + " 레인으로 이동");

            Debug.Log("보스 색 변경: " + targetColor + " / 2초 안에 해당 레인으로 이동해야 합니다.");

            return;
        }

        bool isCorrectLane = playerColor == targetColor;

        if (isCorrectLane)
        {
            wrongLaneTimer = 0f;

            RefreshJudgeText("색상 일치 : " + playerColor);

            // 성공 상태로 처음 들어왔을 때만 로그 출력
            if (!wasCorrectLane)
            {
                Debug.Log(
                    "색상 판정 성공! 플레이어 레인: " +
                    playerColor +
                    " / 보스 색상: " +
                    targetColor
                );
            }

            wasCorrectLane = true;
            wasWrongLane = false;

            return;
        }

        wrongLaneTimer += Time.deltaTime;

        float remainTime = Mathf.Max(0f, graceTime - wrongLaneTimer);

        RefreshJudgeText(
            "색상 불일치! " +
            targetColor +
            " 레인으로 이동하세요 / 남은 시간: " +
            remainTime.ToString("0.0") +
            "초"
        );

        // 실패 상태로 처음 들어왔을 때만 로그 출력
        if (!wasWrongLane)
        {
            Debug.Log(
                "색상 판정 대기 중. 현재 플레이어 레인: " +
                playerColor +
                " / 목표 보스 색상: " +
                targetColor +
                " / 제한 시간: " +
                graceTime +
                "초"
            );
        }

        wasCorrectLane = false;
        wasWrongLane = true;

        // 2초 안에 못 들어오면 하트 감소
        if (wrongLaneTimer >= graceTime && cooldownTimer <= 0f)
        {
            TakeColorDamage(playerColor, targetColor);

            // 계속 틀린 레인에 있으면 바로 연속으로 깎이지 않게 다시 유예시간 부여
            wrongLaneTimer = 0f;
            cooldownTimer = damageCooldown;

            // 다시 실패 대기 로그가 뜰 수 있게 초기화
            wasWrongLane = false;
        }
    }

    private void TakeColorDamage(TrackColorType playerColor, TrackColorType targetColor)
    {
        if (UI_Hearts.Instance != null)
        {
            UI_Hearts.Instance.TakeDamage();

            Debug.Log(
                "색상 판정 실패! 하트 1 감소 / 플레이어 레인: " +
                playerColor +
                " / 목표 보스 색상: " +
                targetColor
            );
        }
        else
        {
            Debug.LogWarning("UI_Hearts.Instance가 없습니다. HealthUI의 UI_Hearts 오브젝트가 켜져 있는지 확인하세요.");
        }
    }

    private void RefreshJudgeText(string message)
    {
        if (judgeText != null)
        {
            judgeText.text = message;
        }
    }
}