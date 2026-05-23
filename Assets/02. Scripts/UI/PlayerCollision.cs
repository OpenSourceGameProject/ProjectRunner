using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    // 유니티에서 충돌을 감지하는 절대 공식 함수
    private void OnTriggerEnter(Collider other)
    {
        // 부딪힌 오브젝트의 태그가 "Obstacle"(장애물)인지 확인
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("⚠️ 장애물 충돌 감지!");

            // 하트 UI를 찾아서 하트를 하나 줄이라고 명령
            if (UI_Hearts.Instance != null)
            {
                UI_Hearts.Instance.TakeDamage();
            }
        }
    }
}