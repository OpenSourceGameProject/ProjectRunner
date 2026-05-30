using UnityEngine;
using UnityEngine.InputSystem;

public class CircularRunnerControl : MonoBehaviour
{
    [Header("트랙 설정")]
    public Transform trackCenter;
    public float[] trackRadii = { 8f, 10f, 12f };
    public int startingTrackIndex = 1;

    [Header("이동 속도")]
    public float runSpeed = 60f;
    public float laneSwitchSpeed = 10f;

    // --- [새로 추가된 속도 증가 시스템] ---
    [Header("자동 속도 증가 시스템")]
    [Tooltip("몇 초마다 속도를 증가시킬지 설정합니다. (현재 30초)")]
    public float speedIncreaseInterval = 30f;

    [Tooltip("지정된 시간이 될 때마다 증가할 속도량입니다.")]
    public float speedIncreaseAmount = 5f;

    [Tooltip("도달할 수 있는 최대 속도 제한입니다. (원형 트랙 기준)")]
    public float maxSpeed = 120f;

    private float speedTimer = 0f; // 시간을 잴 내부 타이머
    // -------------------------------------

    [Header("점프 및 슬라이드 설정")]
    public float jumpForce = 6f;
    public float gravity = -15f;
    public float slideDuration = 1.0f;

    private int currentTrackIndex;
    private float currentRadius;
    private float currentAngle = 0f;
    private Animator animator;

    // 상태 관리 변수
    private float verticalVelocity = 0f;
    private float baseY;
    private bool isJumping = false;
    private bool isSliding = false;
    private float slideTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentTrackIndex = Mathf.Clamp(startingTrackIndex, 0, trackRadii.Length - 1);
        currentRadius = trackRadii[currentTrackIndex];
        baseY = transform.position.y;

        // 인스펙터 창에서 trackCenter가 연결되어 있지 않다면(None), 씬에서 이름으로 자동 검색합니다.
        if (trackCenter == null)
        {
            GameObject centerObject = GameObject.Find("CircleCenter");
            if (centerObject != null)
            {
                trackCenter = centerObject.transform;
            }
            else
            {
                Debug.LogError("씬에서 'CircleCenter'라는 이름의 오브젝트를 찾을 수 없습니다!");
            }
        }

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
        HandleSlideTimer();
        HandleSpeedIncrease(); // [새로 추가된 함수 호출] 매 프레임 시간을 체크합니다.
        MovePlayer();
    }

    // --- [새로 추가된 함수] 원형 트랙 속도 증가 타이머 계산 ---
    private void HandleSpeedIncrease()
    {
        // 최대 속도에 도달했다면 더 이상 타이머를 계산하지 않습니다.
        if (runSpeed >= maxSpeed) return;

        speedTimer += Time.deltaTime; // 매 프레임마다 실제 흐른 시간을 누적합니다.

        // 누적된 시간이 설정한 간격(예: 30초)을 넘었다면
        if (speedTimer >= speedIncreaseInterval)
        {
            speedTimer = 0f; // 타이머 초기화
            runSpeed += speedIncreaseAmount; // 속도 증가!

            // 증가한 속도가 최대 속도를 넘지 않도록 제한
            runSpeed = Mathf.Min(runSpeed, maxSpeed);

            Debug.Log("원형 트랙 캐릭터 속도 증가! 현재 속도: " + runSpeed);
        }
    }
    // --------------------------------------------------------

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

        // 2. 점프 (W 키)
        if (Keyboard.current.wKey.wasPressedThisFrame && !isJumping && !isSliding)
        {
            isJumping = true;
            verticalVelocity = jumpForce;
            if (animator != null) animator.SetTrigger("Jump");
        }

        // 3. 슬라이드 (S 키)
        if (Keyboard.current.sKey.wasPressedThisFrame && !isJumping && !isSliding)
        {
            isSliding = true;
            slideTimer = slideDuration;
            if (animator != null) animator.SetTrigger("Slide");
        }
    }

    private void HandleSlideTimer()
    {
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                isSliding = false;
            }
        }
    }

    private void MovePlayer()
    {
        currentAngle += runSpeed * Time.deltaTime;
        currentRadius = Mathf.Lerp(currentRadius, trackRadii[currentTrackIndex], Time.deltaTime * laneSwitchSpeed);

        // Y축 부양 방지 철통 방어 로직 적용
        float newY = baseY;

        if (isJumping)
        {
            verticalVelocity += gravity * Time.deltaTime;
            newY = transform.position.y + verticalVelocity * Time.deltaTime;

            if (newY <= baseY)
            {
                if (animator != null) animator.SetTrigger("Land");
                newY = baseY;
                isJumping = false;
                verticalVelocity = 0f;
            }
        }
        else
        {
            newY = baseY;
            verticalVelocity = 0f;
        }

        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 newPosition = new Vector3(
            trackCenter.position.x + Mathf.Cos(rad) * currentRadius,
            newY,
            trackCenter.position.z + Mathf.Sin(rad) * currentRadius
        );

        transform.position = newPosition;

        Vector3 moveDirection = new Vector3(-Mathf.Sin(rad), 0, Mathf.Cos(rad));
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        if (animator != null)
        {
            animator.SetBool("IsRunning", true);
        }
    }
}