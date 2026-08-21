public static class CentralServerKeyCache
{
    private const string CacheLocation = "internal/KeyCache";

    private const string CacheSignatureLocation = "internal/KeySignatureCache";

    private const string InternalDir = "internal/";

    internal static readonly global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter MasterKey;

    private const string MasterPublicKey = "-----BEGIN PUBLIC KEY-----\r\nMIGbMBAGByqGSM49AgEGBSuBBAAjA4GGAAQAbL0YvrhVB2meqCq5XzjAJD8Ii0hb\r\nBHdIQ587N583cP8twjDhcITjZhBHJPJDuA85XdpgG04HwT0SD3WcAvoQXBUAUsG1\r\nLS9TR4urHwfgfroq4tH2HAQE6ZxFZeIFSglLO8nxySim4yKBj96HLG624lzKvzoD\r\nId+GOwjcd3XskOq9Dwc=\r\n-----END PUBLIC KEY-----";

    // Master public key for the local test central server (Tools/LocalCentralServer). It is only used
    // when LocalCentralServer.IsActive (i.e. a "localcentral.txt" override file is present in the app
    // folder), so the official master key above stays in force for normal play.
    private const string LocalMasterPublicKey = "-----BEGIN PUBLIC KEY-----\r\nMHYwEAYHKoZIzj0CAQYFK4EEACIDYgAExfV64k8w0CS81pQOVhTj1lH2/6EaVIfB\r\np8U6v8QkPCpqg2D/AUBEtoS18sSWsVXaDreMZYbhZZ6I1uyG3aommT6jLPXd6d6+\r\nV5C2RJBlt+lr7OG7TBBKtMt+D0Mi7E88\r\n-----END PUBLIC KEY-----\r\n";

    static CentralServerKeyCache()
    {
        string key = LocalCentralServer.IsActive ? LocalMasterPublicKey : MasterPublicKey;
        MasterKey = global::Cryptography.ECDSA.PublicKeyFromString(key);
    }

    public static string ReadCache()
    {
        try
        {
            string appFolder = FileManager.GetAppFolder();
            string path = appFolder + "internal/KeyCache";
            string path2 = appFolder + "internal/KeySignatureCache";
            if (!global::System.IO.File.Exists(path))
            {
                ServerConsole.AddLog("Central server public key not found in cache.");
                return null;
            }
            if (!global::System.IO.File.Exists(path2))
            {
                ServerConsole.AddLog("Central server public key signature not found in cache.");
                return null;
            }
            string[] source = FileManager.ReadAllLines(path);
            string[] array = FileManager.ReadAllLines(path2);
            if (array.Length == 0)
            {
                ServerConsole.AddLog("Can't load central server public key from cache - empty signature.");
                return null;
            }
            string text = global::System.Linq.Enumerable.Aggregate(source, "", (string current, string line) => current + line + "\r\n").Trim();
            try
            {
                if (global::Cryptography.ECDSA.Verify(text, array[0], MasterKey))
                {
                    return text;
                }
                global::GameCore.Console.AddLog("Invalid signature of Central Server Key in cache!", global::UnityEngine.Color.red);
                return null;
            }
            catch (global::System.Exception ex)
            {
                if (ServerStatic.IsDedicated)
                {
                    ServerConsole.AddLog("Can't load central server public key from cache - " + ex.Message);
                }
                else
                {
                    global::GameCore.Console.AddLog("Can't load central server public key from cache - " + ex.Message, global::UnityEngine.Color.magenta);
                }
                return null;
            }
        }
        catch (global::System.Exception ex2)
        {
            ServerConsole.AddLog("Can't read public key cache - " + ex2.Message);
            return null;
        }
    }

    public static void SaveCache(string key, string signature)
    {
        try
        {
            if (!global::Cryptography.ECDSA.Verify(key, signature, MasterKey))
            {
                global::GameCore.Console.AddLog("Invalid signature of Central Server Key!", global::UnityEngine.Color.red);
                return;
            }
            string appFolder = FileManager.GetAppFolder();
            string path = appFolder + "internal/KeyCache";
            if (!global::System.IO.Directory.Exists(FileManager.GetAppFolder() + "internal/"))
            {
                global::System.IO.Directory.CreateDirectory(FileManager.GetAppFolder() + "internal/");
            }
            if (global::System.IO.File.Exists(path))
            {
                if (key == ReadCache())
                {
                    ServerConsole.AddLog("Key cache is up to date.");
                    return;
                }
                global::System.IO.File.Delete(path);
            }
            ServerConsole.AddLog("Updating key cache...");
            FileManager.WriteStringToFile(key, path);
            FileManager.WriteStringToFile(signature, appFolder + "internal/KeySignatureCache");
            ServerConsole.AddLog("Key cache updated.");
        }
        catch (global::System.Exception ex)
        {
            ServerConsole.AddLog("Can't write public key cache - " + ex.Message);
        }
    }
}
