using System;
using System.Collections.Generic;
using System.Diagnostics;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using UnityEngine;
using VoiceChat;

namespace PlayerRoles.Voice
{
    public class VoiceBubble : MonoBehaviour
    {
        [Serializable]
        private struct Bubble
        {
            public Sprite Icon;
            public float MaxDistance;
            public VoiceChatChannel Channel;
        }

        [SerializeField]
        private Bubble[] _serializedOverrides;

        [SerializeField]
        private VoiceModuleBase _voiceModule;

        [SerializeField]
        private FirstPersonMovementModule _fpc;

        [SerializeField]
        private SpriteRenderer _rend;

        [SerializeField]
        private Gradient _colorOverNormalizedDistance;

        private const float SustainTime = 0.15f;

        private readonly Dictionary<VoiceChatChannel, Bubble> _overrides = new();
        private readonly Stopwatch _sustainSw = Stopwatch.StartNew();

        private Transform _t;
        private bool _isCulled;

        private bool IsHidden
        {
            get
            {
                if (_voiceModule == null || _sustainSw == null)
                    return true;

                // Sustain window elapsed -> bubble hidden.
                if (_sustainSw.Elapsed.TotalSeconds > SustainTime)
                    return true;

                if (!_voiceModule.Role.TryGetOwner(out ReferenceHub owner) || owner == null)
                    return true;

                // Never draw the local player's own bubble.
                if (owner.isLocalPlayer)
                    return true;

                // While spectating, hide only the spectated player's own bubble;
                // everyone else's stays visible. In first person all remote bubbles show.
                if (SpectatorTargetTracker.TryGetTrackedPlayer(out ReferenceHub trackedPlayer))
                {
                    return trackedPlayer == owner;
                }

                return false;
            }
        }

        private void OnEnable() => _isCulled = false;
        private void OnDisable() => _isCulled = true;

        private void Awake()
        {
            _t = transform;

            foreach (var bubble in _serializedOverrides)
            {
                if (bubble.Icon != null)
                    _overrides[bubble.Channel] = bubble;
            }

            if (gameObject != null)
                gameObject.SetActive(HideHUDController.IsHUDVisible);

            HideHUDController.ToggleHUD += OnHUDToggle;
            MainCameraController.OnUpdated += UpdateIcon;
        }

        private void OnDestroy()
        {
            HideHUDController.ToggleHUD -= OnHUDToggle;
            MainCameraController.OnUpdated -= UpdateIcon;
        }

        private void OnHUDToggle(bool visible)
        {
            if (gameObject != null)
                gameObject.SetActive(visible);
        }

        private void UpdateIcon()
        {
            // Culled (component disabled): leave the renderer untouched and bail, as in v12.
            if (_isCulled)
                return;

            if (_rend == null || _voiceModule == null)
                return;

            // Refresh the sustain window FIRST, unconditionally, whenever we're speaking —
            // this must happen before the IsHidden gate or the window can never renew.
            if (_voiceModule.IsSpeaking)
                _sustainSw.Restart();

            VoiceChatChannel channel = _voiceModule.CurrentChannel;

            if (!_overrides.TryGetValue(channel, out Bubble bubble))
            {
                _rend.enabled = false;
                return;
            }

            if (IsHidden)
            {
                _rend.enabled = false;
                return;
            }

            Vector3 cameraPos = MainCameraController.CurrentCamera != null
                ? MainCameraController.CurrentCamera.position
                : MainCameraController.LastPosition;

            float distance = Vector3.Distance(_t.position, cameraPos);
            float normalizedDistance = bubble.MaxDistance > 0f
                ? Mathf.Clamp01(distance / bubble.MaxDistance)
                : 1f;

            _rend.enabled = true;
            _rend.sprite = bubble.Icon;

            if (_colorOverNormalizedDistance != null)
            {
                _rend.color = _colorOverNormalizedDistance.Evaluate(normalizedDistance);
            }

            if (_t != null && MainCameraController.CurrentCamera != null)
            {
                _t.forward = MainCameraController.CurrentCamera.forward;
            }
        }

        public VoiceBubble()
        {
            _overrides = new Dictionary<VoiceChatChannel, Bubble>();
            _sustainSw = Stopwatch.StartNew();
        }
    }
}