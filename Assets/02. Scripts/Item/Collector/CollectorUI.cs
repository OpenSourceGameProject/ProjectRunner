using UnityEngine;

namespace ShiftRunner.Item {
    public class CollectorUI : MonoBehaviour {
        private IItemCollector _collector;
        private IItemCollector Collector {
            get {
                // 일단 PlayerCollector로 고정. 
                // 나중에 다른 Collector가 생기면 수정 필요.
                _collector ??= FindAnyObjectByType<PlayerCollector>(FindObjectsInactive.Exclude);

                return _collector;
            }
        }

        private void Update() {
            
        }
    }
}