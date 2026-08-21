using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OperationalGuide
{

    public class AbilityColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI References")]
        public GameObject ChildTextParent;
        public Image AbilityIcon;

        [Header("Color Settings")]
        public Color SelectedColor;
        public Color DefaultColor;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (AbilityIcon == null) return;
            AbilityIcon.color = SelectedColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (ChildTextParent != null && !ChildTextParent.activeInHierarchy)
            {
                if (AbilityIcon != null)
                    AbilityIcon.color = DefaultColor;
            }
        }
    }
}