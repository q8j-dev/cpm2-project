namespace Persistence.Systems
{
	public interface ICipherStrategy
	{
		string Encode(string data);

		string Decode(string encodedData);
	}
}
