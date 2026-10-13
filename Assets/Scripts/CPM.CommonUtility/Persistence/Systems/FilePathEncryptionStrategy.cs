using System;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Persistence.Systems
{
	public class FilePathEncryptionStrategy : ICipherStrategy
	{
		private readonly ObscuredString key;

		private const string IllegalChars = "/\\:*?";

		private const char SafeChar = '_';

		public FilePathEncryptionStrategy(string key)
		{
			this.key = key;
		}

		public string Encode(string data)
		{
			string text = "/\\:*?";
			foreach (char oldChar in text)
			{
				data = data.Replace(oldChar, '_');
			}
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			byte[] bytes2 = Encoding.UTF8.GetBytes(key);
			byte[] array = new byte[bytes.Length];
			for (int j = 0; j < bytes.Length; j++)
			{
				array[j] = (byte)(bytes[j] ^ bytes2[j % bytes2.Length]);
			}
			return Convert.ToBase64String(array);
		}

		public string Decode(string encodedData)
		{
			byte[] array = Convert.FromBase64String(encodedData);
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			byte[] array2 = new byte[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = (byte)(array[i] ^ bytes[i % bytes.Length]);
			}
			string text = Encoding.UTF8.GetString(array2);
			string text2 = "/\\:*?";
			foreach (char newChar in text2)
			{
				text = text.Replace('_', newChar);
			}
			return text;
		}
	}
}
