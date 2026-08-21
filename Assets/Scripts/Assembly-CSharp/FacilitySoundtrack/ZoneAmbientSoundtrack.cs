using System;
using MapGeneration;
using PlayerRoles.Spectating;
using UnityEngine;

namespace FacilitySoundtrack
{
    public class ZoneAmbientSoundtrack : SoundtrackLayerBase
    {
        [Serializable]
        private class ZoneSoundtrack
        {
            public FacilityZone TargetZone;

            public AudioSource Source;

            public float VolumeScale;

            public float CrossfadeVolume { get; set; }
        }

        [SerializeField]
        private float _fadeInSpeed;

        [SerializeField]
        private float _fadeOutSpeed;

        [SerializeField]
        private float _crossfadeSpeed;

        [SerializeField]
        private ZoneSoundtrack[] _zoneSoundtracks;

        private float _weight;

        private FacilityZone _lastZone;

        private bool IsMuted
        {
            get
            {
                if (SpectatorTargetTracker.TryGetTrackedPlayer(out var hub))
                {
                    return IsMutedForPlayer(hub);
                }
                if (!ReferenceHub.TryGetLocalHub(out var hub2))
                {
                    return false;
                }
                return IsMutedForPlayer(hub2);
            }
        }

        public override bool Additive => true;

        public override float Weight => _weight;

        public override void UpdateVolume(float masterScale)
        {
            RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPosition(MainCameraController.CurrentCamera.position);
            if (roomIdentifier != null)
            {
                _lastZone = roomIdentifier.Zone;
            }
            float num = (IsMuted ? (0f - _fadeOutSpeed) : _fadeInSpeed);
            _weight = Mathf.Clamp01(_weight + num * Time.deltaTime);
            ZoneSoundtrack[] zoneSoundtracks = _zoneSoundtracks;
            foreach (ZoneSoundtrack zoneSoundtrack in zoneSoundtracks)
            {
                float target = ((zoneSoundtrack.TargetZone == _lastZone) ? 1 : 0);
                zoneSoundtrack.CrossfadeVolume = Mathf.MoveTowards(zoneSoundtrack.CrossfadeVolume, target, _crossfadeSpeed * Time.deltaTime);
                zoneSoundtrack.Source.volume = zoneSoundtrack.CrossfadeVolume * zoneSoundtrack.VolumeScale * masterScale;
            }
        }

        private bool IsMutedForPlayer(ReferenceHub hub)
        {
            global::CustomPlayerEffects.StatusEffectBase[] allEffects = hub.playerEffectsController.AllEffects;
            for (int i = 0; i < allEffects.Length; i++)
            {
                if (allEffects[i] is global::CustomPlayerEffects.ISoundtrackMutingEffect soundtrackMutingEffect && soundtrackMutingEffect.MuteSoundtrack)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
