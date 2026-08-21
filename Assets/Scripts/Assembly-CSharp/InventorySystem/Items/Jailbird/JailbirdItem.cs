using AudioPooling;
using Footprinting;
using InventorySystem.Drawers;
using InventorySystem.GUI;
using InventorySystem.Items.Autosync;
using InventorySystem.Items.Pickups;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Subroutines;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Networking;

namespace InventorySystem.Items.Jailbird
{
    public class JailbirdItem : AutosyncItem, IItemDescription, IItemNametag, IMovementInputOverride, IMovementSpeedModifier, IItemAlertDrawer, IItemDrawer, IEquipDequipModifier
    {
        private const float DamageLimit = 500f;
        private const float DamageWarning = 400f;
        private const int ChargesLimit = 5;
        private const int ChargesWarning = 4;
        private const float ServerChargeTolerance = 0.4f;
        private const float HintDuration = 10f;

        [SerializeField]
        private AudioClip _hitClip;

        [SerializeField]
        private float _meleeDelay;

        [SerializeField]
        private float _meleeCooldown;

        [SerializeField]
        private float _chargeDuration;

        [SerializeField]
        private float _chargeReadyTime;

        [SerializeField]
        private float _chargeMovementSpeedMultiplier;

        [SerializeField]
        private float _chargeMovementSpeedLimiter;

        [SerializeField]
        private float _chargeCancelVelocitySqr;

        [SerializeField]
        private float _chargeAutoengageTime;

        [SerializeField]
        private float _chargeDetectionDelay;

        [SerializeField]
        private float _brokenRemoveTime;

        [SerializeField]
        private JailbirdHitreg _hitreg;

        private readonly TolerantAbilityCooldown _serverAttackCooldown = new TolerantAbilityCooldown();
        private readonly AbilityCooldown _clientAttackCooldown = new AbilityCooldown();
        private readonly AbilityCooldown _clientDelayCooldown = new AbilityCooldown();

        private double _chargeResetTime;
        private bool _chargeLoading;
        private bool _charging;
        private bool _chargeAnyDetected;
        private bool _firstChargeFrame;
        private float _chargeLoadElapsed;
        private bool _attackTriggered;
        private bool _broken;

        // Host-mode deviation: on a listen-server the same JailbirdItem instance runs both
        // the client input flow and the server command flow. In the original code they share
        // _attackTriggered/_charging, which deadlocks the whole attack chain for the host
        // (EquipUpdate sets _attackTriggered before ServerProcessCmd sees the AttackTriggered
        // cmd, so the server branch is skipped; ClientAttack then clears the flag before the
        // AttackPerformed cmd is processed, so ServerAttack never runs — no damage, no RPCs,
        // and remote clients see the swing/charge poses stuck). NW never runs a playing
        // listen-host, so this was latent upstream. Server-side state lives in its own fields.
        private bool _serverAttackTriggered;
        private bool _serverCharging;

        private static float _localRemainingHint = HintDuration;

        public override float Weight => 1.7f;
        public int TotalChargesPerformed { get; private set; }
        public bool MovementOverrideActive => _charging || _serverCharging;
        public Vector3 MovementOverrideDirection => base.Owner.transform.forward;
        public bool MovementModifierActive => _charging || _serverCharging;
        public float MovementSpeedMultiplier => _chargeMovementSpeedMultiplier;
        public float MovementSpeedLimit => _chargeMovementSpeedLimiter;

        public bool AllowHolster => !_charging && !_serverCharging && !_chargeLoading;
        public bool AllowEquip => true;
        public string Description => "What?!";
        public string Name => "Jailbird";

        public string AlertText
        {
            get
            {
                if (_localRemainingHint < 0f)
                    return string.Empty;
                _localRemainingHint -= Time.deltaTime;
                string attackKey = string.Concat("</color>", new ReadableKeyCode(ActionName.Shoot), "<color=white>");
                string chargeKey = string.Concat("</color>", new ReadableKeyCode(ActionName.Zoom), "<color=white>");
                return string.Concat("<color=white>Press ", attackKey, " to attack.\nHold ", chargeKey, " to charge.</color>");
            }
        }

        public static event Action<ushort, JailbirdMessageType> OnRpcReceived;
        public event Action<JailbirdMessageType> OnCmdSent;

        public override void OnAdded(ItemPickupBase pickup)
        {
            base.OnAdded(pickup);
            _hitreg.Setup(this);

            if (NetworkServer.active && pickup is JailbirdPickup jailbirdPickup)
            {
                TotalChargesPerformed = jailbirdPickup.TotalCharges;
                _hitreg.TotalMeleeDamageDealt = jailbirdPickup.TotalMelee;
                ServerRecheckUsage();
            }
        }

