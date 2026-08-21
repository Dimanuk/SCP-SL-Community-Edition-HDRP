using InventorySystem.GUI;
using InventorySystem.Items.Pickups;
using Scp914;
using UnityEngine;
using Mirror;
using VoiceChat.Playbacks;

namespace InventorySystem.Items.Radio
{
    public class RadioItem : ItemBase, IAcquisitionConfirmationTrigger, IItemDescription, IItemNametag, IUpgradeTrigger
    {
        public RadioRangeMode[] Ranges;
        public AnimationCurve VoiceVolumeCurve;
        public AnimationCurve NoiseLevelCurve;

        private ItemTranslationReader _itm;
        private bool _enabled;
        private float _battery;
        private byte _lastSentBatteryLevel;
        private byte _rangeId;

        private static KeyCode _circleModeKey;
        private static KeyCode _toggleKey;

        public bool AcquisitionAlreadyReceived { get; set; }

        public override float Weight => 1.7f;

        public bool IsUsable
        {
            get => _enabled && _battery > 0f;
        }

        public string Description => _itm.Description;
        public string Name => _itm.Name;

        public byte BatteryPercent
        {
            get => (byte)Mathf.RoundToInt(_battery * 100f);
            set => _battery = value / 100f;
        }

        public RadioMessages.RadioRangeLevel RangeLevel => (RadioMessages.RadioRangeLevel)(_enabled ? _rangeId : -1);

        public void ServerConfirmAcqusition()
        {
            SendStatusMessage();
        }

        public void ServerOnUpgraded(Scp914KnobSetting setting)
        {
            BatteryPercent = 100;
            SendStatusMessage();
        }

        public override void OnAdded(ItemPickupBase ipb)
        {
            if (IsLocalPlayer)
            {
                _itm = new ItemTranslationReader(ItemTypeId);
                _circleModeKey = NewInput.GetKey(ActionName.Shoot);
                _toggleKey = NewInput.GetKey(ActionName.Zoom);
            }

            if (NetworkServer.active)
            {
                if (ipb is RadioPickup radioPickup)
                {
                    _enabled = radioPickup.SavedEnabled;
                    _rangeId = radioPickup.SavedRange;
                    _battery = radioPickup.SavedBattery;
                }
                else
                {
                    _enabled = true;
                    _battery = 1f;
                    _rangeId = 1;
                }
            }
        }

        public override void OnRemoved(ItemPickupBase pickup)
        {
            if (NetworkServer.active && pickup is RadioPickup radioPickup)
            {
                radioPickup.SavedEnabled = _enabled;
                radioPickup.SavedRange = _rangeId;
                radioPickup.SavedBattery = _battery;
            }
        }

        public override ItemPickupBase ServerDropItem()
        {
            if (!NetworkServer.active)
                throw new System.InvalidOperationException("Method ServerDropItem can only be executed on the server.");

            if (PickupDropModel == null)
            {
                Debug.LogError("No pickup drop model set. Could not drop the item.");
                return null;
            }

            PickupSyncInfo psi = new(ItemTypeId, Owner.transform.position, Quaternion.identity, Weight, ItemSerial);
            ItemPickupBase pickup = OwnerInventory.ServerCreatePickup(this, psi, spawn: false);

            if (pickup is RadioPickup radioPickup)
            {
                radioPickup.SavedEnabled = _enabled;
                radioPickup.SavedRange = _rangeId;
                radioPickup.SavedBattery = _battery;
            }

            NetworkServer.Spawn(pickup.gameObject);
            OwnerInventory.ServerRemoveItem(psi.Serial, pickup);
            pickup.PreviousOwner = new Footprinting.Footprint(Owner);
            return pickup;
        }

        public override void OnEquipped()
        {
            if (NetworkServer.active)
            {
                SendStatusMessage();
            }
        }

        public override void EquipUpdate()
        {
            if (!IsLocalPlayer || !InventoryGuiController.ItemsSafeForInteraction || Cursor.visible)
                return;

            if (Input.GetKeyDown(_circleModeKey) && _enabled)
            {
                base.Owner.connectionToServer.Send(new ClientRadioCommandMessage(RadioMessages.RadioCommand.ChangeRange));
            }

            if (Input.GetKeyDown(_toggleKey))
            {
                RadioMessages.RadioCommand cmd = _enabled ? RadioMessages.RadioCommand.Disable : RadioMessages.RadioCommand.Enable;
                base.Owner.connectionToServer.Send(new ClientRadioCommandMessage(cmd));
            }
        }

        public void ServerProcessCmd(RadioMessages.RadioCommand command)
        {
            switch (command)
            {
                case RadioMessages.RadioCommand.Enable:
                    if (_enabled || _battery <= 0f) return;
                    _enabled = true;
                    break;
                case RadioMessages.RadioCommand.Disable:
                    if (!_enabled) return;
                    _enabled = false;
                    break;
                case RadioMessages.RadioCommand.ChangeRange:
                    _rangeId = (byte)((_rangeId + 1) % Ranges.Length);
                    break;
            }
            SendStatusMessage();
        }

        public void UserReceiveInfo(RadioStatusMessage info)
        {
            _enabled = info.Range != RadioMessages.RadioRangeLevel.RadioDisabled;
            _battery = info.Battery / 100f;
        }

        private void Update()
        {
            if (NetworkServer.active && IsUsable)
            {
                float consumptionRate = PersonalRadioPlayback.IsTransmitting(base.Owner)
                    ? Ranges[_rangeId].MinuteCostWhenTalking
                    : Ranges[_rangeId].MinuteCostWhenIdle;

                _battery = Mathf.Clamp01(_battery - (consumptionRate / 60f / 100f) * Time.deltaTime);

                if (_battery <= 0f)
                {
                    _enabled = false;
                }

                if (Mathf.Abs(_lastSentBatteryLevel - BatteryPercent) >= 1 && base.OwnerInventory.CurItem.TypeId == ItemType.Radio)
                {
                    SendStatusMessage();
                }
            }
        }

        private void SendStatusMessage()
        {
            _lastSentBatteryLevel = BatteryPercent;
            NetworkServer.SendToReady(new RadioStatusMessage(this));
        }
    }
}
