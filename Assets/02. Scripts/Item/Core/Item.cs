using UnityEngine;

namespace ShiftRunner.Item {
    public abstract class Item : MonoBehaviour, IItem {
        public const string Tag = "Item";

        [SerializeField]
        private ItemData _data;
        public ItemData Data => _data;

        public virtual void OnSpawned() { }

        public virtual void OnCollected(IItemCollector collector) { }

        public virtual void OnTick(float deltaTime) { }
        public abstract void OnUsed();

        protected virtual void Update() {
            OnTick(Time.deltaTime);
        }
    }
}