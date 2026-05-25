using UnityEngine;

namespace ShiftRunner.Item {
    public class CoinItem : InstantItem {
        public int Amount { get; set; } = 10;

        public override void OnUsed() {
            UI_Score.Instance.AddScore(Amount);
        }
    }
}