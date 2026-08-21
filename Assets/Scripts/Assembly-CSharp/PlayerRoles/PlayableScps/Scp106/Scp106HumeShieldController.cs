using PlayerRoles.PlayableScps.HumeShield;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106HumeShieldController : DynamicHumeShieldController
    {
        private Scp106Role _role106;
        private Scp106StalkAbility _stalk;

        public override float HsRegeneration
        {
            get
            {
                if (_stalk == null || !_stalk.IsActive)
                    return 0f;

                if (_role106 == null)
                    return 0f;

                Scp106SinkholeController sinkhole = _role106.Sinkhole;
                if (sinkhole == null || !sinkhole.IsHidden)
                    return 0f;

                return RegenerationRate * HsMax;
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();

            _role106 = base.Role as Scp106Role;
			
            if (_role106 != null && _role106.SubroutineModule != null)
            {
                _role106.SubroutineModule.TryGetSubroutine(out _stalk);
            }
        }
    }
}