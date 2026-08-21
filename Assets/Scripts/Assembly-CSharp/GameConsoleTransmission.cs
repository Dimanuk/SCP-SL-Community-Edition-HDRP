
using Mirror;
using Org.BouncyCastle.Security;
using RemoteAdmin;
using Security;
using UnityEngine;

public class GameConsoleTransmission : NetworkBehaviour
{
	public RemoteAdminCryptographicManager CryptoManager;

	public QueryProcessor Processor;

	private static SecureRandom _secureRandom;

	private RateLimit _cmdRateLimit;

    private void Start()
    {
        ReferenceHub hub = ReferenceHub.GetHub(base.gameObject);
        _cmdRateLimit = hub.playerRateLimitHandler.RateLimits[2];
        CryptoManager = GetComponent<global::RemoteAdmin.RemoteAdminCryptographicManager>();
        Processor = hub.queryProcessor;
        if (_secureRandom == null)
        {
            _secureRandom = new global::Org.BouncyCastle.Security.SecureRandom();
        }
    }

    [Server]
	public void SendToClient(string text, string color)
	{
        if (!global::Mirror.NetworkServer.active)
        {
            global::UnityEngine.Debug.LogWarning("[Server] function 'System.Void GameConsoleTransmission::SendToClient(System.String,System.String)' called when server was not active");
        }
        else
        {
            SendToClient(base.connectionToClient, text, color);
        }
    }

	[Server]
	public void SendToClient(NetworkConnection connection, string text, string color)
	{
        if (!global::Mirror.NetworkServer.active)
        {
            global::UnityEngine.Debug.LogWarning("[Server] function 'System.Void GameConsoleTransmission::SendToClient(Mirror.NetworkConnection,System.String,System.String)' called when server was not active");
            return;
        }
        byte[] bytes = Utf8.GetBytes(color + "#" + text);
        if (CryptoManager.EncryptionKey == null)
        {
            TargetPrintOnConsole(connection, bytes, encrypted: false);
        }
        else
        {
            TargetPrintOnConsole(connection, global::Cryptography.AES.AesGcmEncrypt(bytes, CryptoManager.EncryptionKey, _secureRandom), encrypted: true);
        }
    }

	[TargetRpc]
	public void TargetPrintOnConsole(NetworkConnection connection, byte[] data, bool encrypted)
	{
        if (data == null)
        {
            return;
        }
        string empty = string.Empty;
        if (!encrypted)
        {
            empty = Utf8.GetString(data);
        }
        else
        {
            if (CryptoManager.EncryptionKey == null)
            {
                global::GameCore.Console.AddLog("Can't process encrypted message from server before completing ECDHE exchange.", global::UnityEngine.Color.magenta);
                return;
            }
            try
            {
                empty = Utf8.GetString(global::Cryptography.AES.AesGcmDecrypt(data, CryptoManager.EncryptionKey));
            }
            catch
            {
                SendToClient("Decryption or verification of encrypted message failed.", "magenta");
                return;
            }
        }
        string text = empty.Remove(empty.IndexOf("#", global::System.StringComparison.Ordinal));
        empty = empty.Remove(0, empty.IndexOf("#", global::System.StringComparison.Ordinal) + 1);
        global::GameCore.Console.AddLog((encrypted ? "[FROM SERVER] " : "[UNENCRYPTED FROM SERVER] ") + empty, ProcessColor(text));
    }

	[Client]
	public void SendToServer(string command)
	{
        if (!global::Mirror.NetworkClient.active)
        {
            global::UnityEngine.Debug.LogWarning("[Client] function 'System.Void GameConsoleTransmission::SendToServer(System.String)' called when client was not active");
            return;
        }
        byte[] bytes = Utf8.GetBytes(command);
        if (CryptoManager.EncryptionKey == null)
        {
            CmdCommandToServer(bytes, encrypted: false);
        }
        else
        {
            CmdCommandToServer(global::Cryptography.AES.AesGcmEncrypt(bytes, CryptoManager.EncryptionKey, _secureRandom), encrypted: true);
        }
    }

	[Command]
	public void CmdCommandToServer(byte[] data, bool encrypted)
	{
        if (!_cmdRateLimit.CanExecute() || data == null)
        {
            return;
        }
        string empty = string.Empty;
        if (!encrypted)
        {
            if (CryptoManager.EncryptionKey != null || CryptoManager.ExchangeRequested)
            {
                SendToClient(base.connectionToClient, "Please use encrypted connection to send commands.", "magenta");
                return;
            }
            empty = Utf8.GetString(data);
        }
        else
        {
            if (CryptoManager.EncryptionKey == null)
            {
                SendToClient(base.connectionToClient, "Can't process encrypted message from server before completing ECDHE exchange.", "magenta");
                return;
            }
            try
            {
                empty = Utf8.GetString(global::Cryptography.AES.AesGcmDecrypt(data, CryptoManager.EncryptionKey));
            }
            catch
            {
                SendToClient(base.connectionToClient, "Decryption or verification of encrypted message failed.", "magenta");
                return;
            }
        }
        Processor.ProcessGameConsoleQuery(empty);
    }

    private global::UnityEngine.Color ProcessColor(string name)
    {
        return name switch
        {
            "red" => global::UnityEngine.Color.red,
            "cyan" => global::UnityEngine.Color.cyan,
            "blue" => global::UnityEngine.Color.blue,
            "magenta" => global::UnityEngine.Color.magenta,
            "white" => global::UnityEngine.Color.white,
            "green" => global::UnityEngine.Color.green,
            "yellow" => global::UnityEngine.Color.yellow,
            "black" => global::UnityEngine.Color.black,
            _ => global::UnityEngine.Color.grey,
        };
    }
}
