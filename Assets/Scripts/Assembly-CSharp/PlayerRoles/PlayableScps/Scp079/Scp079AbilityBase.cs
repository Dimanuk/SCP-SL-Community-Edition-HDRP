using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp079.Rewards;
using PlayerRoles.PlayableScps.Subroutines;

namespace PlayerRoles.PlayableScps.Scp079
{
    public abstract class Scp079AbilityBase : ScpStandardSubroutine<Scp079Role>
    {
        protected Scp079TierManager TierManager { get; private set; }
        protected Scp079AuxManager AuxManager { get; private set; }
        protected Scp079CurrentCameraSync CurrentCamSync { get; private set; }
        public Scp079LostSignalHandler LostSignalHandler { get; private set; }
        protected Scp079RewardManager RewardManager { get; private set; }

        public virtual float AuxRegenMultiplier => 1f;

        protected override void Awake()
        {
            base.Awake();

            SubroutineBase[] allSubroutines = base.ScpRole.SubroutineModule.AllSubroutines;
            foreach (SubroutineBase subroutine in allSubroutines)
            {
                if (subroutine is Scp079TierManager tierManager)
                    TierManager = tierManager;
                else if (subroutine is Scp079AuxManager auxManager)
                    AuxManager = auxManager;
                else if (subroutine is Scp079CurrentCameraSync cameraSync)
                    CurrentCamSync = cameraSync;
                else if (subroutine is Scp079LostSignalHandler lostSignalHandler)
                    LostSignalHandler = lostSignalHandler;
                else if (subroutine is Scp079RewardManager rewardManager)
                    RewardManager = rewardManager;
            }
        }
    }
}