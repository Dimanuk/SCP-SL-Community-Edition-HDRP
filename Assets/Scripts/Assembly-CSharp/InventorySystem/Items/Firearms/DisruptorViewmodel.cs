using UnityEngine;
using InventorySystem.Items.Firearms.Modules;

namespace InventorySystem.Items.Firearms
{
    public class DisruptorViewmodel : AnimatedFirearmViewmodel
    {
        private enum DisruptorLayer
        {
            MainAnims = 0,
            Ads = 1,
            NoMag = 2,
            Trigger = 3,
            Idle = 4
        }

        private ParticleDisruptor _firearm;

        private float _triggerSmooth;

        internal override void OnEquipped()
        {
            base.OnEquipped();
            _firearm = ParentItem as ParticleDisruptor;
        }

        protected override void OnShot()
        {
            // base.OnShot() is what actually sets the Fire trigger (and the shared-hands
            // trigger). It's the only signal non-shooter clients — spectators watching this
            // player in first person — ever get, since they never run DoClientsideAction's
            // predictive AnimatorSetTrigger call. Skipping it left the shot/recharge animation
            // entirely invisible to anyone but the shooter.
            base.OnShot();

            // Fast-forward past the charge-up portion of the clip (stepped, not one big jump —
            // fastMode:false), since by the time this fires the charge already finished.
            AnimatorForceUpdate((_firearm?.ActionModule as DisruptorAction)?.ShotDelay ?? 0f, false);

            _triggerSmooth = 1f;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (_firearm == null)
                return;

            IAdsModule adsModule = _firearm.AdsModule;
            if (adsModule != null)
            {
                SetLayerWeight(DisruptorLayer.Ads, adsModule.ClientAdsAmount);
            }

            IActionModule actionModule = _firearm.ActionModule;
            if (actionModule != null)
            {
                float val = (actionModule.PredictedStatus.Ammo == 0) ? 1f : 0f;
                SetLayerWeight(DisruptorLayer.NoMag, val);
            }

            _triggerSmooth = Mathf.MoveTowards(_triggerSmooth, 0f, Time.deltaTime * 2f);
            SetLayerWeight(DisruptorLayer.Trigger, _triggerSmooth);
        }

        private void SetLayerWeight(DisruptorLayer layer, float val)
        {
            if (ViewmodelAnimator != null)
            {
                ViewmodelAnimator.SetLayerWeight((int)layer, val);
            }
        }
    }
}