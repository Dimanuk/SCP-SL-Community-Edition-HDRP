using System.Collections.Generic;
using PlayerStatsSystem;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.Spectating
{
    public class SpectatorStatBarEmulator : MonoBehaviour
    {
        private readonly List<Graphic> _recolorable = new List<Graphic>();
        private StatSlider[] _statSliders;
        private StatusBar[] _statusBars;
        private GameObject _statsRoot;

        private static readonly int[] RecolorableStatIds = { 0 };

        private void Awake()
        {
            SpectatorTargetTracker.OnTargetChanged += OnTargetChanged;

            if (UserMainInterface.Singleton != null)
            {
                _statsRoot = UserMainInterface.Singleton.PlyStats;
                _statusBars = _statsRoot.GetComponentsInChildren<StatusBar>(true);
                _statSliders = _statsRoot.GetComponentsInChildren<StatSlider>(true);

                foreach (var slider in _statSliders)
                {
                    if (System.Array.IndexOf(RecolorableStatIds, slider._displayedStatId) >= 0)
                        _recolorable.AddRange(slider.GetComponentsInChildren<Graphic>());
                }
            }
        }

        private void OnDestroy()
        {
            SpectatorTargetTracker.OnTargetChanged -= OnTargetChanged;
        }

        private void OnTargetChanged()
        {
            if (_statsRoot == null)
                return;

            if (SpectatorTargetTracker.TryGetTrackedPlayer(out var player))
            {
                var currentRole = player.roleManager.CurrentRole;

                if (currentRole is IHealthbarRole)
                {
                    var roleColor = currentRole.RoleColor;

                    _statsRoot.SetActive(true);

                    foreach (var gc in _recolorable)
                    {
                        var c = gc.color;
                        gc.color = new Color(roleColor.r, roleColor.g, roleColor.b, c.a);
                    }

                    if (_statSliders != null)
                    {
                        foreach (var s in _statSliders)
                            s.ForceUpdate();
                    }

                    if (_statusBars != null)
                    {
                        foreach (var b in _statusBars)
                            b.UpdateBar(true);
                    }

                    return;
                }
            }

            _statsRoot.SetActive(false);
        }
    }
}
