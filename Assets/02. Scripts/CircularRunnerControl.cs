using UnityEngine;
using UnityEngine.InputSystem;
public class CircularRunnerControl : MonoBehaviour
{
    public Transform trackCenter;
    public float[] trackRadii = { 5f, 8f, 11f };
    public int startingTrackIndex = 1;
    public float runSpeed = 60f;
    public float laneSwitchSpeed = 10f;

    private int currentTrackIndex;
    private float currentRadius;
    private float currentAngle = 0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentTrackIndex = Mathf.Clamp(startingTrackIndex, 0, trackRadii.Length - 1);
        currentRadius = trackRadii[currentTrackIndex];

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
        // 신버전 Input System의 키보드 입력 처리
        // 키보드가 연결되어 있지 않은 경우를 대비한 예외 처리
        if (Keyboard.current == null) return;

        // 구버전의 Input.GetKeyDown(KeyCode.A)와 완벽히 동일한 역할
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            currentTrackIndex--;
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            currentTrackIndex++;
        }

        currentTrackIndex = Mathf.Clamp(currentTrackIndex, 0, trackRadii.Length - 1);
    }

    private void MovePlayer()
    {
        currentAngle += runSpeed * Time.deltaTime;
        currentRadius = Mathf.Lerp(currentRadius, trackRadii[currentTrackIndex], Time.deltaTime * laneSwitchSpeed);

        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 newPosition = new Vector3(
            trackCenter.position.x + Mathf.Cos(rad) * currentRadius,
            transform.position.y,
            trackCenter.position.z + Mathf.Sin(rad) * currentRadius
        );

        transform.position = newPosition;

        Vector3 moveDirection = new Vector3(-Mathf.Sin(rad), 0, Mathf.Cos(rad));
        if (moveDirection != Vector3.zero) transform.rotation = Quaternion.LookRotation(moveDirection);

        if (animator != null) animator.SetBool("IsRunning", true);
    }
}
