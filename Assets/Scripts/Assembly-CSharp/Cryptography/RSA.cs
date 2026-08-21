namespace Cryptography
{
	public static class RSA
	{
		public static bool Verify(string data, string signature, string key)
		{
			using (global::System.IO.TextReader reader = new global::System.IO.StringReader(key))
			{
				global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter parameters = (global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter)new global::Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
				global::Org.BouncyCastle.Crypto.ISigner signer = global::Org.BouncyCastle.Security.SignerUtilities.GetSigner("SHA256withRSA");
				signer.Init(forSigning: false, parameters);
				byte[] signature2 = global::System.Convert.FromBase64String(signature);
				byte[] array = global::System.Buffers.ArrayPool<byte>.Shared.Rent(global::System.Text.Encoding.UTF8.GetMaxByteCount(data.Length));
				int bytes = Utf8.GetBytes(data, array);
				signer.BlockUpdate(array, 0, bytes);
				global::System.Buffers.ArrayPool<byte>.Shared.Return(array);
				return signer.VerifySignature(signature2);
			}
		}
	}
}
