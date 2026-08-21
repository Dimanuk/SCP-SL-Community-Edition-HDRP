using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin.Presets
{
    [CreateAssetMenu]
    public class ColorBlockPreset : ScriptableObject
    {
        [SerializeField]
        private ColorBlock _serializedColor = ColorBlock.defaultColorBlock;

        public ColorBlock Color => _serializedColor;
    }
}