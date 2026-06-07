using UnityEngine;

namespace ShiftRunner.Item {
    public class ShieldItem : Item {
        private static int _instanceCount = 0;
        private static Collider[] _collidersCache = null;
        private static GameObject[] _effectsCache = null;
        private static int version = 0;

        public float Duration { get; set; } = 1f;
        private float timer = 0f;
        private bool active = false;
        private int instanceVersion = 0;

        public GameObject EffectPrefab { get; set; }

        public override void OnUsed() {
            if (active) return;

            CacheCollidersIfNeeded();
            active = true;

            instanceVersion = ++version;
            EnableColliders(false);
        }

        public override void OnTick(float deltaTime) {
            if (!active) return;
            timer += deltaTime;

            if (timer >= Duration) {
                // remove
                EnableColliders(true);

                active = false;
                timer = 0f;

                Release();
            }
        }

        private void CacheCollidersIfNeeded() {
            bool need = _collidersCache == null;
            for (int i = 0; !need && i < _collidersCache.Length; i++) {
                if (_collidersCache[i] == null) {
                    need = true;
                }
            }

            if (!need) return;

            var collisions = GameObject.FindObjectsByType<PlayerCollision>();
            _instanceCount = collisions.Length;

            _collidersCache = new Collider[_instanceCount];
            _effectsCache = new GameObject[_instanceCount];

            for (int i = 0; i < _instanceCount; i++) {
                collisions[i].TryGetComponent(out _collidersCache[i]);

                if (EffectPrefab != null) {
                    _effectsCache[i] = GameObject.Instantiate(EffectPrefab, collisions[i].transform);
                    _effectsCache[i].SetActive(false);
                }
            }
        }

        private void EnableColliders(bool enable) {
            if (instanceVersion != version) {
                // another instance is already active, do not modify colliders
                return;
            }

            for (int i = 0; i < _instanceCount; i++) {
                if (_collidersCache[i] != null) {
                    _collidersCache[i].enabled = enable;
                }

                if (_effectsCache[i] != null) {
                    _effectsCache[i].SetActive(!enable);
                }
            }
        }
    }
}