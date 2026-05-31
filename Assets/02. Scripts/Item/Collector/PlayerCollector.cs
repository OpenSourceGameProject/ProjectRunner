using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShiftRunner.Item {
    [RequireComponent(typeof(Collider))]
    public class PlayerCollector : MonoBehaviour, IItemCollector {
        [SerializeField]
        private int capacity = 1;
        
        private readonly List<IItem> _items = new();
        private readonly List<IItem> _usedItems = new();
        public IReadOnlyList<IItem> Items => _items;

        public event Action<IItemCollector> OnItemChanged;

        public void Use(IItem item) {
            if (!_items.Contains(item)) {
                return;
            }

            item.OnUsed();
            
            _items.Remove(item);
            _usedItems.Add(item);

            OnItemChanged?.Invoke(this);
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
            OnItemChanged?.Invoke(this);
            EnsureOnlyValidItems();
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag(Item.Tag)) return;
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

            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                Use(_items[0]);
            }
        }

        private void LateUpdate() {
            EnsureOnlyValidItems();

            float deltaTime = Time.deltaTime;
            foreach (var item in _items) {
                item.OnTick(deltaTime);
            }

            foreach (var item in _usedItems) {
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

            for (int i = _usedItems.Count - 1; i >= 0; i--) {
                if (_usedItems[i] is not null and { Data: not null }) {
                    continue;
                }

                _usedItems.RemoveAt(i);
            }
        }
    }
}