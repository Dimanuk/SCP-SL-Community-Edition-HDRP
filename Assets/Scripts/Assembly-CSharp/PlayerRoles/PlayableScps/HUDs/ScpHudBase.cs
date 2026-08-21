using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.HUDs
{
    public abstract class ScpHudBase : MonoBehaviour
    {
        private ReferenceHub _hub;
        private float _updateCounterTimer;
        private bool _eventAssigned;
        private bool _useCounter;

        public ReferenceHub Hub
        {
            get => _hub;
            set => _hub = value;
        }

        [field: SerializeField]
        public TMP_Text TargetCounter { get; private set; }

        public event Action OnDestroyed;

        protected virtual void ToggleHud(bool b)
        {
            Canvas[] canvases = GetComponentsInChildren<Canvas>();
            for (int i = 0; i < canvases.Length; i++)
                canvases[i].enabled = b;
        }

        protected virtual void Update()
        {
            if (!_useCounter)
                return;

            _updateCounterTimer -= Time.deltaTime;
            if (_updateCounterTimer <= 0f)
            {
                UpdateCounter();
                _updateCounterTimer = 1f;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_eventAssigned)
            {
                HideHUDController.ToggleHUD -= ToggleHud;
                OnDestroyed?.Invoke();
            }
        }

        protected virtual void UpdateCounter()
        {
            int count = HashsetExtensions.Count(ReferenceHub.AllHubs, IsValidTarget);
            
            if (TargetCounter != null)
                TargetCounter.text = count.ToString();
        }

        private static bool IsValidTarget(ReferenceHub hub)
        {
            Faction faction = PlayerRolesUtils.GetFaction(hub);
            if (faction == Faction.SCP)
                return false;

            Team team = PlayerRolesUtils.GetTeam(hub);
            return team != Team.Dead;
        }

        internal virtual void OnDied()
        {
        }

        internal virtual void Init(ReferenceHub hub)
        {
            Hub = hub;
            _eventAssigned = true;
            _useCounter = TargetCounter != null;
            HideHUDController.ToggleHUD += ToggleHud;

            if (!HideHUDController.IsHUDVisible)
                ToggleHud(false);
        }
    }
}