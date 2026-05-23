using UnityEngine;
using UnityEngine.InputSystem; // 최신 Input System 사용

public class CircularRunnerController : MonoBehaviour
{
    [Header("트랙 설정")]
    public Transform trackCenter;
    public float[] trackRadii = { 8f, 10f, 12f };
    public int startingTrackIndex = 1;

    [Header("이동 속도")]
    public float runSpeed = 60f;
    public float laneSwitchSpeed = 10f;

    [Header("점프 설정")]
    [Tooltip("점프할 때 위로 솟구치는 힘의 크기입니다.")]
    public float jumpForce = 6f;

    [Tooltip("캐릭터를 아래로 잡아당기는 중력의 크기입니다. (음수값)")]
    public float gravity = -15f;

    private int currentTrackIndex;
    private float currentRadius;
    private float currentAngle = 0f;
    private Animator animator;

    // 점프 상태 관리 변수
    private float verticalVelocity = 0f; // Y축 이동 속도
    private float baseY;                 // 바닥의 기본 높이
    private bool isJumping = false;      // 현재 점프 중인지 여부

    void Start()
    {
        animator = GetComponent<Animator>();
        currentTrackIndex = Mathf.Clamp(startingTrackIndex, 0, trackRadii.Length - 1);
        currentRadius = trackRadii[currentTrackIndex];

        // 게임 시작 시점의 캐릭터 Y축 높이를 기준 바닥 높이로 저장합니다.
        baseY = transform.position.y;

        if (trackCenter != null)
        {
            Vector3 dir = transform.position - trackCenter.position;
            currentAngle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        }
    }

    void Update()
    {
        if (trackCenter == null) return;

        HandleInput();
        MovePlayer();
    }

    private void HandleInput()
    {
        if (Keyboard.current == null) return;

        // 1. 좌우 트랙 이동 (A, D 키)
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            currentTrackIndex--;
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            currentTrackIndex++;
        }
        currentTrackIndex = Mathf.Clamp(currentTrackIndex, 0, trackRadii.Length - 1);

        // 2. 점프 기능 (W 키) - 이미 점프 중이 아닐 때만 도약 가능
        if (Keyboard.current.wKey.wasPressedThisFrame && !isJumping)
        {
            isJumping = true;
            verticalVelocity = jumpForce; // 위 방향으로 순간적인 힘을 부여

            // 애니메이터에 설정한 Jump 트리거 방아쇠를 당깁니다. (Forward -> Begin 이동)
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    private void MovePlayer()
    {
        // 원형 궤도 및 반지름 계산
        currentAngle += runSpeed * Time.deltaTime;
        currentRadius = Mathf.Lerp(currentRadius, trackRadii[currentTrackIndex], Time.deltaTime * laneSwitchSpeed);

        // 점프 중일 때 중력을 지속적으로 반영하여 verticalVelocity를 감소시킴 (상승하다가 하강하도록)
        if (isJumping)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // 현재 속도를 반영한 새로운 Y축 높이 계산
        float newY = transform.position.y + verticalVelocity * Time.deltaTime;

        // 바닥 착지 판정
        if (newY <= baseY)
        {
            // 공중에 떠 있다가 방금 막 바닥에 닿은 타이밍인 경우
            if (isJumping)
            {
                // 애니메이터에 설정한 Land 트리거 방아쇠를 당깁니다. (Begin -> Land 이동)
                if (animator != null)
                {
                    animator.SetTrigger("Land");
                }
            }

            newY = baseY;          // 캐릭터가 땅 밑으로 파묻히지 않게 고정
            isJumping = false;     // 점프 상태 해제
            verticalVelocity = 0f; // 수직 속도 초기화
        }

        // 삼각함수를 이용한 최종 X, Y, Z 좌표 계산 및 적용
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 newPosition = new Vector3(
            trackCenter.position.x + Mathf.Cos(rad) * currentRadius,
            newY, // 계산된 점프 높이 적용
            trackCenter.position.z + Mathf.Sin(rad) * currentRadius
        );

        transform.position = newPosition;

        // 캐릭터가 달리는 방향을 바라보도록 회전 처리
        Vector3 moveDirection = new Vector3(-Mathf.Sin(rad), 0, Mathf.Cos(rad));
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        // 기본 달리기 애니메이션 상태 유지
        if (animator != null)
        {
            animator.SetBool("IsRunning", true);
        }
    }
}