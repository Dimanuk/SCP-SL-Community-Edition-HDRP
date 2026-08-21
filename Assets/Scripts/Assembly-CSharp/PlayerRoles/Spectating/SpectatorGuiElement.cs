namespace PlayerRoles.Spectating
{
    public class SpectatorGuiElement : global::UnityEngine.MonoBehaviour, global::UnityEngine.EventSystems.IPointerEnterHandler, global::UnityEngine.EventSystems.IEventSystemHandler, global::UnityEngine.EventSystems.IPointerExitHandler
    {
        private static global::PlayerRoles.Spectating.SpectatorGuiElement _lastHighlight;

        public static bool AnyHighlighted { get; private set; }

        public void OnPointerEnter(global::UnityEngine.EventSystems.PointerEventData eventData)
        {
            _lastHighlight = this;
            AnyHighlighted = true;
        }

        public void OnPointerExit(global::UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (!(_lastHighlight != this))
            {
                _lastHighlight = null;
                AnyHighlighted = false;
            }
        }

        private void OnDisable()
        {
            OnPointerExit(null);
        }

        private void OnDestroy()
        {
            OnPointerExit(null);
        }
    }
}
