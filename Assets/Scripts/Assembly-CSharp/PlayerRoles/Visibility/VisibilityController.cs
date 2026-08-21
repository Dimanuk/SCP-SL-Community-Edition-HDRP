namespace PlayerRoles.Visibility
{
    public class VisibilityController : global::UnityEngine.MonoBehaviour, global::GameObjectPools.IPoolSpawnable
    {
        public virtual global::PlayerRoles.Visibility.InvisibilityFlags IgnoredFlags => global::PlayerRoles.Visibility.InvisibilityFlags.None;

        protected ReferenceHub Owner { get; private set; }

        protected global::PlayerRoles.PlayerRoleBase Role { get; private set; }

        public virtual global::PlayerRoles.Visibility.InvisibilityFlags GetActiveFlags(ReferenceHub observer)
        {
            return global::PlayerRoles.Visibility.InvisibilityFlags.None;
        }

        public virtual bool ValidateVisibility(ReferenceHub hub)
        {
            if (!(hub.roleManager.CurrentRole is global::PlayerRoles.Visibility.ICustomVisibilityRole customVisibilityRole))
            {
                return true;
            }
            return (customVisibilityRole.VisibilityController.GetActiveFlags(Owner) & ~IgnoredFlags) == 0;
        }

        public virtual void SpawnObject()
        {
            Role = GetComponentInParent<global::PlayerRoles.PlayerRoleBase>();
            if (Role == null)
            {
                throw new global::System.InvalidOperationException("VisibilityController " + base.name + " does not have a parent role set!");
            }
            if (!Role.TryGetOwner(out var hub))
            {
                throw new global::System.InvalidOperationException("VisibilityController " + base.name + " does not have an owner assigned!");
            }
            Owner = hub;
        }
    }
}
