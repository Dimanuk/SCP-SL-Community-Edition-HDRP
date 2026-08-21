namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079Speaker : global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase
    {
        private static readonly global::System.Collections.Generic.Dictionary<global::MapGeneration.RoomIdentifier, global::System.Collections.Generic.List<global::PlayerRoles.PlayableScps.Scp079.Scp079Speaker>> SpeakersInRooms = new global::System.Collections.Generic.Dictionary<global::MapGeneration.RoomIdentifier, global::System.Collections.Generic.List<global::PlayerRoles.PlayableScps.Scp079.Scp079Speaker>>();

        private bool _wasRegistered;

        protected override void OnRegistered()
        {
            base.OnRegistered();
            _wasRegistered = true;
            SpeakersInRooms.GetOrAdd(Room, () => new global::System.Collections.Generic.List<global::PlayerRoles.PlayableScps.Scp079.Scp079Speaker>()).Add(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_wasRegistered && !(Room == null) && SpeakersInRooms.TryGetValue(Room, out var value) && value.Remove(this) && value.Count == 0)
            {
                SpeakersInRooms.Remove(Room);
            }
        }

        public static bool TryGetSpeaker(global::PlayerRoles.PlayableScps.Scp079.Cameras.Scp079Camera cam, out global::PlayerRoles.PlayableScps.Scp079.Scp079Speaker best)
        {
            best = null;
            if (!SpeakersInRooms.TryGetValue(cam.Room, out var value))
            {
                return false;
            }
            global::UnityEngine.Vector3 position = cam.Position;
            bool flag = false;
            float num = 0f;
            foreach (global::PlayerRoles.PlayableScps.Scp079.Scp079Speaker item in value)
            {
                float sqrMagnitude = (item.Position - position).sqrMagnitude;
                if (!(sqrMagnitude > num && flag))
                {
                    flag = true;
                    num = sqrMagnitude;
                    best = item;
                }
            }
            return flag;
        }
    }
}
