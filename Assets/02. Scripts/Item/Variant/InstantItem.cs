namespace ShiftRunner.Item {
    public abstract class InstantItem : Item {

        // InstantItem은 Collector에 저장되지 않고, 즉시 효과가 발동되는 아이템입니다. 
        // ex) 체력 회복 아이템, 점수 증가 등
        public override void OnCollected(IItemCollector collector) {
            // base.OnCollected(collector);

            OnUsed();
        }
    }
}