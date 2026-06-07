using UnityEngine;

namespace ShiftRunner.Item {
    public class BombBehaviour : ItemBehaviour<BombItem> {
        [SerializeField]
        private int damage = 100;

        [SerializeField]
        private GameObject explosionEffectPrefab;

        protected override void Initialize(BombItem item) {
            item.Damage = damage;
            item.ExplosionEffectPrefab = explosionEffectPrefab;
        }
    }
}