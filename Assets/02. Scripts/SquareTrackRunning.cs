using UnityEngine;

public class SquareTrackRunning : MonoBehaviour
{
    public Transform[] waypoints;
    public float runSpeed = 5f;
    public float rotationSpeed = 15f;
    public float reachThreshold = 0.2f;

    private int currentWaypointIndex = 0;
    private Animator animator; // 애니메이터 추가

    void Start()
    {
        animator = GetComponent<Animator>(); // 애니메이터 가져오기
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        MoveAlongTrack();
    }

    private void MoveAlongTrack()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 targetPosition = new Vector3(targetWaypoint.position.x, transform.position.y, targetWaypoint.position.z);
        Vector3 direction = targetPosition - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, runSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                                                  new Vector2(targetWaypoint.position.x, targetWaypoint.position.z));

        if (distanceToTarget < reachThreshold)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length) currentWaypointIndex = 0;
        }

        // 달리기 애니메이션 실행
        if (animator != null) animator.SetBool("IsRunning", true);
    }
}
