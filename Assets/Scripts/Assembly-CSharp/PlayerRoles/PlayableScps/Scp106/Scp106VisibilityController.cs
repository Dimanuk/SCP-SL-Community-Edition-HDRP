using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Visibility;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106VisibilityController : FpcVisibilityController
    {
        private Scp106Role _role106;
        private Scp106StalkVisibilityController _visSubroutine;

        private bool CheckPlayer(ReferenceHub observer)
        {
            if (observer.IsHuman())
            {
                return _visSubroutine.SyncDamage.ContainsKey(observer.PlayerId);
            }
            return true;
        }

        public override InvisibilityFlags GetActiveFlags(ReferenceHub observer)
        {
            InvisibilityFlags invisibilityFlags = base.GetActiveFlags(observer);
            if (_role106.Sinkhole != null && _role106.Sinkhole.IsHidden && !CheckPlayer(observer))
            {
                invisibilityFlags |= InvisibilityFlags.Scp106Sinkhole;
            }
            return invisibilityFlags;
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            if (NetworkServer.active)
            {
                _role106 = base.Role as Scp106Role;
                _role106.SubroutineModule.TryGetSubroutine(out _visSubroutine);
            }
        }
    }
}
