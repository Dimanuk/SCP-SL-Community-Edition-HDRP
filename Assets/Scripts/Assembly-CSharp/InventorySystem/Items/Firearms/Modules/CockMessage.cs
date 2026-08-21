using System.Runtime.InteropServices;
using Mirror;

namespace InventorySystem.Items.Firearms.Modules
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct CockMessage : NetworkMessage
	{
	}
}
