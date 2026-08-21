namespace Cryptography
{
	public static class AES
	{
		public const int NonceSizeBytes = 32;

		public const int MacSizeBits = 128;

		public static byte[] AesGcmEncrypt(byte[] data, byte[] secret, global::Org.BouncyCastle.Security.SecureRandom secureRandom)
		{
			byte[] array = new byte[32];
			secureRandom.NextBytes(array, 0, array.Length);
			global::Org.BouncyCastle.Crypto.Modes.GcmBlockCipher gcmBlockCipher = new global::Org.BouncyCastle.Crypto.Modes.GcmBlockCipher(new global::Org.BouncyCastle.Crypto.Engines.AesEngine());
			gcmBlockCipher.Init(forEncryption: true, new global::Org.BouncyCastle.Crypto.Parameters.AeadParameters(new global::Org.BouncyCastle.Crypto.Parameters.KeyParameter(secret), 128, array));
			byte[] array2 = new byte[gcmBlockCipher.GetOutputSize(data.Length)];
			int outOff = gcmBlockCipher.ProcessBytes(data, 0, data.Length, array2, 0);
			gcmBlockCipher.DoFinal(array2, outOff);
			using (global::System.IO.MemoryStream memoryStream = new global::System.IO.MemoryStream())
			{
				using (global::System.IO.BinaryWriter binaryWriter = new global::System.IO.BinaryWriter(memoryStream))
				{
					binaryWriter.Write(array);
					binaryWriter.Write(array2);
				}
				return memoryStream.ToArray();
			}
		}

		public static byte[] AesGcmDecrypt(byte[] data, byte[] secret)
		{
			using (global::System.IO.MemoryStream input = new global::System.IO.MemoryStream(data))
			{
				using (global::System.IO.BinaryReader binaryReader = new global::System.IO.BinaryReader(input))
				{
					byte[] array = binaryReader.ReadBytes(32);
					global::Org.BouncyCastle.Crypto.Modes.GcmBlockCipher gcmBlockCipher = new global::Org.BouncyCastle.Crypto.Modes.GcmBlockCipher(new global::Org.BouncyCastle.Crypto.Engines.AesEngine());
					gcmBlockCipher.Init(forEncryption: false, new global::Org.BouncyCastle.Crypto.Parameters.AeadParameters(new global::Org.BouncyCastle.Crypto.Parameters.KeyParameter(secret), 128, array));
					byte[] array2 = binaryReader.ReadBytes(data.Length - array.Length);
					byte[] array3 = new byte[gcmBlockCipher.GetOutputSize(array2.Length)];
					int outOff = gcmBlockCipher.ProcessBytes(array2, 0, array2.Length, array3, 0);
					gcmBlockCipher.DoFinal(array3, outOff);
					return array3;
				}
			}
		}
	}
}
