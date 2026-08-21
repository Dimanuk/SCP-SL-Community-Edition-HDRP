using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using UnityEngine;

namespace InventorySystem.Items.Firearms
{
    public class AutomaticFirearmAnimatorEvents : FirearmAnimatorEventsBase
    {
        private float _curGripStatus;
        private float _gripMoveSpeed;
        private float _prevAds;

        private void InsertMagazine()
        {
            if (!IsServerController)
                return;

            Firearm firearm = TargetFirearm;
            if (firearm == null)
                return;

            FirearmStatusFlags flags = firearm.Status.Flags;
            ushort curAmmo = firearm.OwnerInventory.GetCurAmmo(firearm.AmmoType);

            flags |= FirearmStatusFlags.MagazineInserted;

            byte toLoad = (byte)Mathf.Min(curAmmo, firearm.AmmoManagerModule.MaxAmmo - firearm.Status.Ammo);


            ModifyUserAmmo(-toLoad);

            firearm.Status = new FirearmStatus(
                (byte)(firearm.Status.Ammo + toLoad),
                flags,
                firearm.Status.Attachments);
        }

        private void RemoveMagazine()
        {
            if (!IsServerController)
                return;

            Firearm firearm = TargetFirearm;
            if (firearm == null)
                return;

            FirearmStatusFlags flags = firearm.Status.Flags;

            if (flags.HasFlagFast(FirearmStatusFlags.MagazineInserted))
            {
                int chambered = (firearm.AmmoManagerModule is AutomaticAmmoManager aam)
                    ? aam.ChamberedAmount
                    : 0;

                flags &= ~FirearmStatusFlags.MagazineInserted;

                int returnToUser = firearm.Status.Ammo - chambered;

                ModifyUserAmmo(returnToUser);

                firearm.Status = new FirearmStatus(
                    (byte)chambered,
                    flags,
                    firearm.Status.Attachments);
            }
        }

        private void RemoveMagazineOpenBolt()
        {
            if (!IsServerController)
                return;

            Firearm firearm = TargetFirearm;
            if (firearm == null)
                return;


            ModifyUserAmmo(firearm.Status.Ammo);

            firearm.Status = new FirearmStatus(
                0,
                firearm.Status.Flags & ~FirearmStatusFlags.MagazineInserted,
                firearm.Status.Attachments);
        }

        private void UseChargingHandle()
        {
            if (!IsServerController)
                return;

            Firearm firearm = TargetFirearm;
            if (firearm == null)
                return;

            FirearmStatusFlags flags = firearm.Status.Flags;

            flags |= FirearmStatusFlags.Chambered;
            flags |= FirearmStatusFlags.Cocked;


            firearm.Status = new FirearmStatus(
                firearm.Status.Ammo,
                flags,
                firearm.Status.Attachments);
        }

        private void UnloadChamberedBullet()
        {
            if (!IsServerController)
                return;

            Firearm firearm = TargetFirearm;
            if (firearm == null)
                return;

            FirearmStatusFlags flags = firearm.Status.Flags;

            if (firearm.Status.Ammo != 0 &&
                !flags.HasFlagFast(FirearmStatusFlags.MagazineInserted) &&
                flags.HasFlagFast(FirearmStatusFlags.Chambered))
            {
                if (firearm.AmmoManagerModule is AutomaticAmmoManager aam)
                {
                    ModifyUserAmmo(aam.ChamberedAmount);
                }

                flags &= ~FirearmStatusFlags.Chambered;
                flags |= FirearmStatusFlags.Cocked;


                firearm.Status = new FirearmStatus(0, flags, firearm.Status.Attachments);
            }
        }

        private void MarkAsEquipped()
        {
            Firearm firearm = TargetFirearm;
            if (firearm == null)
                return;

            if ((!firearm.IsLocalPlayer || !IsServerController) && firearm.EquipperModule is EventBasedEquipper eventBasedEquipper)
            {
                eventBasedEquipper.Equip();
            }
        }

        private void SetGripBlendSpeed(float speed)
        {
            if (IsServerController)
                return;

            Firearm firearm = TargetFirearm;
            if (firearm == null)
                return;

            float gripSpeedMod = AttachmentsUtils.AttachmentsValue(firearm, AttachmentParam.AdsSpeedMultiplier);

            _gripMoveSpeed = speed * gripSpeedMod;

            if (Mathf.Abs(speed) < 0.001f)
                return;

            bool force = Mathf.Abs(speed) >= 0.01f;
            RefreshAnim(force);
        }

        private void Update()
        {
            if (_gripMoveSpeed == 0f || IsServerController)
                return;

            RefreshAnim(false);

            if (Mathf.Abs(_gripMoveSpeed - _prevAds) > 0.01f)
            {
                RefreshAnim(true);
                _prevAds = _gripMoveSpeed;
            }
        }

        private void RefreshAnim(bool force)
        {
            Firearm firearm = TargetFirearm;
            if (firearm == null)
                return;

            _curGripStatus = Mathf.Clamp01(_curGripStatus + Time.deltaTime * _gripMoveSpeed);
            firearm.AnimSetFloat(FirearmAnimatorHashes.GripBlend, _curGripStatus);

            if (force)
            {
                if (firearm.HasViewmodel && firearm.ClientViewmodel != null)
                {
                    firearm.ClientViewmodel.AnimatorForceUpdate();
                }
            }
        }
    }
}