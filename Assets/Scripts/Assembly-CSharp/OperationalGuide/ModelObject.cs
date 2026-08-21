using InventorySystem.Items;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace OperationalGuide
{

    public class ModelObject : PannableObject
    {
        public HumanCharacterModel HCM;

        [Header("Item Configuration")]
        [Tooltip("Тип предмета, привязанный к данной модели.")]
        public ItemType ItemHeld = ItemType.None; 

        public override void OnPannableEnable()
        {
            YAxisModifer = 0f;
        }

        public override void OnUpdate()
        {
            if (HCM != null)
            {
                HCM.UpdateHeldItem(ItemIdentifier.None);
            }
        }
    }
}