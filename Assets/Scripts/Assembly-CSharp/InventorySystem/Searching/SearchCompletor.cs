using InventorySystem.Items;
using InventorySystem.Items.Pickups;

namespace InventorySystem.Searching
{
    public abstract class SearchCompletor
    {
        protected readonly ReferenceHub Hub;
        protected readonly ItemPickupBase TargetPickup;
        protected readonly ItemBase TargetItem;
        protected readonly double MaxDistanceSqr;

        public static SearchCompletor FromPickup(SearchCoordinator coordinator, ItemPickupBase targetPickup, double maxDistanceSquared)
        {
            ReferenceHub hub = coordinator.Hub;

            if (!InventoryItemLoader.AvailableItems.TryGetValue(targetPickup.Info.ItemId, out var itemBase))
            {
                return null;
            }

            if (itemBase is ICustomSearchCompletorItem customItem)
                return customItem.GetCustomSearchCompletor(hub, targetPickup, itemBase, maxDistanceSquared);

            return new ItemSearchCompletor(hub, targetPickup, itemBase, maxDistanceSquared);
        }

        protected SearchCompletor(ReferenceHub hub, ItemPickupBase targetPickup, ItemBase targetItem, double maxDistanceSquared)
        {
            Hub = hub;
            TargetPickup = targetPickup;
            TargetItem = targetItem;
            MaxDistanceSqr = maxDistanceSquared;
        }

        protected bool ValidateDistance()
        {
            return (double)(TargetPickup.transform.position - Hub.transform.position).sqrMagnitude <= MaxDistanceSqr;
        }

        protected virtual bool ValidateAny()
        {
            if (!PlayerRoles.PlayerRolesUtils.IsHuman(Hub))
                return false;

            if (TargetPickup.Info.Locked)
                return false;

            if (InventorySystem.Disarming.DisarmedPlayers.IsDisarmed(Hub.inventory))
                return false;

            if (Hub.interCoordinator.AnyBlocker(BlockedInteraction.GrabItems))
                return false;

            return true;
        }

        public virtual bool ValidateStart()
        {
            if (ValidateAny())
                return ValidateDistance();
            return false;
        }

        public virtual bool ValidateUpdate()
        {
            if (TargetPickup != null && ValidateAny())
                return ValidateDistance();
            return false;
        }

        public abstract void Complete();
    }
}