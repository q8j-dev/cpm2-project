using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Connection
{
	public interface ITokenService
	{
		string Token { get; set; }

		UniTask LoadToken();
	}
}
