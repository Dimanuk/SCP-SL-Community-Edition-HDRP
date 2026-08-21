using System;
using System.Linq;
using System.Text;
using NorthwoodLib.Pools;
using PlayerRoles.PlayableScps.Scp079.GUI;
using TMPro;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Map
{
    public class Scp079TargetCounter : Scp079GuiElementBase
    {
        [Serializable]
        private struct CounterSet
        {
            [SerializeField]
            private TargetCounter[] _allCounters;

            public string Text
            {
                get
                {
                    StringBuilder sb = StringBuilderPool.Shared.Rent();
                    TargetCounter[] allCounters = _allCounters;
                    for (int i = 0; i < allCounters.Length; i++)
                    {
                        TargetCounter counter = allCounters[i];
                        sb.Append(counter.Header);
                        sb.Append(": ");
                        sb.Append(Utils.NonAllocLINQ.HashsetExtensions.Count(ReferenceHub.AllHubs, counter.Check));
                        sb.Append('\n');
                    }
                    return StringBuilderPool.Shared.ToStringReturn(sb);
                }
            }
        }

        [Serializable]
        private struct TargetCounter
        {
            [SerializeField]
            private string _defaultValue;

            [SerializeField]
            private string _translationKey;

            [SerializeField]
            private int _translationIndex;

            [SerializeField]
            private Team[] _teams;

            public string Header => TranslationReader.Get(_translationKey, _translationIndex, _defaultValue);

            public bool Check(ReferenceHub hub)
            {
                return _teams.Contains(hub.GetTeam());
            }
        }

        [SerializeField]
        private CounterSet[] _countersForTier;

        [SerializeField]
        private TextMeshProUGUI _counterTxt;

        private bool _isDirty;

        private Scp079TierManager _tier;

        internal override void Init(Scp079Role role, ReferenceHub owner)
        {
            base.Init(role, owner);
            _isDirty = true;
            PlayerRoleManager.OnRoleChanged += OnRoleChanged;
            role.SubroutineModule.TryGetSubroutine(out _tier);
        }

        private void OnDestroy()
        {
            PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
        }

        private void Update()
        {
            if (Scp079ToggleMapAbility.MapState && _isDirty)
            {
                int index = Mathf.Clamp(_tier.AccessTierIndex, 0, _countersForTier.Length - 1);
                _counterTxt.text = _countersForTier[index].Text;
            }
        }

        private void OnRoleChanged(ReferenceHub hub, PlayerRoleBase oldRole, PlayerRoleBase newRole)
        {
            _isDirty = true;
        }
    }
}