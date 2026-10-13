using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace _Car_Parking.Scripts.Other.Cypher
{
	public class CypherStandalone : ICypherCustom
	{
		public static string keyAdd = "12345678";

		public static string ivAdd = "01234567";

		private const bool Logging = false;

		public string Encode(string message, string key, string iv)
		{
			return Convert.ToBase64String(CustomEncrypt(message, key, iv));
		}

		public string Decode(string encryptedString, string key, string iv)
		{
			return CustomDecrypt(Convert.FromBase64String(encryptedString), key, iv);
		}

		public static byte[] CustomEncrypt(string plainText, string key, string iv)
		{
			key = key.Substring(0, 8) + keyAdd;
			iv = iv.Substring(0, 8) + ivAdd;
			return EncryptStringToBytesAes(plainText, key, iv);
		}

		public static string CustomDecrypt(byte[] cipherText, string key, string iv)
		{
			key = key.Substring(0, 8) + keyAdd;
			iv = iv.Substring(0, 8) + ivAdd;
			return DecryptStringFromBytesAes(cipherText, key, iv);
		}

		public static byte[] EncryptStringToBytesAes(string plainText, string key, string iv)
		{
			using Aes aes = Aes.Create();
			aes.Key = Encoding.UTF8.GetBytes(key);
			aes.IV = Encoding.UTF8.GetBytes(iv);
			ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
			using MemoryStream memoryStream = new MemoryStream();
			using CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
			using (StreamWriter streamWriter = new StreamWriter(stream))
			{
				streamWriter.Write(plainText);
			}
			return memoryStream.ToArray();
		}

		public static string DecryptStringFromBytesAes(byte[] cipherText, string key, string iv)
		{
			string text = null;
			using Aes aes = Aes.Create();
			aes.Key = Encoding.UTF8.GetBytes(key);
			aes.IV = Encoding.UTF8.GetBytes(iv);
			ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
			using MemoryStream stream = new MemoryStream(cipherText);
			using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
			using StreamReader streamReader = new StreamReader(stream2);
			return streamReader.ReadToEnd();
		}
	}
}
