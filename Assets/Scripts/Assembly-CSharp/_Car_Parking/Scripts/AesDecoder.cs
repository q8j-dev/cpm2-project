using Newtonsoft.Json;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.Other.Cypher;

namespace _Car_Parking.Scripts
{
	public class AesDecoder : IDecoder
	{
		public Result<T, string> Decode<T>(string data)
		{
			return Result<T, string>.Success(JsonConvert.DeserializeObject<T>(WalletCypher.Decode(data)));
		}
	}
}
