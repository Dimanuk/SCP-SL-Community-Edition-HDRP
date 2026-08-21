namespace InventorySystem.Items.Usables
{
    public class PlayerHandler
    {
        public global::InventorySystem.Items.Usables.CurrentlyUsedItem CurrentUsable = global::InventorySystem.Items.Usables.CurrentlyUsedItem.None;

        public readonly global::System.Collections.Generic.List<global::InventorySystem.Items.Usables.RegenerationProcess> ActiveRegenerations = new global::System.Collections.Generic.List<global::InventorySystem.Items.Usables.RegenerationProcess>();

        public readonly global::System.Collections.Generic.Dictionary<ItemType, float> PersonalCooldowns = new global::System.Collections.Generic.Dictionary<ItemType, float>();

        public void DoUpdate(ReferenceHub hub)
        {
            global::PlayerStatsSystem.HealthStat module = hub.playerStats.GetModule<global::PlayerStatsSystem.HealthStat>();
            foreach (global::InventorySystem.Items.Usables.RegenerationProcess activeRegeneration in ActiveRegenerations)
            {
                activeRegeneration.GetValue(out var isDone, out var value);
                module.ServerHeal(value);
                if (isDone)
                {
                    ActiveRegenerations.Remove(activeRegeneration);
                    break;
                }
            }
        }

        public void ResetAll()
        {
            CurrentUsable = global::InventorySystem.Items.Usables.CurrentlyUsedItem.None;
            ActiveRegenerations.Clear();
            PersonalCooldowns.Clear();
        }
    }
}
