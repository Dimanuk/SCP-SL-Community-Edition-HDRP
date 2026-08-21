namespace Cryptography
{
	public static class PBKDF2
	{
		public static string Pbkdf2HashString(string password, byte[] salt, int iterations, int outputBytes)
		{
			return global::System.Convert.ToBase64String(Pbkdf2HashBytes(password, salt, iterations, outputBytes));
		}

		public static byte[] Pbkdf2HashBytes(string password, byte[] salt, int iterations, int outputBytes)
		{
			using (global::System.Security.Cryptography.Rfc2898DeriveBytes rfc2898DeriveBytes = new global::System.Security.Cryptography.Rfc2898DeriveBytes(password, salt)
			{
				IterationCount = iterations
			})
			{
				return rfc2898DeriveBytes.GetBytes(outputBytes);
			}
		}
	}
}
