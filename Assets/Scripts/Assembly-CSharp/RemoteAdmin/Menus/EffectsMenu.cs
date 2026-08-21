using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CustomPlayerEffects;
using RemoteAdmin.Elements;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace RemoteAdmin.Menus
{
    public class EffectsMenu : RaCommandMenu
    {
        private readonly List<RaEffectButton> _effectButtons = new List<RaEffectButton>();

        [SerializeField] private RaEffectButton _buttonTemplate;
        [SerializeField] private Transform _rootParent;
        [SerializeField] private Color _negativeColor;
        [SerializeField] private Color _positiveColor;
        [SerializeField] private Color _mixedColor;

        private static Func<StatusEffectBase, StatusEffectBase.EffectClassification> _cachedClassificationSelector;

        public void ResetOptions()
        {
            foreach (var button in _effectButtons)
            {
                if (button.InputFieldA != null)
                    button.InputFieldA.text = string.Empty;

                if (button.InputFieldB != null)
                    button.InputFieldB.text = string.Empty;
            }
        }

        protected override void OnStart()
        {
            if (!ReferenceHub.TryGetLocalHub(out var localHub))
                return;

            var playerEffects = localHub.playerEffectsController;

            _cachedClassificationSelector ??= e => e.Classification;

            var orderedEffects = playerEffects.AllEffects
                .OrderByDescending(_cachedClassificationSelector);

            foreach (var effect in orderedEffects)
            {
                var button = Instantiate(_buttonTemplate, _rootParent);
                button.CommandMenu = this;

                bool hasCustomDisplay = IsCustomDisplay(effect, out var customDisplay);
                button.EffectId = effect.GetType().Name;

                string displayName = hasCustomDisplay
                    ? customDisplay.DisplayName
                    : BuildName(effect.name);
                button.EffectName = displayName;

                var textColor = effect.Classification switch
                {
                    StatusEffectBase.EffectClassification.Positive => _positiveColor,
                    StatusEffectBase.EffectClassification.Negative => _negativeColor,
                    StatusEffectBase.EffectClassification.Mixed => _mixedColor,
                    _ => _mixedColor,
                };

                if (button.Text != null)
                    button.Text.color = textColor;

                button.Setup();
                _effectButtons.Add(button);
            }
        }

        private bool IsCustomDisplay(StatusEffectBase statusEffect, out ICustomRADisplay display)
        {
            if (statusEffect is ICustomRADisplay custom)
            {
                display = custom;
                return true;
            }

            display = null;
            return false;
        }

        private string BuildName(string objectName)
        {
            return Regex.Replace(objectName, @"(\B[A-Z])", " $1");
        }
    }
}