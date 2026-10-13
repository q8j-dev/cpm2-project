using System;
using System.Text;

namespace Persistence.Systems
{
	public class UTFEncodingStrategy : ICipherStrategy
	{
		public string Encode(string data)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
		}

		public string Decode(string encodedData)
		{
			byte[] bytes = Convert.FromBase64String(encodedData);
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
