using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Free_drive;

namespace _Car_Parking.Scripts.SellExchangeFree
{
	public interface ISellExchangeFreeService
	{
		UniTask TrySetCarOnSellTriggerAsync(SellCarTrigger sellCarTrigger, GameObject car);

		void SellCarPlaced(string carModel, GameObject car, int numberSellCarTrigger, string tradeOfferID);

		void SellCarExit(int place);

		void SellCarExit(SellCarTriggerData sellCarTriggerData);

		UniTask TrySellCarToOtherPlayerAsync(int price, GameObject otherPerson);

		UniTask TryExchangeCarToOtherPlayerAsync(GameObject otherPerson);
	}
}
