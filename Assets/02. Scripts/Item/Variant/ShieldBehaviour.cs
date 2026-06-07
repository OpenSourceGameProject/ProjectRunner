using UnityEngine;

namespace ShiftRunner.Item {
    public class ShieldBehaviour : ItemBehaviour<ShieldItem> {
        [SerializeField]
        private float duration = 1f;

        [SerializeField]
        private GameObject effectPrefab;

        protected override void Initialize(ShieldItem item) {
            item.Duration = duration;
            item.EffectPrefab = effectPrefab;
        }
    }
}