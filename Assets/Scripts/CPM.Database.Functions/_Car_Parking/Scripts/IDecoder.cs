using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts
{
	public interface IDecoder
	{
		Result<T, string> Decode<T>(string data);
	}
}
