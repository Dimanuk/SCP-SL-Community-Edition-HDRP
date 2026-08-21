using System;
using InventorySystem.Items.Firearms.Modules;
using UnityEngine;

namespace InventorySystem.Items.Firearms
{
    public class ShotgunViewmodel : AnimatedFirearmViewmodel
    {
        [SerializeField]
        private GameObject[] _loadedShells;

        [SerializeField]
        private float _recoilPerShot;

        [SerializeField]
        private float _recoilOffset;

        private Shotgun _shotgun;

        private float _triggerSmooth;

        private const int ShootLayer = 1;

        private const int TriggerLayer = 4;

        private PumpAction PumpAction
        {
            get
            {
                if (_shotgun == null) return null;
                return _shotgun.ActionModule as PumpAction;
            }
        }

        public override void InitAny()
        {
            base.InitAny();
            _shotgun = base.ParentItem as Shotgun;

            if (_shotgun != null)
            {
                _shotgun.OnShotCalled += OnFire;
            }
        }

        // The trigger layer chases the trigger state at fixed rates (pull is much faster than
        // release), the shoot layer is slammed to full weight per shot in OnFire.
        private const float TriggerPullSpeed = 17f;
        private const float TriggerReleaseSpeed = 8f;

        protected override void LateUpdate()
        {
            if (_shotgun == null) return;
            base.LateUpdate();

            PumpAction pump = this.PumpAction;
            if (pump == null)
                return;

            float rate = pump.IsTriggerHeld ? TriggerPullSpeed : -TriggerReleaseSpeed;
            _triggerSmooth = Mathf.Clamp01(_triggerSmooth + rate * Time.deltaTime);
            base.AnimatorSetLayerWeight(TriggerLayer, _triggerSmooth);

            if (_loadedShells != null)
            {
                int visibleInMag = pump.PredictedStatus.Ammo - pump.ChamberedRounds;

                for (int i = 0; i < _loadedShells.Length; i++)
                {
                    if (_loadedShells[i] != null)
                    {
                        _loadedShells[i].SetActive(i < visibleInMag);
                    }
                }
            }
        }

        private void OnFire()
        {
            PumpAction pump = this.PumpAction;
            if (pump == null)
                return;

            float weight = pump.LastFiredAmount * _recoilPerShot + _recoilOffset;
            base.AnimatorSetLayerWeight(ShootLayer, Mathf.Max(weight, 1f));
        }
    }
}