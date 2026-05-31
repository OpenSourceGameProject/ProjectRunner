using UnityEngine;

namespace ShiftRunner.Item {
    public class ShieldItem : Item {
        public float Duration { get; set; } = 1f;
        private float timer = 0f;
        private bool active = false;

        public override void OnUsed() {
            if (active) return;

            active = true;
        }

        public override void OnTick(float deltaTime) {
            if (!active) return;
            timer += deltaTime;

            if (timer >= Duration) {
                // remove

                active = false;
                timer = 0f;

                Release();
            }
        }
    }
}