using System.Collections.Generic;
using Mirror;

public class CustomInterestManagement : InterestManagement
{
    public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
    {
        return true;
    }

    public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn.isReady)
            {
                newObservers.Add(conn);
            }
        }

        if (NetworkServer.localConnection != null && NetworkServer.localConnection.isReady)
        {
            newObservers.Add(NetworkServer.localConnection);
        }
    }
}