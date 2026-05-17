namespace ShiftRunner.Item {
    public abstract class Item : IItem {
        public const string Tag = "Item";

        public ItemData Data { get; private set; }
        public bool IsValid {
            get {
                if (Data == null) return false;

                return true;
            }
        }

        public Item() {
            
        }

        // di
        public void Initialize(ItemData data) {
            Data = data;
        }

        public virtual void OnSpawned() { }

        public virtual void OnCollected(IItemCollector collector) {
            collector.Store(this);
        }

        public virtual void OnTick(float deltaTime) { }
        public abstract void OnUsed();
    }
}