using Mirror;

namespace InventorySystem.Items.Firearms.Attachments.Components
{
    [global::System.Serializable]
    public struct ReflexSightSyncMessage : global::Mirror.NetworkMessage
    {
        public ushort WeaponSerial;

        public int AttachmentId;

        public int TextureId;

        public int ColorId;

        public int SizeId;

        private const int BitsPerField = 4;
        private const int MaxFieldValue = (1 << BitsPerField) - 1;

        public ReflexSightSyncMessage(global::Mirror.NetworkReader reader)
        {
            WeaponSerial = reader.ReadUShort();
            ushort num = reader.ReadUShort();
            AttachmentId = (num & 0xF000) >> 12;
            TextureId = (num & 0xF00) >> 8;
            ColorId = (num & 0xF0) >> 4;
            SizeId = num & 0xF;
        }

        public void Write(global::Mirror.NetworkWriter writer)
        {
            if (!Validate())
                throw new System.InvalidOperationException("ReflexSightSyncMessage field values exceed 4-bit capacity (max 15).");

            int clampedAttachmentId = AttachmentId & MaxFieldValue;
            int clampedTextureId = TextureId & MaxFieldValue;
            int clampedColorId = ColorId & MaxFieldValue;
            int clampedSizeId = SizeId & MaxFieldValue;

            int num = clampedAttachmentId << 12;
            int num2 = clampedTextureId << 8;
            int num3 = clampedColorId << 4;
            int sizeId = clampedSizeId;

            writer.WriteUShort(WeaponSerial);
            writer.WriteUShort((ushort)(num + num2 + num3 + sizeId));
        }

        public bool Validate()
        {
            return AttachmentId <= MaxFieldValue
                && TextureId <= MaxFieldValue
                && ColorId <= MaxFieldValue
                && SizeId <= MaxFieldValue;
        }

        public override string ToString()
        {
            return $"Texture={TextureId} Color={ColorId} Size={SizeId}";
        }
    }
}