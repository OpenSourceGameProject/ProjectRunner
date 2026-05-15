using System.Collections.Generic;

namespace ShiftRunner.Item {
    public interface IItemCollector {
        IReadOnlyList<IItem> Items { get; }
        void Use(IItem item);
    }
}