namespace Persistence.Systems
{
	public class NoCipherStrategy : ICipherStrategy
	{
		public string Encode(string data)
		{
			return data;
		}

		public string Decode(string encodedData)
		{
			return encodedData;
		}
	}
}
