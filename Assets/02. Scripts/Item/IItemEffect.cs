namespace ShiftRunner.Item {
    public interface IItemEffect {
        void Apply();
    }

    public interface IItemEffect<T> {
        void Apply(T context);
    }
}