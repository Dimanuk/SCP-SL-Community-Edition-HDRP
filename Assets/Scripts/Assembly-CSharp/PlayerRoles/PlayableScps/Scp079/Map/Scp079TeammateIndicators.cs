using System;
using System.Collections.Generic;
using PlayerRoles.PlayableScps.Scp079.GUI;
using TMPro;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp079.Map
{
    public class Scp079TeammateIndicators : Scp079GuiElementBase
    {
        private enum IndicatorType
        {
            Low = 0,
            Medium = 1,
            High = 2
        }

        private readonly Dictionary<ReferenceHub, RectTransform> _instances = new Dictionary<ReferenceHub, RectTransform>();

        private Scp079TierManager _tierManager;
        private IZoneMap[] _maps;

        [SerializeField]
        private RectTransform _templateLow;

        [SerializeField]
        private RectTransform _templateMid;

        [SerializeField]
        private RectTransform _templateHigh;

        private IndicatorType CurType
        {
            get
            {
                int accessTierLevel = _tierManager.AccessTierLevel;
                if (accessTierLevel <= 2)
                    return IndicatorType.Low;
                if (accessTierLevel == 3)
                    return IndicatorType.Medium;
                return IndicatorType.High;
            }
        }

        internal override void Init(Scp079Role role, ReferenceHub owner)
        {
            base.Init(role, owner);
            role.SubroutineModule.TryGetSubroutine(out _tierManager);
            _tierManager.OnLevelledUp += Rebuild;
            PlayerRoleManager.OnRoleChanged += OnRoleChanged;
            _maps = GetComponentsInChildren<IZoneMap>(includeInactive: true);
            Rebuild();
        }

        private void OnDestroy()
        {
            _tierManager.OnLevelledUp -= Rebuild;
            PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
        }

        private void OnRoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (_instances.TryGetValue(userHub, out RectTransform value))
            {
                Destroy(value.gameObject);
                _instances.Remove(userHub);
            }
            SetupPlayer(userHub);
        }

        private void SetupPlayer(ReferenceHub hub)
        {
            if (!hub.IsSCP(includeZombies: false))
                return;

            RectTransform rectTransform;
            switch (CurType)
            {
                case IndicatorType.Low:
                    rectTransform = Instantiate(_templateLow);
                    break;
                case IndicatorType.Medium:
                    rectTransform = Instantiate(_templateMid);
                    break;
                case IndicatorType.High:
                    rectTransform = Instantiate(_templateHigh);
                    string text = hub.roleManager.CurrentRole.RoleTypeId.ToString();
                    if (text.Length > 3)
                    {
                        rectTransform.GetComponentInChildren<TextMeshProUGUI>().text = text.Substring(3);
                    }
                    break;
                default:
                    return;
            }

            rectTransform.localScale = Vector3.one;
            _instances[hub] = rectTransform;
        }

        private void Rebuild()
        {
            _instances.ForEachValue(x => Destroy(x.gameObject));
            _instances.Clear();
            ReferenceHub.AllHubs.ForEach(SetupPlayer);
        }

        private void Update()
        {
            if (!Scp079ToggleMapAbility.MapState)
                return;

            IndicatorType curType = CurType;
            foreach (KeyValuePair<ReferenceHub, RectTransform> instance in _instances)
            {
                bool flag = false;
                IZoneMap[] maps = _maps;
                for (int i = 0; i < maps.Length; i++)
                {
                    if (maps[i].TrySetTeammateIndicator(instance.Key, instance.Value, curType != IndicatorType.Low))
                    {
                        flag = true;
                    }
                }
                instance.Value.gameObject.SetActive(flag);
                if (flag && curType == IndicatorType.High)
                {
                    instance.Value.GetComponentInChildren<TextMeshProUGUI>().rectTransform.rotation = _templateHigh.rotation;
                }
            }
        }
    }
}