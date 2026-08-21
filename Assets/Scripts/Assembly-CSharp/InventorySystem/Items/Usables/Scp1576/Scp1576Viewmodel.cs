using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Utils.NonAllocLINQ;
using AudioPooling;
using static InventorySystem.Items.Usables.StatusMessage;

namespace InventorySystem.Items.Usables.Scp1576
{
    public class Scp1576Viewmodel : UsableItemViewmodel
    {
        private const ItemType Scp1576Type = ItemType.SCP1576;

        private static readonly Dictionary<ushort, float> PrevWeights = new Dictionary<ushort, float>();
        private static readonly Dictionary<ushort, Stopwatch> TimersBySerial = new Dictionary<ushort, Stopwatch>();

        private static float _cachedUseTime;
        private static bool _useTimeCacheSet;

        private bool _wasCranking;

        [SerializeField]
        private int _posLayer;

        [SerializeField]
        private Material _beltMaterial;

        [SerializeField]
        private Vector2 _beltSpeed;

        [SerializeField]
        private ParticleSystem _particles;

        [SerializeField]
        private AudioSource _audioLoop;

        [SerializeField]
        private AudioClip _endRecordClip;

        [SerializeField]
        private AudioClip _rewindClip;

        [SerializeField]
        private AudioClip _startRecording;

        [SerializeField]
        private Scp1576Source _playbackSource;

        private static float UseTime
        {
            get
            {
                if (_useTimeCacheSet)
                    return _cachedUseTime;

                if (InventoryItemLoader.AvailableItems.TryGetValue(Scp1576Type, out ItemBase item) && item is UsableItem usable)
                {
                    _cachedUseTime = usable.UseTime;
                    _useTimeCacheSet = true;
                    return _cachedUseTime;
                }

                return 0f;
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (!PrevWeights.TryGetValue(ItemId.SerialNumber, out float prevWeight))
                prevWeight = 0f;

            if (TimersBySerial.TryGetValue(ItemId.SerialNumber, out Stopwatch timer) && timer != null && timer.IsRunning)
            {
                float elapsed = (float)timer.Elapsed.TotalSeconds;
                float useTime = UseTime;

                if (elapsed > useTime)
                {
                    if (_wasCranking)
                    {
                        _particles.Play();
                        PlaySound(_startRecording);
                        _playbackSource.enabled = _playbackSource.enabled || IsLocal;
                        _wasCranking = false;
                        Scp1576Item.LocallyUsed = Scp1576Item.LocallyUsed || IsLocal;
                    }

                    float remaining = elapsed - useTime;
                    _audioLoop.volume = remaining;
                    float scroll = remaining / 30f;
                    _beltMaterial.mainTextureOffset += _beltSpeed * Time.deltaTime;
                    prevWeight = Mathf.Clamp01(scroll);
                }
                else if (elapsed > Scp1576Item.HornReturnDelay)
                {
                    _wasCranking = true;

                    if (prevWeight > 0f)
                    {
                        prevWeight -= Time.deltaTime * Scp1576Item.HornReturnSpeed;
                        if (prevWeight <= 0f)
                            PlaySound(_rewindClip);
                    }
                }

                prevWeight = Mathf.Clamp01(prevWeight);
                PrevWeights[ItemId.SerialNumber] = prevWeight;
            }
            else
            {
                if (_particles.isPlaying)
                    _particles.Stop();

                if (_audioLoop.volume > 0f)
                    PlaySound(_endRecordClip);

                _audioLoop.volume = 0f;
                _playbackSource.enabled = false;
                Scp1576Item.LocallyUsed = false;
            }

            AnimatorSetLayerWeight(_posLayer, prevWeight);
        }

        private static void PlaySound(AudioClip clip)
        {
            AudioSourcePoolManager.PlaySound(clip, (Transform)null, 1f, 1f, FalloffType.Exponential, AudioMixerChannelType.DefaultSfx, 0f);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            UsableItemsController.OnClientStatusReceived += OnClientStatusReceived;
            Scp1576Pickup.OnHornPositionUpdated += OnHornPositionUpdated;
        }

        private static void OnHornPositionUpdated(ushort serial, float pos)
        {
            PrevWeights[serial] = pos;
        }

        private static void OnClientStatusReceived(StatusMessage msg)
        {
            if (msg.Status == StatusType.Start)
            {
                ushort serial = msg.ItemSerial;

                if (ReferenceHub.AllHubs.Any(x => x.inventory.CurItem.SerialNumber == serial && x.inventory.CurItem.TypeId == Scp1576Type))
                {
                    if (!TimersBySerial.TryGetValue(serial, out Stopwatch timer))
                    {
                        timer = new Stopwatch();
                        TimersBySerial[serial] = timer;
                    }
                    timer.Restart();
                }
            }
            else if (msg.Status == StatusType.Cancel)
            {
                if (TimersBySerial.TryGetValue(msg.ItemSerial, out Stopwatch timer))
                    timer.Reset();
            }
        }
    }
}