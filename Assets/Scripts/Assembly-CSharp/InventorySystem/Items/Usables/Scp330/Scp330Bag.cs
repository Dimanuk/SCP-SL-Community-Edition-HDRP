using System;
using System.Collections.Generic;
using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp330
{
    public class Scp330Bag : UsableItem, ICustomSearchCompletorItem, IAcquisitionConfirmationTrigger
    {
        public int SelectedCandyId;

        public List<CandyKindID> Candies;

        public const int MaxCandies = 6;

        public override bool CanStartUsing => false;

        public bool AcquisitionAlreadyReceived { get; set; }

        public override ItemDescriptionType DescriptionType => ItemDescriptionType.Scp330Bag;

        public bool IsCandySelected
        {
            get
            {
                if (SelectedCandyId < 0)
                    return false;

                return Candies != null && SelectedCandyId < Candies.Count;
            }
        }

        public SearchCompletor GetCustomSearchCompletor(ReferenceHub hub, ItemPickupBase ipb, ItemBase ib, double disSqrt)
        {
            return new Scp330SearchCompletor(hub, ipb, ib, disSqrt);
        }

        public override void OnAdded(ItemPickupBase pickup)
        {
            base.OnAdded(pickup);

            if (!NetworkServer.active)
                return;

            Scp330Pickup scp330Pickup = pickup as Scp330Pickup;
            if (!ServerProcessPickup(Owner, scp330Pickup, out Scp330Bag bag))
                return;

            if (bag == null)
                return;

            if (bag == this)
                return;

            ServerRemoveSelf();
        }

        public override void OnRemoved(ItemPickupBase pickup)
        {
            base.OnRemoved(pickup);

            if (!NetworkServer.active)
                return;

            Scp330Pickup scp330Pickup = pickup as Scp330Pickup;
            if (scp330Pickup != null)
                scp330Pickup.StoredCandies = Candies;
        }

        public override void OnEquipped()
        {
            SelectedCandyId = -1;
        }

        public override void OnHolstered()
        {
            IsUsing = false;
        }

        public void ServerConfirmAcqusition()
        {
            ServerRefreshBag();
        }

        public override void ServerOnUsingCompleted()
        {
            if (SelectedCandyId < 0 || Candies == null || SelectedCandyId >= Candies.Count)
                return;

            if (!Scp330Candies.Dictionarized)
            {
                foreach (ICandy candy in Scp330Candies.AllCandies)
                {
                    if (candy != null)
                        Scp330Candies.DictionarizedCandies[candy.Kind] = candy;
                }
                Scp330Candies.Dictionarized = true;
            }

            CandyKindID selectedKind = Candies[SelectedCandyId];

            if (!Scp330Candies.DictionarizedCandies.TryGetValue(selectedKind, out ICandy candyInstance))
                return;

            IsUsing = false;
            candyInstance.ServerApplyEffects(Owner);

            Candies.RemoveAt(SelectedCandyId);
            OwnerInventory.ServerSelectItem(0);
            ServerRefreshBag();
        }

        public void DropCandy(int index)
        {
            SendClientMessage(index, true);
        }

        public void SelectCandy(int index)
        {
            SelectedCandyId = index;
            SendClientMessage(index, false);
        }

        public bool TryAddSpecific(CandyKindID kind)
        {
            if (Candies == null || Candies.Count >= MaxCandies)
                return false;

            Candies.Add(kind);
            return true;
        }

        public CandyKindID TryRemove(int index)
        {
            if (index < 0 || Candies == null || index >= Candies.Count)
                return CandyKindID.None;

            CandyKindID kind = Candies[index];
            Candies.RemoveAt(index);
            ServerRefreshBag();
            return kind;
        }

        public static bool ServerProcessPickup(ReferenceHub ply, Scp330Pickup pickup, out Scp330Bag bag)
        {
            if (!TryGetBag(ply, out bag))
            {
                ushort serial = (pickup != null) ? pickup.Info.Serial : (ushort)0;
                return ply.inventory.ServerAddItem(ItemType.SCP330, serial, pickup) != null;
            }

            bool result = false;
            if (pickup == null)
            {
                result = bag.TryAddSpecific(Scp330Candies.GetRandom());
            }
            else
            {
                while (pickup.StoredCandies.Count > 0 && bag.TryAddSpecific(pickup.StoredCandies[0]))
                {
                    result = true;
                    pickup.StoredCandies.RemoveAt(0);
                }
            }

            if (bag.AcquisitionAlreadyReceived)
                bag.ServerRefreshBag();

            return result;
        }

        public static bool TryGetBag(ReferenceHub hub, out Scp330Bag bag)
        {
            bag = null;

            if (hub == null || hub.inventory == null || hub.inventory.UserInventory == null)
                return false;

            foreach (var kvp in hub.inventory.UserInventory.Items)
            {
                if (kvp.Value is Scp330Bag scp330Bag)
                {
                    bag = scp330Bag;
                    if (scp330Bag.Candies != null && scp330Bag.Candies.Count > 0)
                        return true;
                }
            }

            return bag != null;
        }

        public static void AddSimpleRegeneration(ReferenceHub hub, float rate, float duration)
        {
            AnimationCurve curve = AnimationCurve.Constant(0f, duration, rate);
            var handler = UsableItemsController.GetHandler(hub);

            if (handler == null)
                return;

            var regen = new RegenerationProcess(curve, 1f, 1f);
            handler.ActiveRegenerations.Add(regen);
        }

        public void SendClientMessage(int candyIdex, bool drop)
        {
            var msg = new SelectScp330Message
            {
                Serial = ItemSerial,
                CandyID = candyIdex,
                Drop = drop
            };
            NetworkClient.Send(msg);
        }

        public void ServerRefreshBag()
        {
            if (Candies == null || Candies.Count == 0)
            {
                ServerRemoveSelf();
                return;
            }

            var msg = new SyncScp330Message
            {
                Serial = ItemSerial,
                Candies = Candies
            };

            var conn = OwnerInventory.connectionToClient;
            if (conn != null)
                conn.Send(msg);
        }

        public Scp330Bag()
        {
            Candies = new List<CandyKindID>();
        }
    }
}
