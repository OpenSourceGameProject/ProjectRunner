namespace ShiftRunner.Item {
    public static class ItemFactory {
        public static Item Create(ItemData data) {
            Item item = data.Type switch {
                // normal items
                

                // test items
                ItemType.TestItem => new TestItem(),
                ItemType.TestInstantItem => new TestInstantItem(),

                // exception
                _ => throw new System.ArgumentException($"Unsupported ItemType: {data.Type}")
            };

            item.Initialize(data);
            return item;
        }
    }
}