using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Other
{
	public class MemoryPackDecoder : IDecoder
	{
		private readonly string _accountID;

		private readonly bool _useBrotli;

		public MemoryPackDecoder(string accountID, bool useBrotli = false)
		{
			_accountID = accountID;
			_useBrotli = useBrotli;
		}

		public Result<T, string> Decode<T>(string data)
		{
			return MemoryPackHelper.TryDeserialize<T>(data, _accountID, _useBrotli);
		}
	}
}
