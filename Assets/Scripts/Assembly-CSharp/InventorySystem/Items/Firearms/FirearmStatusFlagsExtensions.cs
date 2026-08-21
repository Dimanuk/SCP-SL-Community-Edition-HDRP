namespace InventorySystem.Items.Firearms
{
    public static class FirearmStatusFlagsExtensions
    {
        public static bool HasFlagFast(this FirearmStatusFlags flags, FirearmStatusFlags flag)
        {
            return (flags & flag) == flag;
        }
    }
}