using UnityEngine;

namespace ShiftRunner.Item {
    public class TestInstantItem : InstantItem {
        protected override void UseEffect() {
            Debug.Log("TestInstantItem used");
        }
    }
}