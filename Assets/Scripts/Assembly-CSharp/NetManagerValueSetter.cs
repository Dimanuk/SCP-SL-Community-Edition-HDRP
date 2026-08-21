public class NetManagerValueSetter : global::UnityEngine.MonoBehaviour
{
    private CustomNetworkManager _singleton;

    private void Start()
    {
        _singleton = global::Mirror.NetworkManager.singleton.GetComponent<CustomNetworkManager>();
    }

    public void ChangeIP(string ip)
    {
        _singleton.networkAddress = ip;
        CustomNetworkManager.ConnectionIp = ip;
    }

    public void ChangePort(ushort port)
    {
        global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port = port;
    }

    public void JoinGame()
    {
        _singleton.StartClient();
    }

    public void HostGame()
    {
        _singleton.StartHost();
    }

    public void Disconnect()
    {
        _singleton.StopHost();
    }
}
