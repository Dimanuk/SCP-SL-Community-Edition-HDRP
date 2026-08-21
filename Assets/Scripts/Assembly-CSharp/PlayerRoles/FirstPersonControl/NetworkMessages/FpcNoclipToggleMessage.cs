using System.Runtime.InteropServices;
using Mirror;
using PlayerStatsSystem;

namespace PlayerRoles.FirstPersonControl.NetworkMessages
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct FpcNoclipToggleMessage : NetworkMessage
    {
        public void ProcessMessage(NetworkConnection sender)
        {
            if (ReferenceHub.TryGetHubNetID(sender.identity.netId, out var hub) && FpcNoclip.IsPermitted(hub))
            {
                if (hub.roleManager.CurrentRole is IFpcRole)
                {
                    hub.playerStats.GetModule<AdminFlagsStat>().InvertFlag(AdminFlags.Noclip);
                }
                else
                {
                    hub.gameConsoleTransmission.SendToClient("Noclip is not supported for this class.", "yellow");
                }
            }
        }
    }
}
