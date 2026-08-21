namespace Cryptography
{
	public static class ECDH
	{
		public static global::Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateKeys(int size = 384)
		{
			global::Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator eCKeyPairGenerator = new global::Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator("ECDH");
			global::Org.BouncyCastle.Crypto.KeyGenerationParameters parameters = new global::Org.BouncyCastle.Crypto.KeyGenerationParameters(new global::Org.BouncyCastle.Security.SecureRandom(), size);
			eCKeyPairGenerator.Init(parameters);
			return eCKeyPairGenerator.GenerateKeyPair();
		}

		public static global::Org.BouncyCastle.Crypto.Agreement.ECDHBasicAgreement Init(global::Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair localKey)
		{
			global::Org.BouncyCastle.Crypto.Agreement.ECDHBasicAgreement eCDHBasicAgreement = new global::Org.BouncyCastle.Crypto.Agreement.ECDHBasicAgreement();
			eCDHBasicAgreement.Init(localKey.Private);
			return eCDHBasicAgreement;
		}

		public static byte[] DeriveKey(global::Org.BouncyCastle.Crypto.Agreement.ECDHBasicAgreement exchange, global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter remoteKey)
		{
			using (global::System.Security.Cryptography.SHA256 sHA = global::System.Security.Cryptography.SHA256.Create())
			{
				return sHA.ComputeHash(exchange.CalculateAgreement(remoteKey).ToByteArray());
			}
		}
	}
}
