namespace Interactables.Interobjects.DoorUtils
{
    [global::UnityEngine.CreateAssetMenu(fileName = "New Panel Visual Settings", menuName = "ScriptableObject/Doors/PanelVisualSettings")]
    public class PanelVisualSettings : global::UnityEngine.ScriptableObject
    {
        public global::UnityEngine.Material PanelOpenMat;

        public global::UnityEngine.Material PanelClosedMat;

        public global::UnityEngine.Material PanelMovingMat;

        public global::UnityEngine.Material PanelErrorMat;

        public global::UnityEngine.Material PanelDeniedMat;

        public const string DoorTranslationKey = "Doors";

        [global::UnityEngine.SerializeField]
        private int _textOpenTranslationId;

        [global::UnityEngine.SerializeField]
        private int _textClosedTranslationId;

        [global::UnityEngine.SerializeField]
        private int _textMovingTranslationId;

        [global::UnityEngine.SerializeField]
        private int _textLockedDownTranslationId;

        [global::UnityEngine.SerializeField]
        private int _textErrorTranslationId;

        [global::UnityEngine.SerializeField]
        private int _textDeniedTranslationId;

        private string _textOpenCache;

        private string _textClosedCache;

        private string _textMovingCache;

        private string _textLockedDownCache;

        private string _textErrorCache;

        private string _textDeniedCache;

        private bool _textOpenSet;

        private bool _textClosedSet;

        private bool _textMovingSet;

        private bool _textLockedDownSet;

        private bool _textErrorSet;

        private bool _textDeniedSet;

        public string TextOpen
        {
            get
            {
                if (!_textOpenSet)
                {
                    _textOpenCache = TranslationReader.Get("Doors", _textOpenTranslationId, "<color=#00ff00>OPEN</color>");
                    _textOpenSet = true;
                }
                return _textOpenCache;
            }
        }

        public string TextClosed
        {
            get
            {
                if (!_textClosedSet)
                {
                    _textClosedCache = TranslationReader.Get("Doors", _textClosedTranslationId, "<color=#07A2FE>CLOSED</color>");
                    _textClosedSet = true;
                }
                return _textClosedCache;
            }
        }

        public string TextMoving
        {
            get
            {
                if (!_textMovingSet)
                {
                    _textMovingCache = TranslationReader.Get("Doors", _textMovingTranslationId, "<color=#FFA600>MOVING</color>");
                    _textMovingSet = true;
                }
                return _textMovingCache;
            }
        }

        public string TextLockedDown
        {
            get
            {
                if (!_textLockedDownSet)
                {
                    _textLockedDownCache = TranslationReader.Get("Doors", _textLockedDownTranslationId, "<color=#FF0000>LOCKDOWN</color>");
                    _textLockedDownSet = true;
                }
                return _textLockedDownCache;
            }
        }

        public string TextError
        {
            get
            {
                if (!_textErrorSet)
                {
                    _textErrorCache = TranslationReader.Get("Doors", _textErrorTranslationId, "<color=#FF0000>ERROR</color>");
                    _textErrorSet = true;
                }
                return _textErrorCache;
            }
        }

        public string TextDenied
        {
            get
            {
                if (!_textDeniedSet)
                {
                    _textDeniedCache = TranslationReader.Get("Doors", _textDeniedTranslationId, "<color=#FF0000>ACCESS DENIED</color>");
                    _textDeniedSet = true;
                }
                return _textDeniedCache;
            }
        }

        public void Reset()
        {
            _textOpenSet = false;
            _textClosedSet = false;
            _textMovingSet = false;
            _textLockedDownSet = false;
            _textErrorSet = false;
            _textDeniedSet = false;
        }
    }
}
