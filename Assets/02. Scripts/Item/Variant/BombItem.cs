using UnityEngine;

namespace ShiftRunner.Item {
    public class BombItem : InstantItem {
        public int Damage { get; set; } = 100;
        public GameObject ExplosionEffectPrefab { get; set; }

        protected override void UseEffect() {
            UI_Score.Instance.AddScore(Damage);

            // ...
            GameObject.Instantiate(ExplosionEffectPrefab);
        }
    }
}