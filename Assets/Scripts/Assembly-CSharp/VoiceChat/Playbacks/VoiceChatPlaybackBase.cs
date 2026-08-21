using System;
using System.Runtime.CompilerServices;

using UnityEngine;

namespace VoiceChat.Playbacks
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class VoiceChatPlaybackBase : MonoBehaviour
    {
        public float VolumeScale = 1f;

        public int Channels = 1;

        public int _isPlayingSustainRemFrames;

        public float _collectedLoudness;

        public int _collectedSamples;

        public float _targetLoudness;

        public const int LoudnessCollectorThreshold = 1200;

        public const float LoudnessCollectorMultiplier = 5f;

        public const float LoudnessLerpSpeed = 40f;

        public const int IsPlayingSustainFrames = 15;

        public static int _flatlinePcmLen = 0;

        public static float[] _flatlinePcm = Array.Empty<float>();

        public AudioSource Source { get; set; }

        public float Loudness { get; set; }

        public abstract int MaxSamples { get; }

        public AudioClip Flatline
        {
            get
            {
                int num = 48000;
                return AudioClip.Create(string.Empty, num, Channels, num, stream: true, delegate (float[] buf)
                {
                    int num2 = buf.Length;
                    if (_flatlinePcmLen < num2)
                    {
                        _flatlinePcm = new float[num2];
                        for (int i = 0; i < num2; i++)
                        {
                            _flatlinePcm[i] = 1f;
                        }

                        _flatlinePcmLen = num2;
                    }

                    Array.Copy(_flatlinePcm, buf, num2);
                });
            }
        }

        public void OnAudioFilterRead(float[] data, int channels)
        {
            int num = MaxSamples * channels;
            int num2 = data.Length;
            if (num <= 0)
            {
                Array.Clear(data, 0, num2);
                return;
            }

            int num3 = 0;
            bool flag = num < num2;
            int num4 = (flag ? num : num2);
            while (num3 < num4)
            {
                float num5 = ReadSample();
                for (int i = 0; i < channels; i++)
                {
                    data[num3] *= num5 * VolumeScale;
                    num3++;
                }

                _collectedLoudness += Mathf.Abs(num5);
            }

            if (flag)
            {
                Array.Clear(data, num, num2 - num);
            }

            _collectedSamples += num2;
        }

        public void UpdateLoudnessMeter()
        {
            if (_collectedSamples >= 1200)
            {
                float num = _collectedLoudness * 5f;
                _targetLoudness = Mathf.Sqrt(num / (float)_collectedSamples);
                _collectedLoudness = 0f;
                _collectedSamples = 0;
            }

            Loudness = Mathf.Lerp(Loudness, _targetLoudness, Time.deltaTime * 40f);
        }

        public virtual void OnEnable()
        {
            Source.Play();
        }

        public virtual void OnDisable()
        {
            Source.Stop();
        }

        public virtual void Awake()
        {
            Source = GetComponent<AudioSource>();
            Source.clip = Flatline;
            Source.loop = true;
            Source.bypassReverbZones = true;
            Source.priority = 0;
        }

        public virtual void Update()
        {
            UpdateLoudnessMeter();
        }

        public virtual void UpdatePlayback(bool isPlaying, bool prevPlaying)
        {
            if (isPlaying == prevPlaying)
            {
                return;
            }

            if (isPlaying)
            {
                if (!Source.isPlaying)
                {
                    Source.Play();
                }
            }
            else
            {
                Source.Pause();
            }
        }

        public abstract float ReadSample();

        public void RegisterInput()
        {
            bool num = _isPlayingSustainRemFrames > 0;
            _isPlayingSustainRemFrames = 15;
            if (!num)
            {
                UpdatePlayback(isPlaying: true, prevPlaying: false);
            }
        }
    }
}
