using System;
using System.Security.Cryptography;
using System.Text;

namespace CPM.Persistence.Integrity
{
	public static class SaveFileSignature
	{
		private static readonly byte[] EmptyContent = new byte[0];

		public static string Compute(string key, byte[] content)
		{
			using HMACSHA256 hMACSHA = new HMACSHA256(Encoding.UTF8.GetBytes(key ?? string.Empty));
			return Convert.ToBase64String(hMACSHA.ComputeHash(content ?? EmptyContent));
		}

		public static bool Verify(string signature, string key, byte[] content)
		{
			if (string.IsNullOrEmpty(signature))
			{
				return false;
			}
			return FixedTimeEquals(signature, Compute(key, content));
		}

		private static bool FixedTimeEquals(string a, string b)
		{
			if (a.Length != b.Length)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < a.Length; i++)
			{
				num |= a[i] ^ b[i];
			}
			return num == 0;
		}
	}
}
