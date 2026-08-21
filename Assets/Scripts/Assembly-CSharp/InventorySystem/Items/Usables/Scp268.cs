using CustomPlayerEffects;
using InventorySystem.Items.Usables;
using Mirror;
using System;
using System.Diagnostics;
using UnityEngine;

namespace InventorySystem.Items.Usables
{
    public class Scp268 : UsableItem, IWearableItem
    {
        private bool _isWorn;

        private const float InvisibilityTime = 15f;
        private const float CooldownTime = 120f;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        // Base getter also ticks RemainingCooldown down each frame — the original
        // client inlines the full base body here after the IsWorn check.
        public override string AlertText => IsWorn ? string.Empty : base.AlertText;

        public bool IsWorn
        {
            get
            {
                if (!base.IsLocalPlayer || NetworkServer.active)
                    return _isWorn;

                return Effect.Intensity > 0;
            }
            set => _isWorn = value;
        }

        public override bool AllowHolster
        {
            get
            {
                if (IsUsing)
                    return IsWorn;

                return true;
            }
        }

        private Invisible Effect => base.Owner.playerEffectsController.GetEffect<Invisible>();

        public override void ServerOnUsingCompleted()
        {
            IsUsing = false;
            IsWorn = true;
            SetState(true);
            ServerSetPersonalCooldown(CooldownTime);
        }

        public override void OnHolstered()
        {
            base.OnHolstered();

            if (NetworkServer.active)
                SetState(false);

            if (base.IsLocalPlayer)
                IsUsing = false;
        }

        public override void EquipUpdate()
        {
            base.EquipUpdate();

            if (base.IsLocalPlayer && IsWorn && IsUsing)
                IsUsing = false;

            if (NetworkServer.active && _stopwatch.IsRunning)
            {
                if (_stopwatch.Elapsed.TotalSeconds >= InvisibilityTime || Effect.Intensity == 0)
                {
                    SetState(false);
                }
            }
        }

        private void SetState(bool state)
        {
            if (state)
            {
                Effect.Intensity = 1;
                _stopwatch.Restart();
            }
            else if (IsWorn)
            {
                Effect.Intensity = 0;
                _stopwatch.Stop();
                IsWorn = false;

                if (base.OwnerInventory.CurItem.TypeId == ItemTypeId)
                {
                    base.OwnerInventory.ServerSelectItem(0);
                }
            }
        }

        public Scp268()
        {
            _stopwatch = new Stopwatch();
            CanStartUsing = true;
        }
    }
}
