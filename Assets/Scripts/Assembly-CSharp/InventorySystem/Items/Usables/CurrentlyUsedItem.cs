namespace InventorySystem.Items.Usables
{
    public readonly struct CurrentlyUsedItem
    {
        public static global::InventorySystem.Items.Usables.CurrentlyUsedItem None = new global::InventorySystem.Items.Usables.CurrentlyUsedItem(null, 0, 0f);

        public readonly global::InventorySystem.Items.Usables.UsableItem Item;

        public readonly ushort ItemSerial;

        public readonly float StartTime;

        public CurrentlyUsedItem(global::InventorySystem.Items.Usables.UsableItem item, ushort serial, float startTime)
        {
            Item = item;
            ItemSerial = serial;
            StartTime = startTime;
        }
    }
}
