using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossObstacleAttackSystem : MonoBehaviour
{
    public enum ObstacleType
    {
        Jump,
        Slide
    }

    [Header("Boss Animation")]
    public Animator bossAnimator;
    public string attackTriggerName = "Attack";
    public float attackMotionDelay = 1.0f;

    [Header("Obstacle Prefabs")]
    public GameObject jumpObstaclePrefab;
    public GameObject slideObstaclePrefab;

    [Header("Pattern Settings")]
    public int minObstacleCount = 10;
    public int maxObstacleCount = 20;
    public float obstaclePatternDuration = 20f;
    public float restTime = 4f;

    [Header("Track Settings")]
    public float[] laneRadii = { 8.8f, 10.2f, 11.6f };
    public float angleStep = 15f;

    [Header("Height Settings")]
    public float hiddenY = 11.5f;
    public float warningY = 13.35f;
    public float jumpObstacleY = 13.9f;
    public float slideObstacleY = 14.8f;

    [Header("Obstacle Move Settings")]
    public float riseDuration = 0.7f;
    public float fallDuration = 0.6f;
    public AnimationCurve riseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Obstacle Scale")]
    public Vector3 jumpObstacleScale = new Vector3(1.2f, 1.0f, 1.2f);
    public Vector3 slideObstacleScale = new Vector3(2.5f, 2.2f, 0.5f);

    [Header("Warning")]
    public GameObject warningPrefab;
    public float warningTime = 1.0f;
    public Vector3 warningScale = new Vector3(1.8f, 0.05f, 1.8f);

    [Header("Fairness Settings")]
    public bool avoidSamePosition = true;
    public float minAngleGapSameLane = 45f;
    public bool useAnyLaneGap = true;
    public float minAngleGapAnyLane = 15f;
    public int maxSpawnTryCount = 500;

    [Header("Option")]
    public bool startOnPlay = true;

    private readonly List<GameObject> currentObstacles = new List<GameObject>();
    private readonly List<GameObject> currentWarnings = new List<GameObject>();

    private Coroutine patternLoopCoroutine;

    private void Start()
    {
        if (startOnPlay)
        {
            patternLoopCoroutine = StartCoroutine(BossPatternLoop());
        }
    }

    private IEnumerator BossPatternLoop()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            yield return StartCoroutine(DoOneBossObstaclePattern());
            yield return new WaitForSeconds(restTime);
        }
    }

    private IEnumerator DoOneBossObstaclePattern()
    {
        ClearCurrentPattern();

        PlayBossAttackAnimation();

        yield return new WaitForSeconds(attackMotionDelay);

        List<SpawnPointData> spawnPoints = GenerateRandomSpawnPoints();

        SpawnWarnings(spawnPoints);

        yield return new WaitForSeconds(warningTime);

        ClearWarnings();

        SpawnObstaclesHidden(spawnPoints);

        yield return StartCoroutine(RiseAllObstacles());

        yield return new WaitForSeconds(obstaclePatternDuration);

        yield return StartCoroutine(FallAllObstacles());

        ClearCurrentPattern();
    }

    private void PlayBossAttackAnimation()
    {
        if (bossAnimator == null)
            return;

        if (string.IsNullOrEmpty(attackTriggerName))
            return;

        bossAnimator.SetTrigger(attackTriggerName);
    }

    private List<SpawnPointData> GenerateRandomSpawnPoints()
    {
        List<SpawnPointData> result = new List<SpawnPointData>();
        HashSet<string> usedKeys = new HashSet<string>();

        int targetObstacleCount = Random.Range(minObstacleCount, maxObstacleCount + 1);
        int angleCount = Mathf.RoundToInt(360f / angleStep);

        int tryCount = 0;

        while (result.Count < targetObstacleCount && tryCount < maxSpawnTryCount)
        {
            tryCount++;

            int laneIndex = Random.Range(0, laneRadii.Length);
            int angleIndex = Random.Range(0, angleCount);

            float radius = laneRadii[laneIndex];
            float angle = angleIndex * angleStep;

            string key = laneIndex + "_" + angleIndex;

            if (avoidSamePosition && usedKeys.Contains(key))
                continue;

            if (!IsFarEnoughFromSameLaneObstacles(result, laneIndex, angle))
                continue;

            if (useAnyLaneGap && !IsFarEnoughFromAnyLaneObstacles(result, angle))
                continue;

            usedKeys.Add(key);

            ObstacleType type = Random.value < 0.5f
                ? ObstacleType.Jump
                : ObstacleType.Slide;

            SpawnPointData data = new SpawnPointData
            {
                laneIndex = laneIndex,
                radius = radius,
                angle = angle,
                type = type
            };

            result.Add(data);
        }

        if (result.Count < targetObstacleCount)
        {
            Debug.LogWarning(
                "목표 장애물 개수보다 적게 생성됨: " +
                result.Count + "/" + targetObstacleCount +
                " | 최소 간격이 너무 넓거나 장애물 개수가 너무 많을 수 있습니다."
            );
        }

        return result;
    }

    private bool IsFarEnoughFromSameLaneObstacles(
        List<SpawnPointData> existingPoints,
        int newLaneIndex,
        float newAngle
    )
    {
        for (int i = 0; i < existingPoints.Count; i++)
        {
            SpawnPointData existing = existingPoints[i];

            if (existing.laneIndex != newLaneIndex)
                continue;

            float angleGap = Mathf.Abs(Mathf.DeltaAngle(existing.angle, newAngle));

            if (angleGap < minAngleGapSameLane)
                return false;
        }

        return true;
    }

    private bool IsFarEnoughFromAnyLaneObstacles(
        List<SpawnPointData> existingPoints,
        float newAngle
    )
    {
        for (int i = 0; i < existingPoints.Count; i++)
        {
            SpawnPointData existing = existingPoints[i];

            float angleGap = Mathf.Abs(Mathf.DeltaAngle(existing.angle, newAngle));

            if (angleGap < minAngleGapAnyLane)
                return false;
        }

        return true;
    }

    private void SpawnWarnings(List<SpawnPointData> spawnPoints)
    {
        if (warningPrefab == null)
            return;

        foreach (SpawnPointData data in spawnPoints)
        {
            Vector3 pos = GetPositionOnTrack(data.radius, data.angle, warningY);

            GameObject warning = Instantiate(
                warningPrefab,
                pos,
                Quaternion.Euler(0f, data.angle, 0f),
                transform
            );

            warning.name = "Warning_" + data.type;
            warning.transform.localScale = warningScale;

            currentWarnings.Add(warning);
        }
    }

    private void SpawnObstaclesHidden(List<SpawnPointData> spawnPoints)
    {
        foreach (SpawnPointData data in spawnPoints)
        {
            GameObject prefab = data.type == ObstacleType.Jump
                ? jumpObstaclePrefab
                : slideObstaclePrefab;

            if (prefab == null)
            {
                Debug.LogWarning(data.type + " 장애물 Prefab이 연결되지 않았습니다.");
                continue;
            }

            float activeY = data.type == ObstacleType.Jump
                ? jumpObstacleY
                : slideObstacleY;

            Vector3 hiddenPos = GetPositionOnTrack(data.radius, data.angle, hiddenY);
            Vector3 activePos = GetPositionOnTrack(data.radius, data.angle, activeY);

            GameObject obstacle = Instantiate(
                prefab,
                hiddenPos,
                Quaternion.Euler(0f, data.angle, 0f),
                transform
            );

            if (data.type == ObstacleType.Jump)
            {
                obstacle.name = "JumpObstacle";
                obstacle.transform.localScale = jumpObstacleScale;
            }
            else
            {
                obstacle.name = "SlideObstacle";
                obstacle.transform.localScale = slideObstacleScale;
            }

            ObstacleMoveData moveData = obstacle.AddComponent<ObstacleMoveData>();
            moveData.hiddenPosition = hiddenPos;
            moveData.activePosition = activePos;

            currentObstacles.Add(obstacle);
        }
    }

    private IEnumerator RiseAllObstacles()
    {
        float timer = 0f;

        while (timer < riseDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / riseDuration);
            float curvedT = riseCurve.Evaluate(t);

            for (int i = 0; i < currentObstacles.Count; i++)
            {
                GameObject obstacle = currentObstacles[i];

                if (obstacle == null)
                    continue;

                ObstacleMoveData moveData = obstacle.GetComponent<ObstacleMoveData>();

                if (moveData == null)
                    continue;

                obstacle.transform.position = Vector3.Lerp(
                    moveData.hiddenPosition,
                    moveData.activePosition,
                    curvedT
                );
            }

            yield return null;
        }

        SnapAllObstaclesToActivePosition();
    }

    private IEnumerator FallAllObstacles()
    {
        float timer = 0f;

        while (timer < fallDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fallDuration);
            float curvedT = riseCurve.Evaluate(t);

            for (int i = 0; i < currentObstacles.Count; i++)
            {
                GameObject obstacle = currentObstacles[i];

                if (obstacle == null)
                    continue;

                ObstacleMoveData moveData = obstacle.GetComponent<ObstacleMoveData>();

                if (moveData == null)
                    continue;

                obstacle.transform.position = Vector3.Lerp(
                    moveData.activePosition,
                    moveData.hiddenPosition,
                    curvedT
                );
            }

            yield return null;
        }
    }

    private void SnapAllObstaclesToActivePosition()
    {
        for (int i = 0; i < currentObstacles.Count; i++)
        {
            GameObject obstacle = currentObstacles[i];

            if (obstacle == null)
                continue;

            ObstacleMoveData moveData = obstacle.GetComponent<ObstacleMoveData>();

            if (moveData == null)
                continue;

            obstacle.transform.position = moveData.activePosition;
        }
    }

    private Vector3 GetPositionOnTrack(float radius, float angle, float y)
    {
        float rad = angle * Mathf.Deg2Rad;

        float x = Mathf.Sin(rad) * radius;
        float z = Mathf.Cos(rad) * radius;

        return new Vector3(x, y, z);
    }

    private void ClearWarnings()
    {
        for (int i = currentWarnings.Count - 1; i >= 0; i--)
        {
            if (currentWarnings[i] != null)
                Destroy(currentWarnings[i]);
        }

        currentWarnings.Clear();
    }

    private void ClearCurrentPattern()
    {
        ClearWarnings();

        for (int i = currentObstacles.Count - 1; i >= 0; i--)
        {
            if (currentObstacles[i] != null)
                Destroy(currentObstacles[i]);
        }

        currentObstacles.Clear();
    }

    public void StartPatternManually()
    {
        if (patternLoopCoroutine != null)
            StopCoroutine(patternLoopCoroutine);

        patternLoopCoroutine = StartCoroutine(BossPatternLoop());
    }

    public void StopPattern()
    {
        if (patternLoopCoroutine != null)
        {
            StopCoroutine(patternLoopCoroutine);
            patternLoopCoroutine = null;
        }

        ClearCurrentPattern();
    }

    private class SpawnPointData
    {
        public int laneIndex;
        public float radius;
        public float angle;
        public ObstacleType type;
    }

    private class ObstacleMoveData : MonoBehaviour
    {
        public Vector3 hiddenPosition;
        public Vector3 activePosition;
    }
}