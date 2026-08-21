namespace Cryptography
{
	public static class Sha
	{
		public static byte[] Sha1(byte[] message)
		{
            using (global::System.Security.Cryptography.SHA1 sHA = global::System.Security.Cryptography.SHA1.Create())
			{
				return sHA.ComputeHash(message);
			}
		}

		public static byte[] Sha1(byte[] message, int offset, int length)
		{
			using (global::System.Security.Cryptography.SHA1 sHA = global::System.Security.Cryptography.SHA1.Create())
			{
				return sHA.ComputeHash(message, offset, length);
			}
		}

		public static byte[] Sha1(string message)
		{
			byte[] array = global::System.Buffers.ArrayPool<byte>.Shared.Rent(global::System.Text.Encoding.UTF8.GetMaxByteCount(message.Length));
			int bytes = Utf8.GetBytes(message, array);
			byte[] result = Sha1(array, 0, bytes);
			global::System.Buffers.ArrayPool<byte>.Shared.Return(array);
			return result;
		}

		public static byte[] Sha256(byte[] message)
		{
			using (global::System.Security.Cryptography.SHA256 sHA = global::System.Security.Cryptography.SHA256.Create())
			{
				return sHA.ComputeHash(message);
			}
		}

		public static byte[] Sha256(byte[] message, int offset, int length)
		{
			using (global::System.Security.Cryptography.SHA256 sHA = global::System.Security.Cryptography.SHA256.Create())
			{
				return sHA.ComputeHash(message, offset, length);
			}
		}

		public static byte[] Sha256(string message)
		{
			byte[] array = global::System.Buffers.ArrayPool<byte>.Shared.Rent(global::System.Text.Encoding.UTF8.GetMaxByteCount(message.Length));
			int bytes = Utf8.GetBytes(message, array);
			byte[] result = Sha256(array, 0, bytes);
			global::System.Buffers.ArrayPool<byte>.Shared.Return(array);
			return result;
		}

		public static byte[] Sha256Hmac(byte[] key, byte[] message)
		{
			using (global::System.Security.Cryptography.HMACSHA256 hMACSHA = new global::System.Security.Cryptography.HMACSHA256(key))
			{
				return hMACSHA.ComputeHash(message);
			}
		}

		public static byte[] Sha512(string message)
		{
			byte[] array = global::System.Buffers.ArrayPool<byte>.Shared.Rent(global::System.Text.Encoding.UTF8.GetMaxByteCount(message.Length));
			int bytes = Utf8.GetBytes(message, array);
			byte[] result = Sha512(array, 0, bytes);
			global::System.Buffers.ArrayPool<byte>.Shared.Return(array);
			return result;
		}

		public static byte[] Sha512(byte[] message)
		{
			using (global::System.Security.Cryptography.SHA512 sHA = global::System.Security.Cryptography.SHA512.Create())
			{
				return sHA.ComputeHash(message);
			}
		}

		public static byte[] Sha512(byte[] message, int offset, int length)
		{
			using (global::System.Security.Cryptography.SHA512 sHA = global::System.Security.Cryptography.SHA512.Create())
			{
				return sHA.ComputeHash(message, offset, length);
			}
		}

		public static byte[] Sha512Hmac(byte[] key, byte[] message)
		{
			using (global::System.Security.Cryptography.HMACSHA512 hMACSHA = new global::System.Security.Cryptography.HMACSHA512(key))
			{
				return hMACSHA.ComputeHash(message);
			}
		}

		public static byte[] Sha512Hmac(byte[] key, int offset, int length, byte[] message)
		{
			using (global::System.Security.Cryptography.HMACSHA512 hMACSHA = new global::System.Security.Cryptography.HMACSHA512(key))
			{
				return hMACSHA.ComputeHash(message, offset, length);
			}
		}

		public static string HashToString(byte[] hash)
		{
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			foreach (byte b in hash)
			{
				stringBuilder.Append(b.ToString("X2"));
			}
			string result = stringBuilder.ToString();
			global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			return result;
		}
	}
}
