using UnityEngine;

namespace CustomCulling
{
    public class NoClipCamExtraPoint : MonoBehaviour
    {
        private CullableRoom _targetParentRoom;
        private bool _parentSet;

        public CullableRoom CullableRoom
        {
            get
            {
                if (!_parentSet)
                {
                    _targetParentRoom = GetComponentInParent<CullableRoom>();
                    _parentSet = true;
                }

                return _targetParentRoom;
            }
        }
    }
}