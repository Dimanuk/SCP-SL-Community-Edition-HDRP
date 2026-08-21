using InventorySystem.GUI;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
    public class DisruptorAds : StandardAds
    {
        private DisruptorAction Disruptor
        {
            get
            {
                if (Firearm.ActionModule is DisruptorAction disruptorAction)
                    return disruptorAction;

                return null;
            }
        }

        protected override bool ForceDisabled
        {
            get
            {
                if (Firearm.AmmoManagerModule == null || !Firearm.AmmoManagerModule.Standby)
                    return true;
                if (Firearm.EquipperModule == null || !Firearm.EquipperModule.Standby)
                    return true;

                DisruptorAction disruptor = Disruptor;
                if (disruptor == null)
                    return true;

                if (disruptor.PredictedStatus.Ammo != 0)
                    return false;

                return disruptor.TimeSinceLastShot > disruptor.ShotDelay + 0.1f;
            }
        }

        protected override bool AllowChange
        {
            get
            {
                if (!InventoryGuiController.ItemsSafeForInteraction)
                    return Firearm.IsSpectated;

                DisruptorAction disruptor = Disruptor;
                if (disruptor == null)
                    return Firearm.IsSpectated;

                if (disruptor.ShotTriggered)
                    return Firearm.IsSpectated;

                return disruptor.TimeSinceLastShot > disruptor.ShotDelay + 0.1f || Firearm.IsSpectated;
            }
        }

        public DisruptorAds(Firearm selfRef, ushort serial, float defaultAdsTime, int adsLayer, byte adsInClip, byte adsOutClip)
            : base(selfRef, serial, defaultAdsTime, adsLayer, adsInClip, adsOutClip)
        {
        }
    }
}