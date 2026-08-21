using Mirror;
using PlayerRoles.PlayableScps.HumeShield;
using UnityEngine;

namespace PlayerStatsSystem
{
    public class HumeShieldStat : SyncedStatBase
    {
        public override SyncMode Mode => SyncMode.PrivateAndSpectators;

        public override float MinValue => 0f;

        public override float MaxValue
        {
            get
            {
                if (base.Hub == null || base.Hub.roleManager == null)
                    return 0f;

                if (!TryGetHsModule(out var controller) || controller == null)
                    return 0f;

                return controller.HsMax;
            }
        }

        public override float CurValue
        {
            get => base.CurValue;
            set => base.CurValue = Mathf.Max(0f, value);
        }

        public override bool CheckDirty(float prevValue, float newValue)
        {
            return Mathf.CeilToInt(prevValue) != Mathf.CeilToInt(newValue);
        }

        public override float ReadValue(NetworkReader reader)
        {
            return (int)reader.ReadUShort();
        }

        public override void WriteValue(NetworkWriter writer)
        {
            int num = Mathf.Clamp(Mathf.CeilToInt(CurValue), 0, 65535);
            writer.WriteUShort((ushort)num);
        }

        internal override void Update()
        {
            base.Update();

            if (!NetworkServer.active || base.Hub == null || base.Hub.roleManager == null)
                return;

            if (!TryGetHsModule(out var controller) || controller == null || controller.HsRegeneration == 0f)
                return;

            float hsCurrent = controller.HsCurrent;
            float num = controller.HsRegeneration * Time.deltaTime;

            if (num > 0f)
            {
                if (hsCurrent < controller.HsMax)
                    CurValue = Mathf.MoveTowards(hsCurrent, controller.HsMax, num);
            }
            else if (hsCurrent > 0f)
            {
                CurValue = hsCurrent + num;
            }
        }

        internal override void ClassChanged()
        {
            base.ClassChanged();

            if (base.Hub == null || base.Hub.roleManager == null)
                return;

            if (base.Hub.roleManager.CurrentRole is not IHumeShieldedRole)
                CurValue = 0f;
        }

        protected override void OnValueChanged(float prevValue, float newValue)
        {
            if (base.Hub == null)
                return;

            if (TryGetHsModule(out var controller) && controller != null)
                controller.OnHsValueChanged(prevValue, newValue);
        }

        private bool TryGetHsModule(out HumeShieldModuleBase controller)
        {
            if (base.Hub == null || base.Hub.roleManager == null)
            {
                controller = null;
                return false;
            }

            if (base.Hub.roleManager.CurrentRole is IHumeShieldedRole humeShieldedRole)
            {
                controller = humeShieldedRole.HumeShieldModule;
                return controller != null;
            }

            controller = null;
            return false;
        }
    }
}