        public override void OnRemoved(ItemPickupBase pickup)
        {
            base.OnRemoved(pickup);
            if (NetworkServer.active && pickup is JailbirdPickup jailbirdPickup)
            {
                if (_broken)
                {
                    jailbirdPickup.DestroySelf();
                    return;
                }
                jailbirdPickup.TotalCharges = TotalChargesPerformed;
                jailbirdPickup.TotalMelee = _hitreg.TotalMeleeDamageDealt;
            }
        }

        public override void OnHolstered()
        {
            base.OnHolstered();
            _chargeLoading = false;
            _charging = false;
            _attackTriggered = false;
            _serverCharging = false;
            _serverAttackTriggered = false;

            if (NetworkServer.active)
            {
                SendRpc(JailbirdMessageType.Holstered);
                if (_broken)
                {
                    base.OwnerInventory.ServerRemoveItem(base.ItemSerial, null);
                }
            }
        }

        public override void EquipUpdate()
        {
            base.EquipUpdate();

            if (_broken)
            {
                if (NetworkServer.active)
                {
                    _brokenRemoveTime -= Time.deltaTime;
                    if (_brokenRemoveTime < 0f)
                    {
                        base.OwnerInventory.ServerRemoveItem(base.ItemSerial, null);
                    }
                }
                return;
            }

            if (_charging || _serverCharging)
            {
                UpdateCharging();
                return;
            }

            if (!base.IsLocalPlayer)
                return;

            if (_attackTriggered)
            {
                if (_clientDelayCooldown.IsReady)
                {
                    ClientAttack();
                    _attackTriggered = false;
                }
                return;
            }

            bool itemsSafe = InventoryGuiController.ItemsSafeForInteraction;

            if (!_clientAttackCooldown.IsReady)
                return;

            if (itemsSafe && Input.GetKey(NewInput.GetKey(ActionName.Zoom, KeyCode.Mouse1)))
            {
                if (!_chargeLoading)
                {
                    _chargeLoading = true;
                    _chargeLoadElapsed = 0f;
                    SendCmd(JailbirdMessageType.ChargeLoadTriggered);
                }

                _chargeLoadElapsed += Time.deltaTime;
                if (_chargeAutoengageTime > _chargeLoadElapsed)
                    return;
            }

            if (_chargeLoading)
            {
                if (_chargeLoadElapsed > _chargeReadyTime)
                {
                    SendCmd(JailbirdMessageType.ChargeStarted);
                }
                else
                {
                    SendCmd(JailbirdMessageType.ChargeFailed);
                    _chargeLoading = false;
                }
            }

            if (!itemsSafe)
                return;

            if (Input.GetKeyDown(NewInput.GetKey(ActionName.InspectItem, KeyCode.None)))
            {
                SendCmd(JailbirdMessageType.Inspect);
            }

            if (Input.GetKey(NewInput.GetKey(ActionName.Shoot, KeyCode.Mouse0)))
            {
                _attackTriggered = true;
                SendCmd(JailbirdMessageType.AttackTriggered);
                _clientDelayCooldown.Trigger(_meleeDelay);
                _clientAttackCooldown.Trigger(_meleeCooldown);
            }
        }

        internal override void ClientProcessRpcTemplate(NetworkReader reader, ushort serial)
        {
            base.ClientProcessRpcTemplate(reader, serial);
            JailbirdMessageType msgType = (JailbirdMessageType)reader.ReadByte();
            OnRpcReceived?.Invoke(serial, msgType);

            if (msgType == JailbirdMessageType.AttackPerformed && reader.ReadBool() && InventoryExtensions.TryGetHubHoldingSerial(serial, out var hub))
            {
                AudioSourcePoolManager.PlaySound(_hitClip, hub.transform, 15f);
            }
        }

        internal override void ClientProcessRpcLocally(NetworkReader reader)
        {
            base.ClientProcessRpcLocally(reader);
            JailbirdMessageType msgType = (JailbirdMessageType)reader.ReadByte();
            switch (msgType)
            {
                case JailbirdMessageType.Broken:
                    _broken = true;
                    break;
                case JailbirdMessageType.ChargeStarted:
                    _charging = true;
                    _firstChargeFrame = true;
                    _chargeLoading = false;
                    _chargeAnyDetected = false;
                    _chargeResetTime = reader.ReadDouble();
                    break;
            }
        }

        internal override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);
            if (_broken || !base.IsEquipped) return;

