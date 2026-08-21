using InventorySystem.GUI;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace InventorySystem
{
    public class Inventory : NetworkBehaviour, IStaminaModifier, IMovementSpeedModifier
    {
        public const int MaxSlots = 8;

        public InventoryInfo UserInventory = new();

        [SyncVar(hook = nameof(OnItemUpdated))]
        public ItemIdentifier CurItem = ItemIdentifier.None;

        public bool SendItemsNextFrame;

        public bool SendAmmoNextFrame;

        private KeyCode _throwKey;

        private ItemIdentifier _prevCurItem;

        internal ReferenceHub _hub;

        private ItemBase _curInstance;

        private float _staminaModifier;

        private float _movementLimiter;

        private float _movementMultiplier;

        private bool _sprintingDisabled;

        private readonly Stopwatch _lastEquipSw = Stopwatch.StartNew();

        [SyncVar]
        private float _syncStaminaModifier;

        [SyncVar]
        private float _syncMovementLimiter;

        [SyncVar]
        private float _syncMovementMultiplier;

        [HideInInspector]
        public ItemBase CurInstance
        {
            get
            {
                return _curInstance;
            }
            set
            {
                if (value == _curInstance)
                {
                    return;
                }

                ItemBase curInstance = _curInstance;
                _curInstance = value;
                bool flag = _curInstance == null;
                if (curInstance != null)
                {
                    curInstance.OnHolstered();
                    curInstance.IsEquipped = false;
                    if (base.isLocalPlayer)
                    {
                        curInstance.ViewModel.gameObject.SetActive(value: false);
                        if (flag)
                        {
                            SharedHandsController.UpdateInstance(null);
                        }
                    }
                }

                if (_curInstance != null)
                {
                    if (base.isLocalPlayer)
                    {
                        _curInstance.ViewModel.gameObject.SetActive(value: true);
                        SharedHandsController.UpdateInstance(_curInstance.ViewModel);
                        _curInstance.ViewModel.OnEquipped();
                    }

                    _curInstance.OnEquipped();
                    _curInstance.IsEquipped = true;
                }
            }
        }

        public float LastItemSwitch => (float)_lastEquipSw.Elapsed.TotalSeconds;

        public Transform ItemWorkspace => SharedHandsController.Singleton.transform;

        public bool StaminaModifierActive => true;

        public bool MovementModifierActive => true;

        public float StaminaUsageMultiplier
        {
            get => IsObserver ? _syncStaminaModifier : _staminaModifier;
        }

        public float StaminaRegenMultiplier => 1f;

        public bool SprintingDisabled
        {
            get => IsObserver ? false : _sprintingDisabled;
        }

        public float MovementSpeedMultiplier
        {
            get => IsObserver ? _syncMovementMultiplier : _movementMultiplier;
        }

        public float MovementSpeedLimit
        {
            get => IsObserver ? _syncMovementLimiter : _movementLimiter;
        }

        private bool IsObserver
        {
            get
            {
                if (!NetworkServer.active)
                    return !isLocalPlayer;
                return false;
            }
        }

        public bool HasViewmodel
        {
            get
            {
                if (base.isLocalPlayer && CurInstance != null)
                {
                    return CurInstance.ViewModel != null;
                }

                return false;
            }
        }

        public static event Action<ReferenceHub> OnItemsModified;

        public static event Action<ReferenceHub> OnAmmoModified;

        public static event Action OnServerStarted;

        public static event Action OnLocalClientStarted;

        public static event Action<ReferenceHub, ItemIdentifier, ItemIdentifier> OnCurrentItemChanged;

        private void OnItemUpdated(ItemIdentifier prev, ItemIdentifier cur)
        {
            if (prev != cur)
                _lastEquipSw.Restart();
        }

        private void Awake()
        {
            _hub = ReferenceHub.GetHub(gameObject);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            if (!NetworkServer.active || RoundRestart.IsRoundRestarting)
            {
                return;
            }

            HashSet<ushort> hashSet = HashSetPool<ushort>.Shared.Rent();
            foreach (KeyValuePair<ushort, ItemBase> item in UserInventory.Items)
            {
                hashSet.Add(item.Key);
            }

            foreach (ushort item2 in hashSet)
            {
                this.ServerRemoveItem(item2, null);
            }

            HashSetPool<ushort>.Shared.Return(hashSet);
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            _throwKey = NewInput.GetKey(ActionName.ThrowItem, KeyCode.T);
            OnLocalClientStarted?.Invoke();

            if (isServer)
            {
                OnServerStarted?.Invoke();
                CustomNetworkManager.InvokeOnClientReady();
            }
        }

        private void Update()
        {
            if (isServer)
            {
                ProcessServerQueue();
            }

            UpdateCurItem();

            if (IsObserver)
                return;

            UpdateObserverItems();
            RefreshModifiers();

            if (isLocalPlayer && HasViewmodel && Input.GetKeyDown(_throwKey) && InventoryGuiController.ItemsSafeForInteraction)
            {
                CmdDropItem(CurItem.SerialNumber, tryThrow: true);
            }
        }

        private void ProcessServerQueue()
        {
            if (SendItemsNextFrame)
            {
                SendItemsNextFrame = false;
                OnItemsModified?.Invoke(_hub);
                ServerSendItems();
            }

            if (SendAmmoNextFrame)
            {
                SendAmmoNextFrame = false;
                OnAmmoModified?.Invoke(_hub);
                ServerSendAmmo();
            }
        }

        public void UpdateCurItem()
        {
            if (_prevCurItem == CurItem)
            {
                return;
            }

            if (base.isLocalPlayer)
            {
                if (CurItem.TypeId != ItemType.None)
                {
                    if (!UserInventory.Items.TryGetValue(CurItem.SerialNumber, out var value))
                    {
                        return;
                    }

                    CurInstance = value;
                }
                else
                {
                    CurInstance = null;
                }
            }

            OnCurrentItemChanged?.Invoke(_hub, _prevCurItem, CurItem);
            _prevCurItem = new ItemIdentifier(CurItem.TypeId, CurItem.SerialNumber);
        }

        public void UpdateObserverItems()
        {
            List<ushort> list = ListPool<ushort>.Shared.Rent();
            foreach (KeyValuePair<ushort, ItemBase> item in UserInventory.Items)
            {
                list.Add(item.Key);
            }

            foreach (ushort item2 in list)
            {
                if (UserInventory.Items.TryGetValue(item2, out var value) && value.enabled)
                {
                    if (value.IsEquipped)
                    {
                        value.EquipUpdate();
                    }

                    value.AlwaysUpdate();
                }
            }

            ListPool<ushort>.Shared.Return(list);
        }

        private void RefreshModifiers()
        {
            _staminaModifier = 1f;
            _movementLimiter = float.MaxValue;
            _movementMultiplier = 1f;
            _sprintingDisabled = false;

            foreach (var item in UserInventory.Items)
            {
                if (item.Value is IStaminaModifier staminaMod && staminaMod.StaminaModifierActive)
                {
                    _staminaModifier *= staminaMod.StaminaUsageMultiplier;
                    _sprintingDisabled |= staminaMod.SprintingDisabled;
                }

                if (item.Value is IMovementSpeedModifier speedMod && speedMod.MovementModifierActive)
                {
                    _movementLimiter = Mathf.Min(_movementLimiter, speedMod.MovementSpeedLimit);
                    _movementMultiplier *= speedMod.MovementSpeedMultiplier;
                }
            }

            if (NetworkServer.active)
            {
                _syncStaminaModifier = _staminaModifier;
                _syncMovementMultiplier = _movementMultiplier;
                _syncMovementLimiter = _movementLimiter;
            }
        }

        [Server]
        public void ServerSelectItem(ushort itemSerial)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                global::UnityEngine.Debug.LogWarning("[Server] function 'System.Void InventorySystem.Inventory::ServerSelectItem(System.UInt16)' called when server was not active");
            }
            else
            {
                if (itemSerial == CurItem.SerialNumber)
                {
                    return;
                }
                global::InventorySystem.Items.ItemBase value = null;
                global::InventorySystem.Items.ItemBase value2 = null;
                bool flag = CurItem.SerialNumber == 0 || (UserInventory.Items.TryGetValue(CurItem.SerialNumber, out value) && CurInstance != null);
                if (itemSerial == 0 || UserInventory.Items.TryGetValue(itemSerial, out value2))
                {
                    if ((CurItem.SerialNumber != 0 && flag && !global::InventorySystem.Items.EquipDequipModifierExtensions.CanHolster(value)) || (itemSerial != 0 && !global::InventorySystem.Items.EquipDequipModifierExtensions.CanEquip(value2)))
                    {
                        return;
                    }
                    if (itemSerial == 0)
                    {
                        CurItem = global::InventorySystem.Items.ItemIdentifier.None;
                        if (!base.isLocalPlayer)
                        {
                            CurInstance = null;
                        }
                    }
                    else
                    {
                        CurItem = new global::InventorySystem.Items.ItemIdentifier(value2.ItemTypeId, itemSerial);
                        if (!base.isLocalPlayer)
                        {
                            CurInstance = value2;
                        }
                    }
                }
                else if (!flag)
                {
                    CurItem = global::InventorySystem.Items.ItemIdentifier.None;
                    if (!base.isLocalPlayer)
                    {
                        CurInstance = null;
                    }
                }
            }
        }

        [Server]
        public void ServerSendItems()
        {
            if (!NetworkServer.active)
            {
                UnityEngine.Debug.LogWarning("[Server] function 'System.Void InventorySystem.Inventory::ServerSendItems()' called when server was not active");
            }
            else
            {
                if (base.isLocalPlayer)
                {
                    return;
                }

                HashSet<ItemIdentifier> hashSet = HashSetPool<ItemIdentifier>.Shared.Rent();
                foreach (KeyValuePair<ushort, ItemBase> item in UserInventory.Items)
                {
                    hashSet.Add(new ItemIdentifier(item.Value.ItemTypeId, item.Key));
                }

                TargetRefreshItems(hashSet.ToArray());
                HashSetPool<ItemIdentifier>.Shared.Return(hashSet);
            }
        }

        [Server]
        public void ServerSendAmmo()
        {
            if (!NetworkServer.active)
            {
                UnityEngine.Debug.LogWarning("[Server] function 'System.Void InventorySystem.Inventory::ServerSendAmmo()' called when server was not active");
            }
            else
            {
                if (base.isLocalPlayer)
                {
                    return;
                }

                List<byte> list = ListPool<byte>.Shared.Rent();
                List<ushort> list2 = ListPool<ushort>.Shared.Rent();
                foreach (KeyValuePair<ItemType, ushort> item in UserInventory.ReserveAmmo)
                {
                    list.Add((byte)item.Key);
                    list2.Add(item.Value);
                }

                TargetRefreshAmmo(list.ToArray(), list2.ToArray());
                ListPool<byte>.Shared.Return(list);
                ListPool<ushort>.Shared.Return(list2);
            }
        }

        [TargetRpc]
        private void TargetRefreshItems(ItemIdentifier[] ids)
        {
            Queue<ItemIdentifier> queue = new Queue<ItemIdentifier>();
            List<ushort> list = UserInventory.Items.Keys.ToList();
            int num = 0;
            for (int i = 0; i < ids.Length; i++)
            {
                ItemIdentifier item = ids[i];
                if (!UserInventory.Items.ContainsKey(item.SerialNumber))
                {
                    queue.Enqueue(item);
                }

                list.Remove(item.SerialNumber);
            }

            while (list.Count > 0)
            {
                DestroyItemInstance(list[0], null, out var _);
                UserInventory.Items.Remove(list[0]);
                list.RemoveAt(0);
                num++;
            }

            List<ushort> list2 = ListPool<ushort>.Shared.Rent();
            while (queue.Count > 0)
            {
                ItemIdentifier itemIdentifier = queue.Dequeue();
                ItemBase itemBase = CreateItemInstance(itemIdentifier, updateViewmodel: true);
                UserInventory.Items[itemIdentifier.SerialNumber] = itemBase;
                itemBase.OnAdded(null);
                if (itemBase is IAcquisitionConfirmationTrigger)
                {
                    list2.Add(itemIdentifier.SerialNumber);
                }

                if (itemIdentifier == CurItem)
                {
                    CurInstance = itemBase;
                }

                num++;
            }

            if (list2.Count > 0)
            {
                CmdConfirmAcquisition(list2.ToArray());
            }

            ListPool<ushort>.Shared.Return(list2);
            if (num > 0 && base.isLocalPlayer)
            {
                Inventory.OnItemsModified?.Invoke(_hub);
            }
        }

        [TargetRpc]
        private void TargetRefreshAmmo(byte[] keys, ushort[] values)
        {
            if (keys.Length == values.Length)
            {
                UserInventory.ReserveAmmo.Clear();
                for (int i = 0; i < keys.Length; i++)
                {
                    UserInventory.ReserveAmmo[(ItemType)keys[i]] = values[i];
                }

                Inventory.OnAmmoModified?.Invoke(_hub);
            }
        }

        [Command]
        public void CmdSelectItem(ushort itemSerial)
        {
            if (!_hub.interCoordinator.AnyBlocker(global::InventorySystem.Items.BlockedInteraction.OpenInventory))
            {
                ServerSelectItem(itemSerial);
            }
        }

        [Command]
        public void CmdProcessHotkey(ActionName hotkeyButtonPressed, ushort clientsideDesiredItem)
        {
            if (!_hub.interCoordinator.AnyBlocker(global::InventorySystem.Items.BlockedInteraction.OpenInventory))
            {
                ServerSelectItem(clientsideDesiredItem);
            }
        }

        [global::Mirror.Command(channel = 4)]
        private void CmdConfirmAcquisition(ushort[] itemSerials)
        {
            foreach (ushort key in itemSerials)
            {
                if (UserInventory.Items.TryGetValue(key, out var value) && value is IAcquisitionConfirmationTrigger { AcquisitionAlreadyReceived: false } acquisitionConfirmationTrigger)
                {
                    acquisitionConfirmationTrigger.ServerConfirmAcqusition();
                    acquisitionConfirmationTrigger.AcquisitionAlreadyReceived = true;
                }
            }
        }

        [global::Mirror.Command(channel = 4)]
        public void CmdDropItem(ushort itemSerial, bool tryThrow)
        {
            if (!UserInventory.Items.TryGetValue(itemSerial, out var value) || !global::InventorySystem.Items.EquipDequipModifierExtensions.CanHolster(value))
            {
                return;
            }
            global::InventorySystem.Items.Pickups.ItemPickupBase itemPickupBase = this.ServerDropItem(itemSerial);
            SendItemsNextFrame = true;
            if (tryThrow && !(itemPickupBase == null) && itemPickupBase.TryGetComponent<global::UnityEngine.Rigidbody>(out var component))
            {
                global::UnityEngine.Vector3 velocity = global::PlayerRoles.FirstPersonControl.FpcExtensionMethods.GetVelocity(_hub);
                global::UnityEngine.Vector3 velocity2 = velocity / 3f + _hub.PlayerCameraReference.forward * 6f * (global::UnityEngine.Mathf.Clamp01(global::UnityEngine.Mathf.InverseLerp(7f, 0.1f, component.mass)) + 0.3f);
                velocity2.x = global::UnityEngine.Mathf.Max(global::UnityEngine.Mathf.Abs(velocity.x), global::UnityEngine.Mathf.Abs(velocity2.x)) * (float)((!(velocity2.x < 0f)) ? 1 : (-1));
                velocity2.y = global::UnityEngine.Mathf.Max(global::UnityEngine.Mathf.Abs(velocity.y), global::UnityEngine.Mathf.Abs(velocity2.y)) * (float)((!(velocity2.y < 0f)) ? 1 : (-1));
                velocity2.z = global::UnityEngine.Mathf.Max(global::UnityEngine.Mathf.Abs(velocity.z), global::UnityEngine.Mathf.Abs(velocity2.z)) * (float)((!(velocity2.z < 0f)) ? 1 : (-1));
                component.position = _hub.PlayerCameraReference.position;
                component.linearVelocity = velocity2;
                component.angularVelocity = global::UnityEngine.Vector3.Lerp(value.ThrowSettings.RandomTorqueA, value.ThrowSettings.RandomTorqueB, global::UnityEngine.Random.value);
                float magnitude = component.angularVelocity.magnitude;
                if (magnitude > component.maxAngularVelocity)
                {
                    component.maxAngularVelocity = magnitude;
                }
            }
        }

        [global::Mirror.Command(channel = 4)]
        public void CmdDropAmmo(byte ammoType, ushort amount)
        {
            this.ServerDropAmmo((ItemType)ammoType, amount, checkMinimals: true);
        }

        public ItemBase CreateItemInstance(ItemIdentifier identifier, bool updateViewmodel)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(identifier.TypeId, out var value))
            {
                return null;
            }

            ItemBase itemBase = UnityEngine.Object.Instantiate(value, ItemWorkspace);
            itemBase.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            itemBase.Owner = _hub;
            itemBase.ItemSerial = identifier.SerialNumber;
            if (updateViewmodel && itemBase.ViewModel != null)
            {
                ItemViewmodelBase itemViewmodelBase = UnityEngine.Object.Instantiate(itemBase.ViewModel, itemBase.transform);
                itemViewmodelBase.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                itemViewmodelBase.InitLocal(itemBase);
                itemViewmodelBase.gameObject.SetActive(value: false);
                itemBase.ViewModel = itemViewmodelBase;
            }

            return itemBase;
        }

        public bool DestroyItemInstance(ushort targetInstance, ItemPickupBase pickup, out ItemBase foundItem)
        {
            if (!UserInventory.Items.TryGetValue(targetInstance, out foundItem))
            {
                return false;
            }

            foundItem.OnRemoved(pickup);
            if (CurInstance == foundItem)
            {
                CurInstance = null;
            }

            UnityEngine.Object.Destroy(foundItem.gameObject);
            return true;
        }
    }
}