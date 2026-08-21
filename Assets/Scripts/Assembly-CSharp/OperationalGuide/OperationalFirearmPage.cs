using UnityEngine;

namespace OperationalGuide
{
    public class OperationalFirearmPage : OperationalPannablePage
    {
        public FirearmObject FirearmObject;
        public ItemType DefaultItem;

        public override void ToggleDescriptionMenu()
        {
            if (DescriptionPage == null)
                return;

            if (PageAnimator != null)
            {
                F1PageActive = !F1PageActive;

                PageAnimator.SetBool("main", F1PageActive);
            }

            if (FirearmObject != null)
            {
                FirearmObject.ItemHeld = DefaultItem;
            }
            TurnOn();
        }

        public virtual void UpdateHeldItem(int item)
        {
            if (FirearmObject != null)
            {
                FirearmObject.ItemHeld = (ItemType)item;
                DefaultItem = FirearmObject.ItemHeld;
            }
        }
    }
}
