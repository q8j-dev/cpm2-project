using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Steam.Base
{
	public interface ISteamPurchaseGate
	{
		bool PurchasesAllowed { get; }

		UniTask FetchAsync();
	}
}
