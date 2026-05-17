using UnityEngine;

namespace ShiftRunner.Item {
    public class TestInstantItem : InstantItem {
        public override void OnUsed() {
            Debug.Log("TestInstantItem used");
        }
    }
}