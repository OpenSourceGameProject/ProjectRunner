using UnityEngine;

namespace ShiftRunner.Item {
    public class ItemBehaviour : MonoBehaviour {
        [SerializeField]
        private ItemData _data;

        protected virtual void Start() {
            OnSpawned();
        }

        protected virtual void OnSpawned() { }

        public virtual void Collect(IItemCollector collector) {
            var item = ItemFactory.Create(_data);

            item.OnCollected(collector);
            OnCollected();
        }

        protected virtual void OnCollected() {
            // 대부분의 오브젝트는 수집되면 사라짐
            Destroy(gameObject);
        }
    }
}