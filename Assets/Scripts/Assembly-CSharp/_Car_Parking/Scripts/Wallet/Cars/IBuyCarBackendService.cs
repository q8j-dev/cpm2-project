using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.CustomDataTypes;

namespace _Car_Parking.Scripts.Wallet.Cars
{
	public interface IBuyCarBackendService
	{
		UniTask<(AdvancedBoolean, string)> TryBuy(CarBackendRequest data);
	}
}
