using UnityEngine;

public class CircularCamera : MonoBehaviour
{
    [Header("추적 대상 및 위치 설정")]
    [Tooltip("따라다닐 캐릭터를 여기에 넣으세요.")]
    public Transform target;

    [Tooltip("캐릭터를 기준으로 카메라가 얼마나 떨어져 있을지(X, Y, Z) 설정합니다.")]
    public Vector3 offset = new Vector3(0f, 3f, -5f);

    [Header("추적 부드러움 정도")]
    [Tooltip("숫자가 클수록 카메라가 캐릭터에 빠릿빠릿하게 따라붙습니다.")]
    public float smoothSpeed = 10f;

    // 카메라는 캐릭터의 이동이 모두 끝난 뒤에 따라가야 덜컹거리지 않으므로 Update 대신 LateUpdate를 씁니다.
    void LateUpdate()
    {
        // 타겟이 없으면 에러를 뿜지 않게 방어
        if (target == null) return;

        // 1. 목표 위치 계산 (원형 트랙이므로 캐릭터가 바라보는 방향을 기준으로 오프셋을 더해줌)
        Vector3 desiredPosition = target.position + (target.rotation * offset);

        // 2. 현재 위치에서 목표 위치로 부드럽게 이동 (Lerp)
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 3. 카메라가 항상 캐릭터의 살짝 위(머리~어깨)를 쳐다보도록 회전
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}