namespace InventorySystem.Items.Firearms
{
    public struct GunAudioMessage : global::Mirror.NetworkMessage
    {
        public const float OutsideLoudnessMultiplier = 2.3f;

        public ItemType Weapon;

        public ReferenceHub ShooterHub;

        public global::RelativePositioning.RelativePosition ShooterPosition;

        public byte AudioClipId;

        public byte MaxDistance;

        public GunAudioMessage(ReferenceHub shooter, byte audioId, byte maxDistance, ReferenceHub target)
        {
            if (target.roleManager.CurrentRole is global::PlayerRoles.Visibility.ICustomVisibilityRole customVisibilityRole && !customVisibilityRole.VisibilityController.ValidateVisibility(shooter))
            {
                ShooterPosition = new global::RelativePositioning.RelativePosition(shooter.transform.position);
                ShooterHub = null;
            }
            else
            {
                ShooterPosition = default(global::RelativePositioning.RelativePosition);
                ShooterHub = shooter;
            }
            Weapon = shooter.inventory.CurItem.TypeId;
            AudioClipId = audioId;
            MaxDistance = maxDistance;
        }

        public void Deserialize(global::Mirror.NetworkReader reader)
        {
            Weapon = (ItemType)reader.ReadByte();
            ShooterPosition = global::RelativePositioning.RelativePositionSerialization.ReadRelativePosition(reader);
            if (ShooterPosition.WaypointId == 0)
            {
                ShooterHub = global::Utils.Networking.ReferenceHubReaderWriter.ReadReferenceHub(reader);
            }
            AudioClipId = reader.ReadByte();
            MaxDistance = reader.ReadByte();
        }

        public void Serialize(global::Mirror.NetworkWriter writer)
        {
            writer.WriteByte((byte)Weapon);
            global::RelativePositioning.RelativePositionSerialization.WriteRelativePosition(writer, ShooterPosition);
            if (ShooterPosition.WaypointId == 0)
            {
                global::Utils.Networking.ReferenceHubReaderWriter.WriteReferenceHub(writer, ShooterHub);
            }
            writer.WriteByte(AudioClipId);
            writer.WriteByte(MaxDistance);
        }
    }
}
