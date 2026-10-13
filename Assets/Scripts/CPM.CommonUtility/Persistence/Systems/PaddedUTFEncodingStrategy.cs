using System;
using System.Text;

namespace Persistence.Systems
{
	public class PaddedUTFEncodingStrategy : ICipherStrategy
	{
		private readonly string pad;

		public PaddedUTFEncodingStrategy(string pad)
		{
			this.pad = pad;
		}

		public PaddedUTFEncodingStrategy()
		{
			pad = "pad";
		}

		public string Encode(string data)
		{
			data = pad + data;
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
		}

		public string Decode(string encodedData)
		{
			byte[] bytes = Convert.FromBase64String(encodedData);
			return Encoding.UTF8.GetString(bytes).Remove(0, pad.Length);
		}
	}
}
