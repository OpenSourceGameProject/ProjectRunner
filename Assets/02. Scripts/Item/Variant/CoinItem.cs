using UnityEngine;

namespace ShiftRunner.Item {
    public class CoinItem : InstantItem {
        private static readonly int amount = 50;

        public override void OnUsed() {
            UI_Score.Instance.AddScore(amount);
        }
    }
}