using UnityEngine;
using UnityEngine.InputSystem;

public class CircularRunnerController : MonoBehaviour
{
    [Header("트랙 설정")]
    public Transform trackCenter;
    public float[] trackRadii = { 8f, 10f, 12f };
    public int startingTrackIndex = 1;

    //mergescenecheckcolor에서 변경
    [Header("트랙 색상 설정")]
    public TrackColorType[] trackColors =
    {
        TrackColorType.Red,
        TrackColorType.Green,
        TrackColorType.Blue
    };

    [Header("이동 속도")]
    public float runSpeed = 60f;
    public float laneSwitchSpeed = 10f;

    [Header("점프 및 슬라이드 설정")]
    public float jumpForce = 6f;
    public float gravity = -15f;

    // [추가된 코드] 슬라이드 지속 시간 (인스펙터에서 조절 가능)
    [Tooltip("슬라이드 애니메이션이 유지되는 시간(초)입니다.")]
    public float slideDuration = 1.0f;

    //mergescenecheckcolor에서 변경
    public int CurrentTrackIndex => currentTrackIndex;
    //mergescenecheckcolor에서 변경
    public TrackColorType CurrentTrackColor
    {
        get
        {
            if (trackColors == null || trackColors.Length == 0)
                return TrackColorType.Green;

            int index = Mathf.Clamp(currentTrackIndex, 0, trackColors.Length - 1);
            return trackColors[index];
        }
    }

    private int currentTrackIndex;
    private float currentRadius;
    private float currentAngle = 0f;
    private Animator animator;

    // 상태 관리 변수
    private float verticalVelocity = 0f;
    private float baseY;
    private bool isJumping = false;

    // [추가된 코드] 슬라이드 상태 및 타이머 변수
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
            // 하이어라키 창에 설정하신 이름인 "CircleCenter"로 정확하게 찾아옵니다.
            GameObject centerObject = GameObject.Find("CircleCenter");

            if (centerObject != null)
            {
                trackCenter = centerObject.transform;
            }
            else
            {
                Debug.LogError("씬에서 'CircleCenter'라는 이름의 오브젝트를 찾을 수 없습니다! 오타가 없는지 확인해주세요.");
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
        HandleSlideTimer(); // [추가된 코드] 슬라이드 시간을 계산하는 함수 호출
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

        // 2. 점프 (W 키) - 점프 중이거나 슬라이드 중일 때는 뛰지 못하게 막음
        if (Keyboard.current.wKey.wasPressedThisFrame && !isJumping && !isSliding)
        {
            isJumping = true;
            verticalVelocity = jumpForce;

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }

        // 3. [추가된 코드] 슬라이드 (S 키) - 땅에 있고 슬라이드 중이 아닐 때만 작동
        if (Keyboard.current.sKey.wasPressedThisFrame && !isJumping && !isSliding)
        {
            isSliding = true;                   // 슬라이드 상태 켜기
            slideTimer = slideDuration;         // 타이머에 지속 시간 충전

            // 애니메이터에 설정한 Slide 트리거 방아쇠를 당깁니다.
            if (animator != null)
            {
                animator.SetTrigger("Slide");
            }
        }
    }

    // [추가된 코드] 시간이 흐르면 슬라이드 상태를 풀어주는 함수
    private void HandleSlideTimer()
    {
        if (isSliding)
        {
            slideTimer -= Time.deltaTime; // 매 프레임마다 시간을 깎음

            if (slideTimer <= 0)
            {
                isSliding = false; // 시간이 다 되면 슬라이드 상태 해제
            }
        }
    }

    private void MovePlayer()
    {
        currentAngle += runSpeed * Time.deltaTime;
        currentRadius = Mathf.Lerp(currentRadius, trackRadii[currentTrackIndex], Time.deltaTime * laneSwitchSpeed);

        if (isJumping)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        float newY = transform.position.y + verticalVelocity * Time.deltaTime;

        if (newY <= baseY)
        {
            if (isJumping)
            {
                if (animator != null)
                {
                    animator.SetTrigger("Land");
                }
            }

            newY = baseY;
            isJumping = false;
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