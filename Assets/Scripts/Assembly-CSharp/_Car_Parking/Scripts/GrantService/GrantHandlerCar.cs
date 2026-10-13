using System;
using System.Threading;
using CPM.Database;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.GrantService
{
	public sealed class GrantHandlerCar : IGrantHandler
	{
		public RewardType Type => RewardType.Car;

		public async UniTask<bool> GrantAsync(RewardItemData reward, CancellationToken token)
		{
			if (!(reward is RewardItemCarData { ID: var carId, CarInstanceId: var carInstanceId }))
			{
				return false;
			}
			NewCarVisualData visual = await CarExtension.GetDefaultVisualDataFromPreset(carId);
			Action<CarInfo> addVinylsCallbackNew = ((visual != null) ? ((Action<CarInfo>)((CarInfo ci) =>
			{
				CarExtension.RecordStyleForNewPurchasedCar(ci, visual);
			})) : null);
			return CarExtension.BuyCarFromPurchase(carId, addVinylsCallbackNew, CarExtension.RecordBodyKitsForNewPurchasedCar, carInstanceId) == BuyCarFromPurchaseResult.Success;
		}
	}
}
