using System.Collections.Generic;
using UnityEngine;

namespace ShiftRunner.Item {
    [RequireComponent(typeof(Collider))]
    public class PlayerCollector : MonoBehaviour, IItemCollector {
        [SerializeField]
        private int capacity = 1;
        
        private readonly List<IItem> _items = new();
        public IReadOnlyList<IItem> Items => _items;

        public void Use(IItem item) {
            if (!_items.Contains(item)) {
                return;
            }

            item.OnUsed();
            EnsureOnlyValidItems();
        }

        public void Store(IItem item) {
            if (_items.Contains(item)) {
                return;
            }

            if (_items.Count >= capacity) {
                return;
            }

            _items.Add(item);
            EnsureOnlyValidItems();
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag(Item.Tag)) return;
            if (!other.TryGetComponent(out ItemBehaviour itemObject)) {
                // why?

                return;
            }

            itemObject.Collect(this);
        }

        private void Update() {
            if (_items.Count == 0) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                Use(_items[0]);
            }
        }

        private void LateUpdate() {
            EnsureOnlyValidItems();

            float deltaTime = Time.deltaTime;
            foreach (var item in _items) {
                item.OnTick(deltaTime);
            }
        }

        /// <summary>
        /// Removes null or invalid items from the list.
        /// </summary>
        private void EnsureOnlyValidItems() {
            for (int i = _items.Count - 1; i >= 0; i--) {
                if (_items[i] is not null and { Data: not null }) {
                    continue;
                }

                _items.RemoveAt(i);
            }
        }
    }
}