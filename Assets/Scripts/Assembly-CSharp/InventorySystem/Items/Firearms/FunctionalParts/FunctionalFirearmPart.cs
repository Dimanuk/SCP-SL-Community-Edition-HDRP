using UnityEngine;

namespace InventorySystem.Items.Firearms.FunctionalParts
{
    public class FunctionalFirearmPart : MonoBehaviour
    {
        private Firearm _fa;

        private bool _faSet;

        [SerializeField]
        private AnimatedFirearmViewmodel _viewmodel;

        protected Firearm Firearm
        {
            get
            {
                if (!_faSet)
                {
                    if (_viewmodel == null)
                        return null;

                    ItemBase parentItem = _viewmodel.ParentItem;
                    _fa = parentItem as Firearm;

                    if (_fa == null)
                        _fa = _viewmodel.GetComponentInParent<Firearm>();

                    _faSet = true;
                }

                return _fa;
            }
        }
    }
}