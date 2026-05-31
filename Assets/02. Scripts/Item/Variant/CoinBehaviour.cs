using UnityEngine;

namespace ShiftRunner.Item {
    public class CoinBehaviour : ItemBehaviour<CoinItem> {
        [SerializeField]
        private int amount = 1;

        protected override void Initialize(CoinItem item) {
            item.Amount = amount;
        }
    }
}