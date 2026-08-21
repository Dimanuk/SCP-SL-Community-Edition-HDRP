using System;
using System.Collections.Generic;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp049.Zombies
{
    public class ZombieIndicatorTracker : ScpStandardSubroutine<ZombieRole>
    {
        [SerializeField]
        private GameObject _effectPrefab;

        private bool _hasInstances;
        private readonly Dictionary<ReferenceHub, GameObject> _instances = new Dictionary<ReferenceHub, GameObject>();

        private static int _cachedCallId = -1;

        public override void ResetObject()
        {
            base.ResetObject();
            if (_hasInstances)
            {
                DestroyAll();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            ReferenceHub.OnPlayerRemoved += DestroyIndicator;
        }

        private void Update()
        {
            if (!base.ScpRole.IsLocalPlayer)
                return;

            ValidateAll();
        }

        private Scp049CallAbility GetCallFast(Scp049Role scp049)
        {
            SubroutineManagerModule subModule = scp049.SubroutineModule;
            SubroutineBase[] all = subModule.AllSubroutines;

            int cached = _cachedCallId;
            if (cached >= 0 && cached < all.Length)
            {
                SubroutineBase candidate = all[cached];
                if (candidate is Scp049CallAbility call)
                    return call;
            }

            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] is Scp049CallAbility call)
                {
                    _cachedCallId = i;
                    return call;
                }
            }

            throw new NullReferenceException("SCP-049 does not have a Scp049CallAbility subroutine!");
        }
        private void ValidateAll()
        {
            List<ReferenceHub> toRemove = null;
            foreach (var kvp in _instances)
            {
                if (!ValidatePlayer(kvp.Key))
                {
                    (toRemove ??= new List<ReferenceHub>()).Add(kvp.Key);
                }
            }
            toRemove?.ForEach(DestroyIndicator);

            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (ValidatePlayer(hub))
                {
                    ShowIndicator(hub);
                }
            }
        }
        private bool ValidatePlayer(ReferenceHub hub)
        {
            if (hub?.roleManager?.CurrentRole is not Scp049Role scp049)
                return false;

            try
            {
                Scp049CallAbility callAbility = GetCallFast(scp049);
                return callAbility.IsMarkerShown;
            }
            catch
            {
                return false;
            }
        }

        private void ShowIndicator(ReferenceHub target)
        {
            if (_instances.ContainsKey(target))
                return;

            _hasInstances = true;

            GameObject indicator = Instantiate(_effectPrefab, target.transform);
            _instances[target] = indicator;
        }

        private void DestroyIndicator(ReferenceHub target)
        {
            if (_instances.TryGetValue(target, out GameObject indicator))
            {
                if (indicator != null)
                {
                    Destroy(indicator);
                }
                _instances.Remove(target);
            }
        }

        private void DestroyAll()
        {
            foreach (var kvp in _instances)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value);
                }
            }
            _instances.Clear();
            _hasInstances = false;
        }
    }
}