namespace ShiftRunner.Item {
    /// <summary>
    /// 아이템이 보관함에 보관되어 있는지, 혹은 즉시 사용되는 아이템인지 등을 나타내는 열거형
    /// </summary>
    public enum ItemStoreType {
        /// <summary>
        /// 즉시 사용되는 아이템. 플레이어가 아이템을 획득하면 바로 효과가 발동
        /// </summary>
        None,     
        /// <summary>
        /// 보관함에 보관되는 아이템. 플레이어가 아이템을 획득하면 보관함에 저장되고, 플레이어가 원할 때 사용 가능
        /// </summary>
        Stored,
    }
}