using CameraShaking;
using PlayerRoles.FirstPersonControl;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106SpectatableModule : FpcSpectatableModule
    {
        internal override void OnBeganSpectating()
        {
            base.OnBeganSpectating();

            if (!(base.MainRole is Scp106Role scp106Role))
                return;

            Scp106Model model = scp106Role.FpcModule.CharacterModelInstance as Scp106Model;

            Scp106PortalShake effect = new Scp106PortalShake(scp106Role, model);
            Scp106PortalShake._latestEffect = effect;

            CameraShakeController.AddEffect(effect);
        }
    }
}
