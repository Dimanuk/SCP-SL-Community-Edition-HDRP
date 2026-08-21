using CustomPlayerEffects;
using UnityEngine;

namespace FacilitySoundtrack
{
    public abstract class SoundtrackLayerBase : MonoBehaviour
    {
        public abstract float Weight { get; }

        public abstract bool Additive { get; }

        public abstract void UpdateVolume(float volumeScale);
    }
}
