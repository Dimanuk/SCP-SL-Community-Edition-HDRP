using System.Collections.Generic;
using PlayerRoles.PlayableScps.Subroutines;
using TMPro;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.GUI
{
    public class Scp079AbilityList : Scp079GuiElementBase
    {
        [SerializeField]
        private List<Scp079KeyAbilityGui> _instances = new List<Scp079KeyAbilityGui>();

        [SerializeField]
        private TextMeshProUGUI _failMessageText;

        [SerializeField]
        private AudioClip _popupSound;

        private IScp079FailMessageProvider _trackedMessage;
        private float _cachedAlpha = -1f;
        private bool _failTextReady;
        private float _fadeoutBeginTime;
        private float _fadeoutEndTime;

        private const float TransitionSpeed = 5.5f;
        private const float FadeoutDuration = 1.8f;
        private const float SustainDuration = 4f;

        private float FailMessageAlpha
        {
            get
            {
                if (_cachedAlpha < 0f)
                {
                    _cachedAlpha = _failMessageText.alpha;
                }
                return _cachedAlpha;
            }
            set
            {
                value = Mathf.Clamp01(value);
                if (value == _cachedAlpha)
                    return;
                
                _failMessageText.alpha = value;
                _cachedAlpha = value;
            }
        }

        private static float CurrentTime => Time.timeSinceLevelLoad;

        public static Scp079AbilityList Singleton { get; private set; }

        public IScp079FailMessageProvider TrackedFailMessage
        {
            get => _trackedMessage;
            set
            {
                bool isNull = value == null || (value is Object obj && obj == null);
                
                if (!isNull)
                {
                    value.OnFailMessageAssigned();
                    if (string.IsNullOrEmpty(value.FailMessage))
                        return;
                }

                _trackedMessage = value;
                _failTextReady = false;

                if (!isNull)
                {
                    _fadeoutBeginTime = CurrentTime + SustainDuration;
                    _fadeoutEndTime = _fadeoutBeginTime + FadeoutDuration;
                    PlaySound(_popupSound);
                }
            }
        }

        private void Awake()
        {
            Singleton = this;
        }

        private void Update()
        {
            UpdateFailMessage();
            UpdateList();
        }

        private void UpdateFailMessage()
        {
            if (!_failTextReady || _trackedMessage == null || string.IsNullOrEmpty(_trackedMessage.FailMessage))
            {
                FailMessageAlpha -= Time.deltaTime * TransitionSpeed;
                
                if (FailMessageAlpha == 0f)
                    _failTextReady = true;
            }
            else
            {
                float target = 1f - Mathf.InverseLerp(_fadeoutBeginTime, _fadeoutEndTime, CurrentTime);
                FailMessageAlpha = Mathf.MoveTowards(FailMessageAlpha, target, Time.deltaTime * TransitionSpeed);
                _failMessageText.text = _trackedMessage.FailMessage;
            }
        }

        private void UpdateList()
        {
            int visibleCount = 0;
            int lastCategory = -1;
            
            SubroutineBase[] allSubroutines = base.Role.SubroutineModule.AllSubroutines;
            
            foreach (SubroutineBase subroutine in allSubroutines)
            {
                if (subroutine is Scp079LostSignalHandler signalHandler && signalHandler.Lost)
                {
                    visibleCount = 0;
                    break;
                }

                if (subroutine is Scp079KeyAbilityBase ability && ability.IsVisible)
                {
                    bool createSpace = false;
                    if (ability.CategoryId != lastCategory)
                    {
                        createSpace = lastCategory != -1;
                        lastCategory = ability.CategoryId;
                    }

                    if (visibleCount >= _instances.Count)
                    {
                        Scp079KeyAbilityGui template = _instances[0];
                        _instances.Add(Object.Instantiate(template, template.transform.parent));
                    }

                    _instances[visibleCount++].Setup(
                        ability.IsReady, 
                        ability.AbilityName, 
                        ability.ActivationKey, 
                        createSpace);
                }
            }

            if (!Scp079Role.LocalInstanceActive)
                visibleCount = 0;

            for (int i = visibleCount; i < _instances.Count; i++)
            {
                _instances[i].gameObject.SetActive(false);
            }
        }

        public Scp079AbilityList()
        {
            _instances = new List<Scp079KeyAbilityGui>();
            _cachedAlpha = -1f;
        }
    }
}
