using System;
using UnityEngine;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;

namespace InventorySystem.Items.Firearms
{
    public class RevolverViewmodel : AnimatedFirearmViewmodel
    {
        private enum RevolverLayer
        {
            MainAnims = 0,
            Ads = 1,
            Unloaded = 2,
            Cylinder = 3,
            DryFire = 4,
            Cocked = 5,
            Idle = 6
        }

        [Serializable]
        private struct RevolverPrimer
        {
            [SerializeField]
            private GameObject _rootObject;

            [SerializeField]
            private int _bulletsAmount;

            [SerializeField]
            private GameObject[] _primers;

            [SerializeField]
            private int _absoluteOffset;

            public void Refresh(int targetCylinder, int remainingBullets, int offset)
            {
                _rootObject.SetActive(remainingBullets == _bulletsAmount);

                if (remainingBullets != _bulletsAmount || _bulletsAmount <= 0)
                    return;

                for (int i = 0; i < _bulletsAmount; i++)
                {
                    int raw = _absoluteOffset + offset + i;
                    int idx = raw >= _bulletsAmount ? raw - _bulletsAmount : raw;
                    if (idx < 0)
                        idx += _bulletsAmount;

                    _primers[idx].SetActive(i < _bulletsAmount - targetCylinder);
                }
            }
        }

        [SerializeField]
        private Transform _swayPivot;

        [SerializeField]
        private RevolverPrimer[] _firedPrimers;

        private Revolver _revolver;

        private byte _unfiredBulletsRemaining;

        private bool _wasCocked;

        private static int ReloadTagHash => FirearmAnimatorHashes.Reload;

        internal override void OnEquipped()
        {
            base.OnEquipped();
            _revolver = base.ParentItem as Revolver;

            if (IsSpectator)
            {
                AnimatorSetFloat(FirearmAnimatorHashes.Random, UnityEngine.Random.value);
            }
        }

        private void RefreshPrimers(int offset)
        {
            if (_revolver == null || _firedPrimers == null)
                return;

            byte remainingBullets = _revolver.AmmoManagerModule.MaxAmmo;

            for (int i = 0; i < _firedPrimers.Length; i++)
                _firedPrimers[i].Refresh(_unfiredBulletsRemaining, remainingBullets, offset);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (_revolver == null)
                return;

            AnimatorStateInfo stateInfo = GetAnimatorStateInfo(0);
            bool hasMag = _revolver.Status.Flags.HasFlagFast(FirearmStatusFlags.MagazineInserted);

            bool reloadFirstHalf = stateInfo.tagHash == ReloadTagHash && stateInfo.normalizedTime < 0.5f;

            SetLayerWeight(RevolverLayer.Unloaded, (reloadFirstHalf || hasMag) ? 0f : 1f);

            if (!(_revolver.ActionModule is DoubleAction doubleAction))
                return;

            bool cocked = doubleAction.Cocked;

            if (hasMag && cocked)
                _wasCocked = true;

            if (!reloadFirstHalf)
            {
                if (!cocked)
                    _wasCocked = false;

                _unfiredBulletsRemaining = _revolver.Status.Ammo;
            }
            else if (!hasMag)
            {
                _unfiredBulletsRemaining = _revolver.AmmoManagerModule.MaxAmmo;
            }

            RefreshPrimers(_wasCocked ? 1 : 0);

            float cockedWeight = doubleAction.Standby ? (cocked ? 1f : 0f) : doubleAction.HammerPosition;
            SetLayerWeight(RevolverLayer.Cocked, cockedWeight);

            SetLayerWeight(RevolverLayer.Cylinder, doubleAction.HammerPosition / _revolver.AmmoManagerModule.MaxAmmo);
        }

        private void SetLayerWeight(RevolverLayer layer, float val)
        {
            base.AnimatorSetLayerWeight((int)layer, val);
        }
    }
}
