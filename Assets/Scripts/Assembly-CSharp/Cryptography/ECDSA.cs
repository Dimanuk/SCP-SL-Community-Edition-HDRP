namespace Cryptography
{
	public static class ECDSA
	{
		public static global::Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateKeys(int size = 384)
		{
			global::Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator eCKeyPairGenerator = new global::Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator("ECDSA");
			global::Org.BouncyCastle.Crypto.KeyGenerationParameters parameters = new global::Org.BouncyCastle.Crypto.KeyGenerationParameters(new global::Org.BouncyCastle.Security.SecureRandom(), size);
			eCKeyPairGenerator.Init(parameters);
			return eCKeyPairGenerator.GenerateKeyPair();
		}

		public static string Sign(string data, global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter privKey)
		{
			return global::System.Convert.ToBase64String(SignBytes(data, privKey));
		}

		public static byte[] SignBytes(string data, global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter privKey)
		{
			try
			{
				byte[] array = global::System.Buffers.ArrayPool<byte>.Shared.Rent(global::System.Text.Encoding.UTF8.GetMaxByteCount(data.Length));
				int bytes = Utf8.GetBytes(data, array);
				byte[] result = SignBytes(array, 0, bytes, privKey);
				global::System.Buffers.ArrayPool<byte>.Shared.Return(array);
				return result;
			}
			catch
			{
				return null;
			}
		}

		public static byte[] SignBytes(byte[] data, global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter privKey)
		{
			return SignBytes(data, 0, data.Length, privKey);
		}

		public static byte[] SignBytes(byte[] data, int offset, int count, global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter privKey)
		{
			try
			{
				global::Org.BouncyCastle.Crypto.ISigner signer = global::Org.BouncyCastle.Security.SignerUtilities.GetSigner("SHA-256withECDSA");
				signer.Init(forSigning: true, privKey);
				signer.BlockUpdate(data, offset, count);
				return signer.GenerateSignature();
			}
			catch
			{
				return null;
			}
		}

		public static bool Verify(string data, string signature, global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter pubKey)
		{
			return VerifyBytes(data, global::System.Convert.FromBase64String(signature), pubKey);
		}

		public static bool VerifyBytes(string data, byte[] signature, global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter pubKey)
		{
			try
			{
				global::Org.BouncyCastle.Crypto.ISigner signer = global::Org.BouncyCastle.Security.SignerUtilities.GetSigner("SHA-256withECDSA");
				signer.Init(forSigning: false, pubKey);
				byte[] array = global::System.Buffers.ArrayPool<byte>.Shared.Rent(global::System.Text.Encoding.UTF8.GetMaxByteCount(data.Length));
				int bytes = Utf8.GetBytes(data, array);
				signer.BlockUpdate(array, 0, bytes);
				global::System.Buffers.ArrayPool<byte>.Shared.Return(array);
				return signer.VerifySignature(signature);
			}
			catch (global::System.Exception ex)
			{
				global::GameCore.Console.AddLog("ECDSA Verification Error (BouncyCastle): " + ex.Message + ", " + ex.StackTrace, global::UnityEngine.Color.red);
				return false;
			}
		}

		public static global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter PublicKeyFromString(string key)
		{
			using (global::System.IO.TextReader reader = new global::System.IO.StringReader(key))
			{
				return (global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter)new global::Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
			}
		}

		public static global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter PrivateKeyFromString(string key)
		{
			using (global::System.IO.TextReader reader = new global::System.IO.StringReader(key))
			{
				return ((global::Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)new global::Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject()).Private;
			}
		}

		public static string KeyToString(global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter key)
		{
			using (global::System.IO.TextWriter textWriter = new global::System.IO.StringWriter())
			{
				global::Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new global::Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
				pemWriter.WriteObject(key);
				pemWriter.Writer.Flush();
				return textWriter.ToString();
			}
		}
	}
}
