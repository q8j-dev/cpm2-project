using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Garage.GarageSellService
{
	public interface IGarageSellService
	{
		bool IsConsumable { get; }

		UniTask<bool> FetchConsumableStatus();

		UniTask SellOrRemove(int garageIndex, GaragePropertyActionType actionType);

		UniTask<PurchaseItemType> GetGaragePurchaseItemType();
	}
}
