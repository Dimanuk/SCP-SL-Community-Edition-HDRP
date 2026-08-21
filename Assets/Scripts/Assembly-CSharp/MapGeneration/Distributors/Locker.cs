using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interactables;
using Interactables.Interobjects.DoorUtils;
using Interactables.Verification;
using InventorySystem.Items.Keycards;
using MEC;
using Mirror;
using NorthwoodLib.Pools;
using UnityEngine;

namespace MapGeneration.Distributors
{
    public class Locker : SpawnableStructure, IServerInteractable, IInteractable
    {
        public LockerLoot[] Loot;

        public LockerChamber[] Chambers;

        [SyncVar]
        public ushort OpenedChambers;

        [SerializeField]
        private AudioClip _grantedBeep;

        [SerializeField]
        private AudioClip _deniedBeep;

        [Header("Leave 0 to fill all chambers")]
        [SerializeField]
        private int _minChambersToFill;

        [SerializeField]
        private int _maxChambersToFill;

        private ushort _prevOpened;

        public IVerificationRule VerificationRule => StandardDistanceVerification.Default;

        public void ServerInteract(ReferenceHub ply, byte colliderId)
        {
            if (colliderId >= Chambers.Length || !Chambers[colliderId].CanInteract)
            {
                return;
            }
            bool flag = !CheckPerms(Chambers[colliderId].RequiredPermissions, ply) && !ply.serverRoles.BypassMode;
            if (flag)
            {
                RpcPlayDenied(colliderId);
                return;
            }
            Chambers[colliderId].SetDoor(!Chambers[colliderId].IsOpen, _grantedBeep);
            RefreshOpenedSyncvar();
        }

        protected virtual void Start()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            Timing.RunCoroutine(_ServerFillChambers().CancelWith(gameObject));
        }

        private IEnumerator<float> _ServerFillChambers()
        {
            while (!SeedSynchronizer.MapGenerated)
            {
                yield return Timing.WaitForOneFrame;
            }
            yield return Timing.WaitForOneFrame;
            yield return Timing.WaitForOneFrame;

            List<LockerChamber> list = new List<LockerChamber>(Chambers);
            if (_minChambersToFill != 0 && _maxChambersToFill >= _minChambersToFill)
            {
                int num = Chambers.Length - Random.Range(_minChambersToFill, _maxChambersToFill + 1);
                for (int i = 0; i < num; i++)
                {
                    list.RemoveAt(Random.Range(0, list.Count));
                }
            }
            foreach (LockerChamber item in list)
            {
                FillChamber(item);
            }
        }

        protected virtual void Update()
        {
            if (_prevOpened != OpenedChambers)
            {
                int num = 1;
                LockerChamber[] chambers = Chambers;
                foreach (LockerChamber lockerChamber in chambers)
                {
                    lockerChamber.SetDoor((OpenedChambers & num) == num || !lockerChamber.AnimatorSet, _grantedBeep);
                    num *= 2;
                }
                _prevOpened = OpenedChambers;
            }
        }

        private void RefreshOpenedSyncvar()
        {
            int num = 1;
            int num2 = 0;
            LockerChamber[] chambers = Chambers;
            for (int i = 0; i < chambers.Length; i++)
            {
                if (chambers[i].IsOpen)
                {
                    num2 += num;
                }
                num *= 2;
            }
            if (num2 != OpenedChambers)
            {
                OpenedChambers = (ushort)num2;
            }
        }

        public bool CheckPerms(KeycardPermissions perms, ReferenceHub ply)
        {
            if ((int)perms > 0)
            {
                if (ply.inventory.CurInstance == null)
                {
                    return false;
                }
                if (!(ply.inventory.CurInstance is KeycardItem keycardItem))
                {
                    return false;
                }
                if (!keycardItem.Permissions.HasFlagFast(perms))
                {
                    return false;
                }
            }
            return true;
        }

        [ClientRpc]
        private void RpcPlayDenied(byte chamberId)
        {
            if (chamberId <= Chambers.Length)
            {
                Chambers[chamberId].PlayDenied(_deniedBeep);
            }
        }

        private void FillChamber(LockerChamber ch)
        {
            List<int> list = ListPool<int>.Shared.Rent();
            for (int i = 0; i < Loot.Length; i++)
            {
                if (Loot[i].RemainingUses > 0 && (ch.AcceptableItems.Length == 0 || ch.AcceptableItems.Contains(Loot[i].TargetItem)))
                {
                    for (int j = 0; j <= Loot[i].ProbabilityPoints; j++)
                    {
                        list.Add(i);
                    }
                }
            }
            if (list.Count > 0)
            {
                int num = list[Random.Range(0, list.Count)];
                ch.SpawnItem(Loot[num].TargetItem, Random.Range(Loot[num].MinPerChamber, Loot[num].MaxPerChamber + 1));
                Loot[num].RemainingUses--;
            }
            ListPool<int>.Shared.Return(list);
        }
    }
}
