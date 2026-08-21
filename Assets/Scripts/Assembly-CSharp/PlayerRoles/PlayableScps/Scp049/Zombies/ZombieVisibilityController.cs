using PlayerRoles.FirstPersonControl;

namespace PlayerRoles.PlayableScps.Scp049.Zombies
{
    public class ZombieVisibilityController : FpcVisibilityController
    {
        public override bool ValidateVisibility(ReferenceHub hub)
        {
            if (base.ValidateVisibility(hub))
                return true;

            return hub.roleManager.CurrentRole is global::PlayerRoles.PlayableScps.Scp049.Scp049Role;
        }
    }
}