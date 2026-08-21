using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Ripples
{
    public class RippleTriggerBase : ScpStandardSubroutine<Scp939Role>
    {
        private bool _playerSet;
        private RipplePlayer _player;

        private static int _playerIndex;

        protected RipplePlayer Player
        {
            get
            {
                if (_playerSet)
                    return _player;

                _player = base.ScpRole.SubroutineModule.AllSubroutines[PlayerIndex] as RipplePlayer;
                _playerSet = true;
                return _player;
            }
        }

        protected bool IsLocalOrSpectated
        {
            get
            {
                if (base.Owner.isLocalPlayer)
                    return true;

                return SpectatorNetworking.IsLocallySpectated(base.Owner);
            }
        }

        private int PlayerIndex
        {
            get
            {
                if (_playerIndex > 0)
                    return _playerIndex;

                SubroutineManagerModule subroutineModule = base.ScpRole.SubroutineModule;

                for (int i = 0; i < subroutineModule.AllSubroutines.Length; i++)
                {
                    if (subroutineModule.AllSubroutines[i] is RipplePlayer)
                    {
                        return _playerIndex = i;
                    }
                }

                throw new System.InvalidOperationException("SCP-939 has no RipplePlayer subroutine!");
            }
        }

        protected void PlayInRange(Vector3 pos, float maxRange, Color color)
        {
            PlayInRangeSqr(pos, maxRange * maxRange, color);
        }

        protected void PlayInRangeSqr(Vector3 pos, float maxRangeSqr, Color color)
        {
            // (pos - ScpRole.FpcModule.Position).sqrMagnitude <= maxRangeSqr
            if (!((pos - base.ScpRole.FpcModule.Position).sqrMagnitude > maxRangeSqr))
            {
                Player.Play(pos, color);
            }
        }
    }
}