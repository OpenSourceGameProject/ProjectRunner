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
            Initialize(item);

            item.OnCollected(collector);
            OnCollected();
        }

        protected virtual void Initialize(Item item) { }

        protected virtual void OnCollected() {
            // 대부분의 오브젝트는 수집되면 사라짐
            Destroy(gameObject);
        }
    }

    public class ItemBehaviour<T> : ItemBehaviour where T : Item {
        protected sealed override void Initialize(Item item) {
            Initialize(item as T);
        }

        protected virtual void Initialize(T item) {
            
        }
    }
}