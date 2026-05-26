using UnityEngine;

namespace ShiftRunner.Item {
    public class BombItem : InstantItem {
        public int Damage { get; set; } = 100;

        protected override void UseEffect() {
            /// ...
        }
    }
}