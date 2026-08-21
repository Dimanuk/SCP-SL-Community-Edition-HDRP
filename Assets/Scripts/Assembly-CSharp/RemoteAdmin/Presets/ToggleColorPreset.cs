using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin.Presets
{
    [CreateAssetMenu]
    public class ToggleColorPreset : ScriptableObject
    {
        [SerializeField]
        private ColorBlock _selected;

        [Space]
        [SerializeField]
        private ColorBlock _unselected;

        public ColorBlock Selected => _selected;
        public ColorBlock Unselected => _unselected;

        public ToggleColorPreset()
        {
            _selected = ColorBlock.defaultColorBlock;
            _unselected = ColorBlock.defaultColorBlock;
        }
    }
}