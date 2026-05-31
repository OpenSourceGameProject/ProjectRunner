using UnityEngine;

namespace ShiftRunner.Item {
    public class ShieldBehaviour : ItemBehaviour<ShieldItem> {
        [SerializeField]
        private float duration = 1f;

        protected override void Initialize(ShieldItem item) {
            item.Duration = duration;
        }
    }
}