            JailbirdMessageType msgType = (JailbirdMessageType)reader.ReadByte();
            switch (msgType)
            {
                case JailbirdMessageType.AttackTriggered:
                    if (!_serverAttackTriggered && _serverAttackCooldown.TolerantIsReady)
                    {
                        _serverAttackTriggered = true;
                        _serverAttackCooldown.Trigger(_meleeCooldown);
                        SendRpc(JailbirdMessageType.AttackTriggered);
                    }
                    break;

                case JailbirdMessageType.AttackPerformed:
                    if (_serverCharging)
                    {
                        ServerAttack(reader);
                    }
                    else if (_serverAttackTriggered)
                    {
                        _serverAttackTriggered = false;
                        ServerAttack(reader);
                    }
                    break;

                case JailbirdMessageType.ChargeLoadTriggered:
                case JailbirdMessageType.ChargeFailed:
                case JailbirdMessageType.Inspect:
                    SendRpc(msgType);
                    break;

                case JailbirdMessageType.ChargeStarted:
                    if (!_serverCharging)
                    {
                        _serverCharging = true;
                        _chargeResetTime = NetworkTime.time + (double)_chargeDuration;
                        TotalChargesPerformed++;
                        SendRpc(JailbirdMessageType.ChargeStarted, wr => wr.WriteDouble(_chargeResetTime));
                    }
                    break;
            }
        }

        private void UpdateCharging()
        {
            double remaining = _chargeResetTime - NetworkTime.time;
            if (NetworkServer.active && _serverCharging && remaining < -0.4)
            {
                ServerAttack(null);
                return;
            }

            if (!base.IsLocalPlayer || !_charging) return;

            if (!_chargeAnyDetected && _hitreg.AnyDetected)
            {
                remaining = Mathf.Min((float)remaining, _chargeDetectionDelay);
                _chargeResetTime = NetworkTime.time + remaining;
                _chargeAnyDetected = true;
            }

            if (remaining > 0.0)
            {
                if (_firstChargeFrame)
                {
                    _firstChargeFrame = false;
                    return;
                }
                if (base.Owner.GetVelocity().SqrMagnitudeIgnoreY() > _chargeCancelVelocitySqr)
                {
                    return;
                }
            }

            ClientAttack();
            _charging = false;
            _clientAttackCooldown.Trigger(_meleeCooldown);
        }

        private void ServerAttack(NetworkReader reader)
        {
            bool anyDamaged = _hitreg.ServerAttack(_serverCharging, reader);
            if (anyDamaged)
            {
                Hitmarker.SendHitmarker(base.Owner, 1f);
            }

            SendRpc(JailbirdMessageType.AttackPerformed, wr => wr.WriteBool(anyDamaged));
            ServerRecheckUsage();

            if (_serverCharging)
            {
                _serverCharging = false;
                if (_broken && anyDamaged && InventoryItemLoader.TryGetItem<InventorySystem.Items.ThrowableProjectiles.ThrowableItem>(ItemType.GrenadeHE, out var grenade))
                {
                    Vector3 pos = base.Owner.transform.position;
                    NetworkUtils.SendToAuthenticated(new InventorySystem.Items.Usables.Scp330.CandyPink.CandyExplosionMessage { Origin = pos });
                    InventorySystem.Items.ThrowableProjectiles.ExplosionGrenade.Explode(new Footprint(base.Owner), pos, grenade.Projectile as InventorySystem.Items.ThrowableProjectiles.ExplosionGrenade);
                }
            }
        }

        private void ClientAttack()
        {
            if (_hitreg.ClientTryAttack())
            {
                OnCmdSent?.Invoke(JailbirdMessageType.AttackPerformed);
            }
        }

        private void ServerRecheckUsage()
        {
            if (_hitreg.TotalMeleeDamageDealt >= DamageWarning || TotalChargesPerformed >= ChargesWarning)
            {
                SendRpc(JailbirdMessageType.AlmostDepleted);
                if (_hitreg.TotalMeleeDamageDealt >= DamageLimit || TotalChargesPerformed >= ChargesLimit)
                {
                    _broken = true;
                    SendRpc(JailbirdMessageType.Broken);
                }
            }
        }

        private void SendRpc(JailbirdMessageType header, Action<NetworkWriter> extraData = null)
        {
            using (new AutosyncRpc(this, true, out NetworkWriter writer))
            {
                writer.WriteByte((byte)header);
                extraData?.Invoke(writer);
            }
        }

        private void SendCmd(JailbirdMessageType msg)
        {
            using (new AutosyncCmd(this, out NetworkWriter writer))
            {
                writer.WriteByte((byte)msg);
            }
            OnCmdSent?.Invoke(msg);
        }
    }
}
