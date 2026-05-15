using UnityEngine;

namespace ShiftRunner.Item {
    [CreateAssetMenu(menuName = "Game/Item Data")]
    public class ItemData : ScriptableObject {
        [field: SerializeField]
        public string DisplayName { get; private set; }

        [field: SerializeField]
        public ItemStoreType StoreType { get; private set; }

        
    }
}