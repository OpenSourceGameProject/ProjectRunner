using UnityEngine;
using UnityEngine.InputSystem;

public class SquareTrackRunning : MonoBehaviour
{
    [Header("시작 위치 및 차선 설정 (겹침 방지)")]
    public float startDistanceOffset = 0f;
    [Range(0, 2)]
    public int startingLane = 1;

    [Header("이동 및 차선 설정")]
    public float runSpeed = 15f;
    public float laneSwitchSpeed = 10f;
    public float laneWidth = 1.5f;

    // --- [새로 추가된 속도 증가 시스템] ---
    [Header("자동 속도 증가 시스템")]
    [Tooltip("몇 초마다 속도를 증가시킬지 설정합니다. (현재 30초)")]
    public float speedIncreaseInterval = 30f;

    [Tooltip("지정된 시간이 될 때마다 증가할 속도량입니다.")]
    public float speedIncreaseAmount = 2f;

    [Tooltip("도달할 수 있는 최대 속도 제한입니다. (물리 버그 방지)")]
    public float maxSpeed = 35f;

    private float speedTimer = 0f; // 시간을 잴 내부 타이머
    // -------------------------------------

    [Header("점프 및 슬라이드 설정")]
    public float jumpForce = 6f;
    public float gravity = -15f;
    public float slideDuration = 1.0f;

    private Transform[] waypoints = new Transform[4];

    private int currentLane;
    private float currentLaneOffset = 0f;
    private float distanceAlongPerimeter = 0f;
    private Animator animator;

    private float verticalVelocity = 0f;
    private float baseY;
    private bool isJumping = false;
    private bool isSliding = false;
    private float slideTimer = 0f;

    private Vector3[] corners = new Vector3[4];
    private float[] sideLengths = new float[4];
    private float totalPerimeter = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();

        currentLane = startingLane;
        currentLaneOffset = (currentLane - 1) * laneWidth;
        distanceAlongPerimeter = startDistanceOffset;

        for (int i = 0; i < 4; i++)
        {
            string waypointName = "Waypoint" + (i + 1);
            GameObject wp = GameObject.Find(waypointName);

            if (wp != null)
            {
                waypoints[i] = wp.transform;
            }
            else
            {
                Debug.LogError("씬에서 '" + waypointName + "' 오브젝트를 찾을 수 없습니다!");
            }
        }

        if (waypoints[0] == null || waypoints[1] == null || waypoints[2] == null || waypoints[3] == null) return;

        baseY = transform.position.y;

        for (int i = 0; i < 4; i++)
        {
            if (waypoints[i] != null)
            {
                corners[i] = new Vector3(waypoints[i].position.x, baseY, waypoints[i].position.z);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            int nextIndex = (i + 1) % 4;
            sideLengths[i] = Vector3.Distance(corners[i], corners[nextIndex]);
            totalPerimeter += sideLengths[i];
        }
    }

    void Update()
    {
        if (waypoints[0] == null || waypoints[1] == null || waypoints[2] == null || waypoints[3] == null) return;

        HandleInput();
        HandleSlideTimer();
        HandleSpeedIncrease(); // [새로 추가된 함수 호출] 매 프레임 시간을 체크합니다.
        MovePlayer();
    }

    // --- [새로 추가된 함수] 속도 증가 타이머 계산 ---
    private void HandleSpeedIncrease()
    {
        // 최대 속도에 도달했다면 더 이상 타이머를 계산하지 않습니다.
        if (runSpeed >= maxSpeed) return;

        speedTimer += Time.deltaTime; // 매 프레임마다 실제 흐른 시간을 누적합니다.

        // 누적된 시간이 우리가 설정한 간격(예: 30초)을 넘었다면
        if (speedTimer >= speedIncreaseInterval)
        {
            speedTimer = 0f; // 타이머를 다시 0으로 초기화
            runSpeed += speedIncreaseAmount; // 속도 증가!

            // 만약 증가한 속도가 최대 속도를 넘었다면, 최대 속도로 고정
            runSpeed = Mathf.Min(runSpeed, maxSpeed);

            // 유니티 콘솔창에서 속도가 잘 오르는지 확인하기 위한 로그 (필요 없으면 지우셔도 됩니다)
            Debug.Log("속도 증가! 현재 속도: " + runSpeed);
        }
    }
    // ------------------------------------------------

    private void HandleInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            currentLane--;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            currentLane++;
        }
        currentLane = Mathf.Clamp(currentLane, 0, 2);

        if (Keyboard.current.upArrowKey.wasPressedThisFrame && !isJumping && !isSliding)
        {
            isJumping = true;
            verticalVelocity = jumpForce;
            if (animator != null) animator.SetTrigger("Jump");
        }

        if (Keyboard.current.downArrowKey.wasPressedThisFrame && !isJumping && !isSliding)
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
        distanceAlongPerimeter += runSpeed * Time.deltaTime;
        distanceAlongPerimeter %= totalPerimeter;

        float dist = distanceAlongPerimeter;
        int currentSide = 0;

        for (int i = 0; i < 4; i++)
        {
            if (dist < sideLengths[i])
            {
                currentSide = i;
                break;
            }
            dist -= sideLengths[i];
        }

        int nextSide = (currentSide + 1) % 4;
        float t = dist / sideLengths[currentSide];

        Vector3 basePos = Vector3.Lerp(corners[currentSide], corners[nextSide], t);

        Vector3 forwardDir = (corners[nextSide] - corners[currentSide]).normalized;
        Vector3 localRightDir = Quaternion.LookRotation(forwardDir) * Vector3.right;

        float targetLaneOffset = (currentLane - 1) * laneWidth;
        currentLaneOffset = Mathf.Lerp(currentLaneOffset, targetLaneOffset, Time.deltaTime * laneSwitchSpeed);

        if (isJumping)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        float newY = transform.position.y + verticalVelocity * Time.deltaTime;

        if (newY <= baseY)
        {
            if (isJumping && animator != null) animator.SetTrigger("Land");
            newY = baseY;
            isJumping = false;
            verticalVelocity = 0f;
        }

        Vector3 finalPosition = basePos + (localRightDir * currentLaneOffset);
        finalPosition.y = newY;
        transform.position = finalPosition;

        if (forwardDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forwardDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }

        if (animator != null)
        {
            animator.SetBool("IsRunning", true);
        }
    }
}