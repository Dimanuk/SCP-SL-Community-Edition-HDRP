using System;
using System.IO;
using Org.BouncyCastle.Crypto;

namespace LocalCentralServer
{
    /// <summary>
    /// Holds the two key pairs the central server needs and persists them so the MASTER public key
    /// (which gets hard-coded into the game's CentralServerKeyCache) stays stable across restarts.
    ///
    ///   master  -> signs the central public key served by publickey.php. The game verifies that
    ///              signature against the hard-coded master public key.
    ///   central -> signs preauth tokens and authentication tokens. The game (dedicated/host) verifies
    ///              those against the central public key it downloads from publickey.php.
    /// </summary>
    internal sealed class KeyStore
    {
        public AsymmetricCipherKeyPair Master { get; private set; }
        public AsymmetricCipherKeyPair Central { get; private set; }

        public string MasterPublicPem { get; private set; }
        public string CentralPublicPem { get; private set; }

        private readonly string _dir;

        private KeyStore(string dir) => _dir = dir;

        public static KeyStore LoadOrCreate(string dir)
        {
            Directory.CreateDirectory(dir);
            KeyStore ks = new KeyStore(dir);

            string masterPath = Path.Combine(dir, "master_private.pem");
            string centralPath = Path.Combine(dir, "central_private.pem");

            bool freshMaster = false;

            if (File.Exists(masterPath))
            {
                ks.Master = Crypto.KeyPairFromString(File.ReadAllText(masterPath));
            }
            else
            {
                ks.Master = Crypto.GenerateKeys();
                File.WriteAllText(masterPath, Crypto.KeyPairToString(ks.Master));
                freshMaster = true;
            }

            if (File.Exists(centralPath))
            {
                ks.Central = Crypto.KeyPairFromString(File.ReadAllText(centralPath));
            }
            else
            {
                ks.Central = Crypto.GenerateKeys();
                File.WriteAllText(centralPath, Crypto.KeyPairToString(ks.Central));
            }

            ks.MasterPublicPem = Crypto.KeyToString(ks.Master.Public);
            ks.CentralPublicPem = Crypto.KeyToString(ks.Central.Public);

            // Always (re)write the human-readable public files next to the keys.
            File.WriteAllText(Path.Combine(dir, "master_public.pem"), ks.MasterPublicPem);
            File.WriteAllText(Path.Combine(dir, "central_public.pem"), ks.CentralPublicPem);

            // Write the ready-to-paste C# literal for CentralServerKeyCache.MasterPublicKey.
            File.WriteAllText(Path.Combine(dir, "MasterPublicKey.csharp.txt"), ToCSharpLiteral(ks.MasterPublicPem));

            if (freshMaster)
            {
                Console.WriteLine();
                Console.WriteLine("====================================================================");
                Console.WriteLine(" A NEW MASTER KEY WAS GENERATED.");
                Console.WriteLine(" Paste the following into CentralServerKeyCache.MasterPublicKey:");
                Console.WriteLine("--------------------------------------------------------------------");
                Console.WriteLine(ToCSharpLiteral(ks.MasterPublicPem));
                Console.WriteLine("--------------------------------------------------------------------");
                Console.WriteLine(" (also saved to keys/MasterPublicKey.csharp.txt)");
                Console.WriteLine("====================================================================");
                Console.WriteLine();
            }

            return ks;
        }

        /// <summary>Turns a PEM string into a C# verbatim-escaped string literal (\r\n preserved).</summary>
        private static string ToCSharpLiteral(string pem)
        {
            string escaped = pem
                .Replace("\\", "\\\\")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\"", "\\\"");
            return "\"" + escaped + "\"";
        }
    }
}
