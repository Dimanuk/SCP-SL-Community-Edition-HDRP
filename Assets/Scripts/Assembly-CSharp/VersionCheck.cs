using GameCore;
using Mirror;
using UnityEngine;

public class VersionCheck : NetworkBehaviour
{
    private void Start()
    {
        if (NetworkServer.active && (!Version.AlwaysAcceptReleaseBuilds || Version.BuildType != Version.VersionType.Release))
        {
            if (Version.ExtendedVersionCheckNeeded)
            {
                TargetCheckExactVersion(base.connectionToClient, Version.VersionString);
            }
            else
            {
                TargetCheckVersion(base.connectionToClient, Version.Major, Version.Minor, Version.Revision);
            }
        }
    }

    [TargetRpc]
    private void TargetCheckVersion(NetworkConnection conn, byte major, byte minor, byte revision)
    {
        if (!Version.ExtendedVersionCheckNeeded)
        {
            if (Version.CompatibilityCheck(major, minor, revision))
            {
                return;
            }
        }

        if (CustomNetworkManager.TypedSingleton != null)
        {
            string serverVersionFormatted = string.Format("{0}.{1}.{2}", major, minor, revision);
            CustomNetworkManager.TypedSingleton.StopClient();
            CustomNetworkManager.TypedSingleton.ShowLog(16, Version.VersionString, serverVersionFormatted);
        }
    }

    [TargetRpc]
    private void TargetCheckExactVersion(NetworkConnection conn, string version)
    {
        if (Version.ListedServerCompatibilityCheck(version))
        {
            return;
        }

        if (CustomNetworkManager.TypedSingleton != null)
        {
            CustomNetworkManager.TypedSingleton.StopClient();
            CustomNetworkManager.TypedSingleton.ShowLog(16, Version.VersionString, version);
        }
    }
}