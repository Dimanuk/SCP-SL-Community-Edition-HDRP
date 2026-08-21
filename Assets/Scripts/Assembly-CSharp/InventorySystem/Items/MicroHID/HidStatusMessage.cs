using Mirror;

namespace InventorySystem.Items.MicroHID
{
	public struct HidStatusMessage : NetworkMessage
	{
		public ushort Serial;

		public HidStatusMessageType MessageType;

		public byte MessageCode;

		public float Time;
	}
}
