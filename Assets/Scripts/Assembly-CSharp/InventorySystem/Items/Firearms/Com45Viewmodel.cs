using UnityEngine;
using InventorySystem.Items.Firearms.Modules;

namespace InventorySystem.Items.Firearms
{
    public class Com45Viewmodel : ClosedBoltFirearmViewmodel
    {
        private const int AdsAnimCount = 3;

        private static readonly int RandomAdsHash = Animator.StringToHash("RandomAds");

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (ParentItem is AutomaticFirearm automaticFirearm)
            {
                IAdsModule adsModule = automaticFirearm.AdsModule;
                if (adsModule != null && adsModule.ClientAdsAmount <= 0f)
                {
                    int randomValue = Random.Range(0, AdsAnimCount);
                    AnimatorSetFloat(RandomAdsHash, (float)randomValue);
                }
            }
        }
    }
}