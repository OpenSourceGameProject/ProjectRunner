namespace ShiftRunner.Item {
    public interface IItem {
        ItemData Data { get; }

        void OnSpawned();
        void OnCollected(IItemCollector collector);
        void OnTick(float deltaTime);
        void OnUsed();
    }
}