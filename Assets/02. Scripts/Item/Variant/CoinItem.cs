using UnityEngine;

namespace ShiftRunner.Item {
    public class CoinItem : InstantItem {
        public int Amount { get; set; } = 10;

        protected override void UseEffect() {
            UI_Score.Instance.AddScore(Amount);
        }
    }
}