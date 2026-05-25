using UnityEngine;

namespace ShiftRunner.Item {
    public class BombBehaviour : ItemBehaviour<BombItem> {
        [SerializeField]
        private int damage = 100;

        protected override void Initialize(BombItem item) {
            item.Damage = damage;
        }
    }
}