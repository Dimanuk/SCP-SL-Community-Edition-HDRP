using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CursorManagement;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp939;
using ToggleableMenus;
using UnityEngine;
using UnityEngine.UI;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{
    public class MimicryMenuController : ToggleableMenuBase
    {
        [CompilerGenerated]
        private static MimicryMenuController _003CSingleton_003Ek__BackingField;

        [CompilerGenerated]
        private static bool _003CReadyForInteraction_003Ek__BackingField;

        [SerializeField]
        private Color _highlightColor;

        [SerializeField]
        private float _highlightLerpSpeed;

        [SerializeField]
        private GameObject _blur;

        [SerializeField]
        private float _fadeSpeed;

        [SerializeField]
        private CanvasGroup _defaultGroup;

        private readonly HashSet<CanvasGroup> _registeredGroups = new HashSet<CanvasGroup>();

        private readonly Stopwatch _interactionDelay = Stopwatch.StartNew();

        private CanvasGroup _curGroup;

        private bool _recenterCursor;

        private const float InteractionDelay = 0.2f;

        public static MimicryMenuController Singleton;
        public static bool ReadyForInteraction;

        public override bool CanToggle
        {
            get
            {
                if (ReferenceHub.TryGetLocalHub(out var hub))
                {
                    return hub.roleManager.CurrentRole is Scp939Role;
                }
                return false;
            }
        }

        public override CursorOverrideMode CursorOverride
        {
            get
            {
                if (_recenterCursor)
                {
                    _recenterCursor = false;
                    return CursorOverrideMode.Centered;
                }
                return base.CursorOverride;
            }
        }

        public CanvasGroup CurrentGroup
        {
            get => _curGroup;
            set
            {
                if (value == _curGroup)
                    return;

                if (_curGroup != null)
                    _recenterCursor = true;

                _curGroup = value;

                if (value != null)
                {
                    _blur.SetActive(true);
                    _registeredGroups.Add(value);
                    _defaultGroup = value;
                }
                else
                {
                    _blur.SetActive(false);
                    IsEnabled = false;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Singleton = this;
        }

        private void Update()
        {
            HashsetExtensions.ForEach(_registeredGroups, delegate(CanvasGroup x)
            {
                x.alpha = Mathf.MoveTowards(
                    x.alpha,
                    (x == CurrentGroup) ? 1f : 0f,
                    Time.deltaTime * _fadeSpeed);
            });

            if (CurrentGroup != null)
            {
                _interactionDelay.Restart();
                ReadyForInteraction = false;
            }
            else if (!ReadyForInteraction)
            {
                ReadyForInteraction = _interactionDelay.Elapsed.TotalSeconds > InteractionDelay;
            }
        }

        protected override void OnToggled()
        {
            CurrentGroup = IsEnabled ? _defaultGroup : null;
        }

        public static void UpdateHighlight(Image img, bool isHighlighted)
        {
            UpdateHighlight(img, isHighlighted, Time.deltaTime * Singleton._highlightLerpSpeed);
        }

        public static void UpdateHighlight(Image img, bool isHighlighted, float lerpAmount)
        {
            Color targetColor = isHighlighted ? Singleton._highlightColor : Color.clear;
            img.color = Color.Lerp(img.color, targetColor, lerpAmount);
        }
    }
}