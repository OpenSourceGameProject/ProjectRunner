using UnityEngine;

namespace ShiftRunner.Item {
    public class TestItem : Item {
        public override void OnUsed() {
            Debug.Log("TestItem used");
        }
    }
}