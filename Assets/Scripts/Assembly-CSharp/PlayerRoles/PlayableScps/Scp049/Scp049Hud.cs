using System;
using System.Collections.Generic;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.HUDs;
using PlayerRoles.PlayableScps.Scp049;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp049
{
    public class Scp049Hud : ScpHudBase
    {
        [Space]
        [SerializeField]
        private AbilityHud _senseElement;

        [SerializeField]
        private Image _senseIndicator;

        [SerializeField]
        private Vector3 _senseMinSize;

        [SerializeField]
        private Vector3 _senseMaxSize;

        [SerializeField]
        private Color _senseNearby;

        [SerializeField]
        private Color _senseFar;

        [SerializeField]
        private float _iconModifier;

        [SerializeField]
        private float _heartbeatModifier;

        [SerializeField]
        private float _heartbeatMin;

        [SerializeField]
        private float _heartbeatMax;

        [SerializeField]
        private AnimationCurve _heartbeatAnimation;

        [Space]
        [SerializeField]
        private LoadingCircleHud _resurrectBar;

        [SerializeField]
        private AbilityHud _callElement;

        [SerializeField]
        private AbilityHud _attackElement;

        [Space]
        [SerializeField]
        private GameObject _hudRoot;

        [SerializeField]
        private ScpWarningHud _warningHud;

        [SerializeField]
        private TMP_Text _zombieCounter;

        private Scp049ResurrectAbility _resurrectAbility;
        private Scp049AttackAbility _attackAbility;
        private Scp049SenseAbility _senseAbility;
        private Scp049CallAbility _callAbility;

        private Transform _senseTransform;
        private Transform _senseTransformParent;
        private GameObject _zombieCounterParent;

        private float _heartbeatTimer;

        internal override void Init(ReferenceHub hub)
        {
            base.Init(hub);

            if (hub.roleManager.CurrentRole is Scp049Role scp049)
            {
                scp049.SubroutineModule.TryGetSubroutine(out _resurrectAbility);
                scp049.SubroutineModule.TryGetSubroutine(out _attackAbility);
                scp049.SubroutineModule.TryGetSubroutine(out _senseAbility);
                scp049.SubroutineModule.TryGetSubroutine(out _callAbility);
            }

            if (_senseIndicator != null)
            {
                _senseTransform = _senseIndicator.transform;
                _senseTransformParent = _senseTransform.parent;
            }

            if (_zombieCounter != null && _zombieCounter.transform.parent != null)
            {
                _zombieCounterParent = _zombieCounter.transform.parent.gameObject;
            }

            if (_attackAbility != null && _attackElement != null)
                _attackElement.Setup(_attackAbility.Cooldown, null);

            if (_senseAbility != null && _senseElement != null)
                _senseElement.Setup(_senseAbility.Cooldown, _senseAbility.Duration);

            if (_callAbility != null && _callElement != null)
                _callElement.Setup(_callAbility.Cooldown, _callAbility.Duration);

            if (_senseAbility != null)
                _senseAbility.OnFailed += ShowSenseError;

            if (_resurrectAbility != null)
                _resurrectAbility.OnErrorReceived += ShowResurrectErrorCode;
        }

        internal override void OnDied()
        {
            if (_hudRoot != null)
                _hudRoot.SetActive(false);
        }

        protected override void OnDestroy()
        {
            if (_senseAbility != null)
                _senseAbility.OnFailed -= ShowSenseError;

            if (_resurrectAbility != null)
                _resurrectAbility.OnErrorReceived -= ShowResurrectErrorCode;

            base.OnDestroy();
        }

        protected override void Update()
        {
            base.Update();

            _attackElement?.Update(false);
            _senseElement?.Update(false);
            _callElement?.Update(false);

            if (_resurrectAbility != null && _resurrectBar != null)
            {
                float progress = Mathf.Clamp01(_resurrectAbility.ProgressStatus);
                
                bool showBar = !_resurrectBar._hideWhenFull || progress < 1f;
                if (_resurrectBar._parent != null)
                    _resurrectBar._parent.SetActive(showBar);

                if (showBar && _resurrectBar._loadingBar != null)
                {
                    if (_resurrectBar._inverseFill)
                        _resurrectBar._loadingBar.fillAmount = 1f - progress;
                    else
                        _resurrectBar._loadingBar.fillAmount = progress;

                    if (_resurrectBar._colorGradient != null)
                        _resurrectBar._loadingBar.color = _resurrectBar._colorGradient.Evaluate(progress);
                }
            }

            if (_senseAbility != null)
                UpdateSenses(_senseAbility.DistanceFromTarget);
        }

        private void ShowSenseError()
        {
            if (_warningHud == null)
                return;

            string text = TranslationReader.Get("SCP049_HUD", 10, "No target found.");
            _warningHud.SetText(text, 3.8f);
        }

        private void ShowResurrectErrorCode(byte code)
        {
            if (_warningHud == null)
                return;

            string format = String.Format("Resurrection refused, code #{0}", code);
            string text = TranslationReader.Get("SCP049_HUD", code - 1, format);
            _warningHud.SetText(text, 3.8f);
        }

        private void UpdateSenses(float distance)
        {
            if (_senseAbility == null || !_senseAbility.HasTarget)
            {
                if (_senseTransform != null)
                    _senseTransform.gameObject.SetActive(false);
                return;
            }

            float t = distance * _iconModifier;

            _heartbeatTimer += Time.deltaTime * Mathf.Lerp(_heartbeatMax, _heartbeatMin, _heartbeatModifier * distance);
            if (_heartbeatTimer > 1f)
                _heartbeatTimer = 0f;

            float pulse = _heartbeatAnimation.Evaluate(_heartbeatTimer);

            if (_senseIndicator != null)
                _senseIndicator.color = Color.Lerp(_senseNearby, _senseFar, t);

            if (_senseTransform != null)
                _senseTransform.localScale = Vector3.Lerp(_senseMinSize, _senseMaxSize, t);

            if (_senseTransformParent != null)
                _senseTransformParent.localScale = Vector3.one * pulse;

            if (_senseTransform != null)
                _senseTransform.gameObject.SetActive(true);
        }

        protected override void UpdateCounter()
        {
            if (Hub == null)
                return;

            int totalTargets = HashsetExtensions.Count(ReferenceHub.AllHubs, h => 
                h.roleManager.CurrentRole is HumanRole);

            if (TargetCounter != null)
                TargetCounter.text = totalTargets.ToString();

            int zombieCount = HashsetExtensions.Count(ReferenceHub.AllHubs, h =>
                PlayerRolesUtils.GetRoleId(h) == RoleTypeId.Scp0492);

            if (_zombieCounter != null)
                _zombieCounter.text = zombieCount.ToString();

            if (_zombieCounterParent != null)
                _zombieCounterParent.SetActive(zombieCount > 0);
        }
    }
}
