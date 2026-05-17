using System.Collections.Generic;
using UnityEngine;

namespace ShiftRunner.Item {
    [RequireComponent(typeof(Collider))]
    public class PlayerCollector : MonoBehaviour, IItemCollector {
        private readonly List<IItem> _items = new();
        public IReadOnlyList<IItem> Items => _items;

        public void Use(IItem item) {
            if (!_items.Contains(item)) {
                return;
            }

            item.OnUsed();
        }

        public void Store(IItem item) {
            if (_items.Contains(item)) {
                return;
            }

            _items.Add(item);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag(Item.Tag)) return;
            if (!other.TryGetComponent(out ItemBehaviour itemObject)) {
                // why?

                return;
            }

            itemObject.Collect(this);
        }
    }
}