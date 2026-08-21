using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioPooling
{
    public class AudioSourcePoolManager : MonoBehaviour
    {
        [SerializeField]
        private CurvePreset[] _curves;

        [SerializeField]
        private ChannelPreset[] _channels;

        private static bool _initialized;
        private static AudioSourcePoolManager _singleton;

        private static readonly HashSet<AudioSource> Instances = new HashSet<AudioSource>();
        private static readonly Dictionary<FalloffType, AnimationCurve> Curves = new Dictionary<FalloffType, AnimationCurve>();
        private static readonly Dictionary<AudioMixerChannelType, AudioMixerGroup> Channels = new Dictionary<AudioMixerChannelType, AudioMixerGroup>();

        private void Awake()
        {
            _singleton = this;
            _initialized = true;

            Instances.Clear();
            Curves.Clear();
            Channels.Clear();

            if (_curves != null)
            {
                foreach (CurvePreset preset in _curves)
                {
                    Curves[preset.Type] = preset.FalloffCurve;
                }
            }

            if (_channels != null)
            {
                foreach (ChannelPreset preset in _channels)
                {
                    Channels[preset.Type] = preset.Group;
                }
            }
        }

        private void OnDestroy()
        {
            _initialized = false;
            _singleton = null;
        }

        public static AudioSource PlaySound(
            AudioClip sound,
            Vector3 position,
            float maxDistance,
            float volume = 1f,
            FalloffType falloffType = FalloffType.Exponential,
            AudioMixerChannelType channel = AudioMixerChannelType.DefaultSfx,
            float spatial = 1f,
            bool reserved = false)
        {
            AudioSource source = PreSetupSource(sound, maxDistance, falloffType, channel, volume, spatial, reserved);
            if (source == null)
                return null;

            source.transform.SetParent(_singleton.transform);
            source.transform.position = position;
            source.Play();
            return source;
        }

        public static AudioSource PlaySound(
            AudioClip sound,
            RelativePositioning.RelativePosition relativePosition,
            float maxDistance,
            float volume = 1f,
            FalloffType falloffType = FalloffType.Exponential,
            AudioMixerChannelType channel = AudioMixerChannelType.DefaultSfx,
            float spatial = 1f,
            bool reserved = false)
        {
            AudioSource source = PlaySound(sound, relativePosition.Position, maxDistance, volume, falloffType, channel, spatial, reserved);
            if (source == null)
                return null;

            if (RelativePositioning.WaypointBase.TryGetWaypoint(relativePosition.WaypointId, out var waypoint))
            {
                source.transform.SetParent(waypoint.transform);
            }

            return source;
        }

        public static AudioSource PlaySound(
            AudioClip sound,
            Transform trackedObject,
            float maxDistance,
            float volume = 1f,
            FalloffType falloffType = FalloffType.Exponential,
            AudioMixerChannelType channel = AudioMixerChannelType.DefaultSfx,
            float spatial = 1f,
            bool reserved = false)
        {
            AudioSource source = PreSetupSource(sound, maxDistance, falloffType, channel, volume, spatial, reserved);
            if (source == null)
                return null;

            source.transform.SetParent(trackedObject);
            source.transform.localPosition = Vector3.zero;
            source.Play();
            return source;
        }

        public static AudioSource GetFree(bool reserved)
        {
            if (!reserved)
            {
                foreach (AudioSource instance in Instances)
                {
                    if (instance != null && !instance.isPlaying)
                    {
                        return instance;
                    }
                }
            }

            GameObject go = new GameObject("PooledAudioSource");
            AudioSource source = go.AddComponent<AudioSource>();

            if (!reserved)
            {
                Instances.Add(source);
            }

            return source;
        }

        private static AudioSource PreSetupSource(
            AudioClip sound,
            float maxDistance,
            FalloffType falloffType,
            AudioMixerChannelType channel,
            float volume,
            float spatial,
            bool reserved)
        {
            if (!_initialized || _singleton == null)
                return null;

            AudioSource source = GetFree(reserved);
            if (source == null)
                return null;

            if (!Curves.TryGetValue(falloffType, out AnimationCurve curve))
            {
                throw new Exception($"Curve for falloff type \"{falloffType}\" is not defined in the AudioSourcePoolManager.");
            }

            if (!Channels.TryGetValue(channel, out AudioMixerGroup mixerGroup))
            {
                throw new Exception($"Channel \"{channel}\" is not defined in the AudioSourcePoolManager.");
            }

            source.enabled = true;
            source.playOnAwake = false;
            source.loop = false;
            source.dopplerLevel = 0f;
            source.volume = volume;
            source.spatialBlend = spatial;
            source.rolloffMode = AudioRolloffMode.Custom;
            source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
            source.maxDistance = maxDistance;
            source.outputAudioMixerGroup = mixerGroup;
            source.clip = sound;
            source.reverbZoneMix = 1f;
            source.pitch = 1f;

            return source;
        }
    }
}