using System;
using PlayerRoles.PlayableScps.Subroutines;

namespace PlayerRoles.PlayableScps.Scp106
{
    public abstract class Scp106VigorAbilityBase : ScpKeySubroutine<Scp106Role>
    {
        private static int _vigorId;

        private static bool _vigorIdSet;

        private Scp106Vigor _vigor;

        private bool _vigorSet;

        private int VigorId
        {
            get
            {
                if (_vigorIdSet)
                    return _vigorId;

                SubroutineBase[] subroutines = AllSubroutines;
                for (int i = 0; i < subroutines.Length; i++)
                {
                    if (subroutines[i] is Scp106Vigor)
                    {
                        _vigorId = i;
                        _vigorIdSet = true;
                        return i;
                    }
                }

                throw new InvalidOperationException(
                    $"{base.Role} has no Scp106Vigor subroutine!");
            }
        }

        private SubroutineBase[] AllSubroutines
            => base.ScpRole.SubroutineModule.AllSubroutines;

        public virtual bool IsSubmerged => false;

        public virtual bool ForceHumanAnimations => false;

        protected Scp106Vigor Vigor
        {
            get
            {
                if (!_vigorSet)
                {
                    _vigorSet = true;
                    _vigor = AllSubroutines[VigorId] as Scp106Vigor;
                }
                return _vigor;
            }
        }
    }
}