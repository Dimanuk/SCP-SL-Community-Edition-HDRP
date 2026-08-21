using Mirror;
using PlayerRoles.PlayableScps.Subroutines;

namespace PlayerRoles.PlayableScps.Scp939
{
    public class Scp939FocusKeySync : ScpKeySubroutine<Scp939Role>
    {
        private Scp939FocusAbility _focus;

        protected override ActionName TargetKey => ActionName.Sneak;

        protected override bool IsKeyHeld
        {
            get => base.IsKeyHeld;
            set
            {
                if (base.IsKeyHeld == value)
                    return;
                
                base.IsKeyHeld = value;
                ClientSendCmd();
            }
        }

        protected override bool KeyPressable
        {
            get
            {
                if (!Owner.isLocalPlayer)
                    return false;

                if (UnityEngine.Cursor.visible)
                    return _focus.TargetState;

                return true;
            }
        }

        public bool FocusKeyHeld { get; private set; }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            writer.WriteBool(IsKeyHeld);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);
            FocusKeyHeld = reader.ReadBool();
        }

        public override void ResetObject()
        {
            base.ResetObject();
            FocusKeyHeld = false;
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _focus);
        }
    }
}