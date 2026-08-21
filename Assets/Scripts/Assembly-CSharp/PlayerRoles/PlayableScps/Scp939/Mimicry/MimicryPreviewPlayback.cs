using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoiceChat.Networking;
using VoiceChat.Playbacks;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{

    public class MimicryPreviewPlayback : VoiceChatPlaybackBase
    {
        private PlaybackBuffer _playback;
        private bool _playbackSet;
        private int _duration;

        public override int MaxSamples
        {
            get
            {
                if (!_playbackSet)
                    return 0;

                return _playback.Length;
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (!_playbackSet)
                    return true;

                return _playback.ReadHead == _playback.WriteHead;
            }
        }


        private string SamplesToString(float samples)
        {
            return ((float)Mathf.CeilToInt(samples / 48000f * 10f) / 10f).ToString("0.0s");
        }

        public override float ReadSample()
        {
            return _playback.Read();
        }


        public void StartPreview(PlaybackBuffer pb)
        {
            if (_playbackSet)
            {
                _playback.Clear();
            }
            else
            {
                _playback = new PlaybackBuffer(pb.Buffer.Length, endlessTapeMode: true);
                _playbackSet = true;
            }

            _duration = pb.Length;
            _playback.Write(pb.Buffer, _duration);
        }

        public void StopPreview()
        {
            if (_playbackSet)
            {
                _playback.Clear();
            }
        }

        public void UpdateProgress(Slider timeline, TextMeshProUGUI elapsed, TextMeshProUGUI duration)
        {
            if (!_playbackSet)
                return;

            int remaining = _duration - _playback.Length;

            timeline.minValue = 0f;
            timeline.maxValue = _duration;
            timeline.value = remaining;

            elapsed.text = SamplesToString(remaining);
            duration.text = SamplesToString(_duration);
        }
    }
}