using UnityEngine;
using UnityEngine.UI; // UI Image 제어를 위한 줄
using System.Collections; // 시간차 효과를 위한 줄

public class PlayerCollision : MonoBehaviour
{
    [Header("피격 효과 UI 설정")]
    public Image bloodImage;
    public float flashDuration = 0.15f;
    public float fadeDuration = 0.4f;
    public float maxAlpha = 0.6f;

    private Coroutine flashCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("⚠️ 장애물 충돌 감지!");

            if (UI_Hearts.Instance != null)
            {
                UI_Hearts.Instance.TakeDamage();
            }

            if (bloodImage != null)
            {
                if (flashCoroutine != null) StopCoroutine(flashCoroutine);
                flashCoroutine = StartCoroutine(FadeBloodEffect());
            }
        }
    }

    IEnumerator FadeBloodEffect()
    {
        Color color = bloodImage.color;

        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, maxAlpha, elapsedTime / flashDuration);
            bloodImage.color = color;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(maxAlpha, 0f, elapsedTime / fadeDuration);
            bloodImage.color = color;
            yield return null;
        }

        color.a = 0f;
        bloodImage.color = color;
    }
}