using System.Runtime.CompilerServices;
using GameObjectPools;

namespace PlayerRoles.PlayableScps.Subroutines
{
    public abstract class ScpStandardSubroutine<T> : SubroutineBase, IPoolSpawnable, IPoolResettable where T : PlayerRoleBase
    {
        public ReferenceHub Owner { get; private set; }
        public T ScpRole { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (base.Role != null)
                ScpRole = base.Role as T;
        }

        protected void GetSubroutine<TSubroutine>(out TSubroutine sr) where TSubroutine : global::PlayerRoles.PlayableScps.Subroutines.SubroutineBase
        {
            sr = null;

            if (base.Role == null)
                return;

            var subroutinedRole = base.Role as global::PlayerRoles.PlayableScps.Subroutines.ISubroutinedScpRole;
            if (subroutinedRole == null)
                return;

            if (subroutinedRole.SubroutineModule == null)
                return;

            subroutinedRole.SubroutineModule.TryGetSubroutine<TSubroutine>(out sr);
        }

        public virtual void SpawnObject()
        {
            if (!base.Role.TryGetOwner(out var hub))
            {
                throw new global::System.InvalidOperationException(
                    "Subroutine " + base.name + " of type " + GetType().FullName + " spawned with no valid owner!");
            }

            Owner = hub;

            if (ScpRole == null && base.Role != null)
                ScpRole = base.Role as T;

            SetupSubroutines();
        }

        protected virtual void SetupSubroutines() { }

        public virtual void ResetObject()
        {
        }
    }
}