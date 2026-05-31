using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField]
        private Image[] icons;

        private async void Start() {
            await GetCollector();

            Collector.OnItemChanged += OnCollectorItemChanged;
        }

        private void OnDestroy() {
            if (Collector is not null) {
                Collector.OnItemChanged -= OnCollectorItemChanged;
            }
        }

        private void OnCollectorItemChanged(IItemCollector collector) {
            // Debug.Log("Collector item changed, updating UI...");
            for (int i = 0; i < icons.Length; i++) {
                if (i >= collector.Items.Count) {
                    icons[i].sprite = null;
                    continue;
                }

                icons[i].sprite = collector.Items[i].Data.Icon;
            }
        }

        private async Awaitable GetCollector() {
            while (Collector is null) {
                await Awaitable.NextFrameAsync();
            }
        }
    }
}