using System;
using System.IO;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace LocalCentralServer
{
    /// <summary>
    /// Mirror of the game's Cryptography.ECDSA helper (Assets/Scripts/Assembly-CSharp/Cryptography/ECDSA.cs).
    /// Uses the SAME BouncyCastle.Crypto.dll the game ships with, so PEM keys and SHA-256withECDSA
    /// signatures produced here verify 1:1 inside the game.
    /// </summary>
    internal static class Crypto
    {
        private const string SignerName = "SHA-256withECDSA";

        public static AsymmetricCipherKeyPair GenerateKeys(int size = 384)
        {
            ECKeyPairGenerator generator = new ECKeyPairGenerator("ECDSA");
            generator.Init(new KeyGenerationParameters(new SecureRandom(), size));
            return generator.GenerateKeyPair();
        }

        /// <summary>ECDSA sign UTF-8 text, returns base64 (matches ECDSA.Sign).</summary>
        public static string Sign(string data, AsymmetricKeyParameter privKey)
        {
            return Convert.ToBase64String(SignBytes(data, privKey));
        }

        public static byte[] SignBytes(string data, AsymmetricKeyParameter privKey)
        {
            byte[] raw = Encoding.UTF8.GetBytes(data);
            ISigner signer = SignerUtilities.GetSigner(SignerName);
            signer.Init(true, privKey);
            signer.BlockUpdate(raw, 0, raw.Length);
            return signer.GenerateSignature();
        }

        public static bool Verify(string data, string signatureBase64, AsymmetricKeyParameter pubKey)
        {
            byte[] raw = Encoding.UTF8.GetBytes(data);
            ISigner signer = SignerUtilities.GetSigner(SignerName);
            signer.Init(false, pubKey);
            signer.BlockUpdate(raw, 0, raw.Length);
            return signer.VerifySignature(Convert.FromBase64String(signatureBase64));
        }

        /// <summary>PEM-encode a key (matches ECDSA.KeyToString). Public key -> "PUBLIC KEY".</summary>
        public static string KeyToString(AsymmetricKeyParameter key)
        {
            using (TextWriter writer = new StringWriter())
            {
                PemWriter pem = new PemWriter(writer);
                pem.WriteObject(key);
                pem.Writer.Flush();
                return writer.ToString();
            }
        }

        public static string KeyPairToString(AsymmetricCipherKeyPair pair)
        {
            using (TextWriter writer = new StringWriter())
            {
                PemWriter pem = new PemWriter(writer);
                pem.WriteObject(pair.Private);
                pem.Writer.Flush();
                return writer.ToString();
            }
        }

        public static AsymmetricCipherKeyPair KeyPairFromString(string pem)
        {
            using (TextReader reader = new StringReader(pem))
            {
                object obj = new PemReader(reader).ReadObject();
                if (obj is AsymmetricCipherKeyPair pair)
                    return pair;
                throw new InvalidDataException("PEM did not contain an EC private key pair.");
            }
        }
    }
}
