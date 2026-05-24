using UnityEngine;
using UnityEngine.UIElements;

public class RectangularCamera : MonoBehaviour
{
    [Header("추적 대상 및 위치 설정")]
    [Tooltip("사각형 트랙을 달리는 캐릭터를 여기에 넣으세요.")]
    public Transform target;

    [Tooltip("캐릭터를 기준으로 카메라가 얼마나 떨어져 있을지(X, Y, Z) 설정합니다.")]
    public Vector3 offset = new Vector3(0f, 4f, -6f);

    [Header("추적 부드러움 정도")]
    [Tooltip("숫자가 클수록 카메라가 캐릭터의 위치를 빠릿빠릿하게 따라갑니다.")]
    public float positionSmoothSpeed = 5f;

    [Tooltip("모서리를 돌 때 화면 회전이 얼마나 부드러울지 결정합니다.")]
    public float rotationSmoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // 1. 목표 위치: 캐릭터의 현재 위치 + 캐릭터가 바라보는 방향 기준 오프셋
        Vector3 desiredPosition = target.position + (target.rotation * offset);

        // 2. 부드러운 위치 이동 (Lerp)
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed * Time.deltaTime);

        // 3. 부드러운 카메라 회전 (Slerp를 사용하여 코너링 시 화면이 확 꺾이는 현상 방지)
        // 카메라가 캐릭터의 등 뒤에서 살짝 위(어깨너머)를 바라보도록 목표 회전값을 계산합니다.
        Vector3 lookTarget = target.position + Vector3.up * 1.5f;
        Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position);

        // 현재 회전값에서 목표 회전값으로 부드럽게 전환합니다.
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}
