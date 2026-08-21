using AudioPooling;
using Mirror;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace InventorySystem.Items.Firearms
{
	public static class FirearmAudioManager
	{
        [global::System.Serializable]
        private class AudibleShooterInfo
        {
            private readonly ReferenceHub _hub;

            private readonly global::System.Collections.Generic.Queue<global::InventorySystem.Items.Firearms.FirearmAudioClip> _remainingClips;

            private readonly global::System.Diagnostics.Stopwatch _lastShotStopwatch;

            private bool _isEmpty;

            private const float MinimalCooldown = 0.05f;

            private const int MaxQueueCount = 7;

            public AudibleShooterInfo(ReferenceHub hub)
            {
                _hub = hub;
                _remainingClips = new global::System.Collections.Generic.Queue<global::InventorySystem.Items.Firearms.FirearmAudioClip>();
                _lastShotStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            }

            public void Enqueue(global::UnityEngine.AudioClip clip, float distance, bool useDedicatedChannel)
            {
                if (_remainingClips.Count < 7)
                {
                    distance = global::UnityEngine.Mathf.Abs(distance);
                    if (useDedicatedChannel)
                    {
                        distance = 0f - distance;
                    }
                    _isEmpty = false;
                    _remainingClips.Enqueue(new global::InventorySystem.Items.Firearms.FirearmAudioClip
                    {
                        MaxDistance = distance,
                        Sound = clip
                    });
                }
            }

            public void Play()
            {
                if (!_isEmpty && !(_lastShotStopwatch.Elapsed.TotalSeconds < 0.05000000074505806))
                {
                    if (!CollectionExtensions.TryDequeue(_remainingClips, out var element))
                    {
                        _isEmpty = true;
                    }
                    else if (!(_hub == null))
                    {
                        global::AudioPooling.AudioMixerChannelType channel = ((element.MaxDistance < 0f) ? global::AudioPooling.AudioMixerChannelType.Weapons : global::AudioPooling.AudioMixerChannelType.DefaultSfx);
                        global::AudioPooling.AudioSourcePoolManager.PlaySound(element.Sound, _hub.transform, global::UnityEngine.Mathf.Abs(element.MaxDistance), 1f, FalloffType.Exponential, channel);
                    }
                }
            }
        }

        private static readonly global::System.Collections.Generic.Dictionary<uint, global::InventorySystem.Items.Firearms.FirearmAudioManager.AudibleShooterInfo> Shooters = new global::System.Collections.Generic.Dictionary<uint, global::InventorySystem.Items.Firearms.FirearmAudioManager.AudibleShooterInfo>();

        public static event Action<ReferenceHub, ItemType, FirearmAudioClip> OnAudioReceived;

        public static event Action<GunAudioMessage> OnMessageReceived;

		[RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::Mirror.NetworkServer.ReplaceHandler<global::InventorySystem.Items.Firearms.GunAudioMessage>((conn, msg) => { });
            CustomNetworkManager.OnClientReady += RegisterHandler;
            StaticUnityMethods.OnLateUpdate += LateUpdate;
        }

        private static void RegisterHandler()
        {
            global::Mirror.NetworkClient.ReplaceHandler<global::InventorySystem.Items.Firearms.GunAudioMessage>(ClientAudioReceived);
            Shooters.Clear();
        }

        private static void ClientAudioReceived(GunAudioMessage msg)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(msg.Weapon, out ItemBase itemBase) || !(itemBase is Firearm firearm))
                return;

            OnMessageReceived?.Invoke(msg);

            if (msg.AudioClipId >= firearm.AudioClips.Length)
                return;

            FirearmAudioClip clip = firearm.AudioClips[msg.AudioClipId];
            bool useDedicated = clip.HasFlag(FirearmAudioFlags.UseDedicatedAudioChannel);

            if (msg.ShooterHub == null)
            {
                AudioSourcePoolManager.PlaySound(
                    clip.Sound,
                    msg.ShooterPosition,
                    (float)msg.MaxDistance,
                    1f,
                    FalloffType.Linear,
                    AudioMixerChannelType.Weapons,
                    1f,
                    false
                );
            }
            else
            {
                OnAudioReceived?.Invoke(msg.ShooterHub, msg.Weapon, clip);

                if (!Shooters.TryGetValue(msg.ShooterHub.netId, out AudibleShooterInfo shooterInfo))
                {
                    shooterInfo = new AudibleShooterInfo(msg.ShooterHub);
                    Shooters[msg.ShooterHub.netId] = shooterInfo;
                }

                shooterInfo.Enqueue(clip.Sound, (float)msg.MaxDistance, useDedicated);
            }
        }

        private static void LateUpdate()
        {
            if (!StaticUnityMethods.IsPlaying)
            {
                return;
            }
            foreach (global::System.Collections.Generic.KeyValuePair<uint, global::InventorySystem.Items.Firearms.FirearmAudioManager.AudibleShooterInfo> shooter in Shooters)
            {
                shooter.Value.Play();
            }
        }


        public static void Serialize(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Firearms.GunAudioMessage value)
        {
            value.Serialize(writer);
        }

        public static global::InventorySystem.Items.Firearms.GunAudioMessage Deserialize(this global::Mirror.NetworkReader reader)
        {
            global::InventorySystem.Items.Firearms.GunAudioMessage result = default(global::InventorySystem.Items.Firearms.GunAudioMessage);
            result.Deserialize(reader);
            return result;
        }
	}
}
