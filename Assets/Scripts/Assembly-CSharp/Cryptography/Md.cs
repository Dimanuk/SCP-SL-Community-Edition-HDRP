namespace Cryptography
{
	public static class Md
	{
		public static byte[] Md5(byte[] message)
		{
			using (global::System.Security.Cryptography.MD5 mD = global::System.Security.Cryptography.MD5.Create())
			{
				return mD.ComputeHash(message);
			}
		}

		public static byte[] Md5(byte[] message, int offset, int length)
		{
			using (global::System.Security.Cryptography.MD5 mD = global::System.Security.Cryptography.MD5.Create())
			{
				return mD.ComputeHash(message, offset, length);
			}
		}

		public static byte[] Md5(string message)
		{
			byte[] array = global::System.Buffers.ArrayPool<byte>.Shared.Rent(global::System.Text.Encoding.UTF8.GetMaxByteCount(message.Length));
			int bytes = Utf8.GetBytes(message, array);
			byte[] result = Md5(array, 0, bytes);
			global::System.Buffers.ArrayPool<byte>.Shared.Return(array);
			return result;
		}
	}
}
