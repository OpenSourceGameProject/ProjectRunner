using UnityEngine;
using UnityEngine.InputSystem;

public class SquareTrackRunning : MonoBehaviour
{
    [Header("이동 및 차선 설정")]
    public float runSpeed = 15f;
    public float laneSwitchSpeed = 10f;
    public float laneWidth = 1.5f;

    [Header("점프 및 슬라이드 설정")]
    public float jumpForce = 6f;
    public float gravity = -15f;
    public float slideDuration = 1.0f;

    // 인스펙터에서 넣을 필요가 없으므로 private으로 변경하여 숨깁니다.
    private Transform[] waypoints = new Transform[4];

    private int currentLane = 1;
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

        // [추가된 코드] 프리팹을 위해 씬에서 Waypoint1 ~ 4를 이름으로 직접 찾습니다.
        for (int i = 0; i < 4; i++)
        {
            // Waypoint1, Waypoint2, Waypoint3, Waypoint4 이름을 차례대로 검색
            string waypointName = "Waypoint" + (i + 1);
            GameObject wp = GameObject.Find(waypointName);

            if (wp != null)
            {
                waypoints[i] = wp.transform;
            }
            else
            {
                Debug.LogError("씬에서 '" + waypointName + "' 오브젝트를 찾을 수 없습니다! 하이어라키 창의 이름을 확인해주세요.");
            }
        }

        // 웨이포인트를 하나라도 못 찾았다면 작동을 중지합니다.
        if (waypoints[0] == null || waypoints[1] == null || waypoints[2] == null || waypoints[3] == null)
        {
            return;
        }

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
        // 웨이포인트가 없으면 에러 방지를 위해 업데이트를 넘깁니다.
        if (waypoints[0] == null || waypoints[1] == null || waypoints[2] == null || waypoints[3] == null) return;

        HandleInput();
        HandleSlideTimer();
        MovePlayer();
    }

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