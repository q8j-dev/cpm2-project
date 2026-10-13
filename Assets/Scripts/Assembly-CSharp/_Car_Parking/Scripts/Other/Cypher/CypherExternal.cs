using System;
using System.Runtime.InteropServices;

namespace _Car_Parking.Scripts.Other.Cypher
{
	public class CypherExternal : ICypherCustom
	{
		[DllImport("cypher")]
		private static extern IntPtr CustomEncrypt(string plainText, string key, string iv);

		[DllImport("cypher")]
		private static extern IntPtr CustomDecrypt(string cypher, string key, string iv);

		[DllImport("cypher")]
		public static extern void FreeString(IntPtr str);

		public string Encode(string plainText, string key, string iv)
		{
			IntPtr intPtr = CustomEncrypt(plainText, key, iv);
			string result = Marshal.PtrToStringAnsi(intPtr);
			FreeString(intPtr);
			return result;
		}

		public string Decode(string cypher, string key, string iv)
		{
			IntPtr intPtr = CustomDecrypt(cypher, key, iv);
			string result = Marshal.PtrToStringAnsi(intPtr);
			FreeString(intPtr);
			return result;
		}
	}
}
