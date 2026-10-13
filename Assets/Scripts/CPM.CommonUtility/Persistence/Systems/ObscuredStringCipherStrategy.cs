using CodeStage.AntiCheat.ObscuredTypes;

namespace Persistence.Systems
{
	public class ObscuredStringCipherStrategy : ICipherStrategy
	{
		private static ObscuredString salt = "sample";

		public string Encode(string data)
		{
			return string.Join("", ObscuredString.Encrypt(data, salt));
		}

		public string Decode(string encodedData)
		{
			return ObscuredString.Decrypt(encodedData.ToCharArray(), salt);
		}
	}
}
