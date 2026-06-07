using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 플레이어의 충돌 감지 및 피격 연출을 제어하는 class입니다.
public class PlayerCollision : MonoBehaviour
{
    [Header("피격 효과 UI 설정")]
    public Image bloodImage;
    public float flashDuration = 0.15f;
    public float fadeDuration = 0.4f;
    public float maxAlpha = 0.6f;
    private Coroutine flashCoroutine;

    // Trigger Collider를 가진 오브젝트와 충돌 시 자동으로 호출되는 함수입니다.
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트의 태그가 "Obstacle"인지 검증합니다.
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("장애물 충돌 감지");
            // 체력 매니저 싱글톤 인스턴스가 존재할 경우 플레이어 데미지를 처리합니다.
            if (UI_Hearts.Instance != null)
            {
                UI_Hearts.Instance.TakeDamage();
            }

            // 피격 UI 이미지가 할당되어 있다면 화면 붉은색 깜빡임 연출을 실행합니다.
            if (bloodImage != null)
            {
                // 이미 피격 코루틴이 실행 중이라면 중복 방지를 위해 기존 코루틴을 강제 종료 시킵니다.
                if (flashCoroutine != null)
                {
                    StopCoroutine(flashCoroutine);
                }
                flashCoroutine = StartCoroutine(FadeBloodEffect());
            }
        }
    }

    // 시간 경과에 따라 피격 UI의 투명도를 부드럽게 조절하는 함수입니다.
    IEnumerator FadeBloodEffect()
    {
        // bloodImage의 현재 컬러 정보를 가져옵니다.
        Color color = bloodImage.color;
        float elapsedTime = 0f;

        // Fade In : 지정된 flashDuration 동안 투명도를 0에서 maxAlpha까지 부드럽게 증가시킵니다.
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, maxAlpha, elapsedTime / flashDuration);
            bloodImage.color = color;
            yield return null;
        }

        elapsedTime = 0f;

        // Fade Out : 지정된 fadeDuration 동안 투명도를 maxAlpha에서 0까지 부드럽게 감소시킵니다.
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(maxAlpha, 0f, elapsedTime / fadeDuration);
            bloodImage.color = color;
            yield return null;
        }

        // 예외 : 연출이 완전히 끝난 후 알파값을 정확히 0으로 세팅하여 화면을 초기화합니다.
        color.a = 0f;
        bloodImage.color = color;
    }
}