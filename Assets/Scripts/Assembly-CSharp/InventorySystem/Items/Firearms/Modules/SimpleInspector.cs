using System.Diagnostics;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
    public class SimpleInspector : IInspectorModule, IFirearmModuleBase
    {
        private const float MinimalAntispamCooldown = 0.2f;

        private readonly int _layer;
        private readonly Firearm _firearm;
        private readonly Stopwatch _cooldownStopwatch;

        public bool Standby => true;

        public bool CanInspect
        {
            get
            {
                if (_firearm.AdsModule == null || _firearm.AdsModule.ClientAdsAmount > 0f)
                    return false;

                if (_firearm.ActionModule == null || !_firearm.ActionModule.Standby)
                    return false;

                if (_firearm.AmmoManagerModule == null || !_firearm.AmmoManagerModule.Standby)
                    return false;

                if (!_firearm.EquipperModule.Standby)
                    return false;

                if (_firearm.ClientViewmodel is not AnimatedFirearmViewmodel animatedViewmodel)
                    return false;

                AnimatorStateInfo stateInfo = animatedViewmodel.GetAnimatorStateInfo(_layer);
                if (stateInfo.tagHash != FirearmAnimatorHashes.Idle)
                    return false;

                return _cooldownStopwatch.Elapsed.TotalSeconds >= MinimalAntispamCooldown;
            }
        }

        public SimpleInspector(Firearm selfRef, int animatorLayer)
        {
            _firearm = selfRef;
            _layer = animatorLayer;
            _cooldownStopwatch = new Stopwatch();
            _cooldownStopwatch.Start();
        }

        public void OnInspect()
        {
            if (_firearm.ClientViewmodel != null)
            {
                _firearm.ClientViewmodel.AnimatorSetTrigger(FirearmAnimatorHashes.Inspect);
            }

            _cooldownStopwatch.Restart();
        }
    }
}
