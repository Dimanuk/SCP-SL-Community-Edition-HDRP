using UnityEngine;

namespace RemoteAdmin.Presets
{
    [CreateAssetMenu]
    public class ColorPreset : ScriptableObject
    {
        [SerializeField]
        private Color _serializedColor = Color.gray;

        public Color Color => _serializedColor;
    }
}