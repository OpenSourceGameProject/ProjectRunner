using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShiftRunner.Item {
    public class ItemSpawner : MonoBehaviour {
        [Serializable]
        private struct SpawnWeight {
            public ItemBehaviour itemPrefab;
            public float weight;
        }

        [Serializable]
        private class SpawnContext {
            public float despawnTime;
        }

        [SerializeField]
        private Transform[] spawnPoints;

        [SerializeField]
        private float spawnProbabilities;

        [SerializeField]
        private SpawnWeight[] spawnWeights;
        private float totalWeights;

        private readonly Dictionary<Transform, SpawnContext> _contexts = new();

        [SerializeField]
        private float minSpawnInterval = 5f;

        [SerializeField]
        private float maxSpawnInterval = 10f;
        private float SpawnInterval => UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);

        private float _spawnTimer = 0f;

        [SerializeField]
        private float despawnInterval = 5f;

        private void Awake() {
            totalWeights = 0f;
            foreach (var spawnWeight in spawnWeights) {
                totalWeights += spawnWeight.weight;
            }
        }

        private void Update() {
            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0f) {
                Spawn();
                _spawnTimer = SpawnInterval;
            }

            float currentTime = Time.time;
            foreach (var kvp in _contexts) {
                var spawnPoint = kvp.Key;
                var context = kvp.Value;

                if (currentTime >= context.despawnTime) {
                    if (spawnPoint.childCount > 0) {
                        Destroy(spawnPoint.GetChild(0).gameObject);
                    }
                    
                    _contexts.Remove(spawnPoint);
                }
            }
        }

        public void Spawn() {
            for (int i = 0; i < spawnPoints.Length; i++) {
                if (UnityEngine.Random.value > spawnProbabilities) {
                    continue;
                }

                var spawnPoint = spawnPoints[i];
                if (_contexts.ContainsKey(spawnPoint)) {
                    continue;
                }

                var itemPrefab = GetRandomItemPrefab();

                Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
                _contexts[spawnPoint] = new SpawnContext {
                    despawnTime = Time.time + despawnInterval
                };
            }
        }

        private ItemBehaviour GetRandomItemPrefab() {
            float randomValue = UnityEngine.Random.value * totalWeights;
            float cumulativeWeight = 0f;

            foreach (var spawnWeight in spawnWeights) {
                cumulativeWeight += spawnWeight.weight;
                if (randomValue <= cumulativeWeight) {
                    return spawnWeight.itemPrefab;
                }
            }

            return spawnWeights[^1].itemPrefab;
        }
    }
}