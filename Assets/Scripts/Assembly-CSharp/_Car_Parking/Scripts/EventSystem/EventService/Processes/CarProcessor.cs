using System;
using CPM.Database;
using Cysharp.Threading.Tasks;
using Persistence.Systems;
using UnityEngine;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.EventSystem.EventService.Processes
{
	public class CarProcessor : RewardProcessor
	{
		private const float StyleWaitSeconds = 8f;

		private const int StyleWaitPollMs = 200;

		private CommonProvider _commonProvider;

		private readonly AllCarsInfo _allCarsInfo;

		private NewCarVisualData _newCarVisualData;

		private int _carId = -1;

		public CarProcessor(EventsAnalytic eventsAnalytic, CommonProvider commonProvider, AllCarsInfo allCarsInfo)
			: base(eventsAnalytic)
		{
			_commonProvider = commonProvider;
			_allCarsInfo = allCarsInfo;
		}

		public override async UniTask Execute(RewardResponseBase responseBase, string eventId)
		{
			if (!(responseBase is RewardCarResponse carResponse) || !_commonProvider.SlotService.SlotCollection.TryGetEmptySlot(out var slot))
			{
				return;
			}
			_carId = carResponse.CarId;
			_newCarVisualData = await CarExtension.GetDefaultVisualDataFromPreset(carResponse.CarId);
			Action<CarInfo> addBodyKitsCallback = null;
			bool flag = !CarIdStorage.IsRewardExclusiveCar(carResponse.CarId);
			if (V.CoreGameManager.AddPurchasedCarToSlot(slot, carResponse.CarId, (flag && _newCarVisualData != null) ? new Action<CarInfo>(RecordStyleForNewCar) : null, addBodyKitsCallback, carResponse.CarInstanceId) == null)
			{
				EventsAnalytic.LogClaimReward(eventId, rewardRecieved: false, carResponse.CarId.ToString(), responseBase.Type);
				return;
			}
			EventsAnalytic.LogClaimReward(eventId, rewardRecieved: true, carResponse.CarId.ToString(), responseBase.Type);
			if (CarIdStorage.IsRewardExclusiveCar(carResponse.CarId))
			{
				NotifyAndFocusGarage(slot.ID, carResponse.CarInstanceId).Forget();
			}
		}

		private async UniTaskVoid NotifyAndFocusGarage(int slotId, string carInstanceId)
		{
			await WaitForBackendStyle(carInstanceId);
			V.CoreGameManager.SetMessage(new LocalizedString("Event.SteamCarClaimed"), 3f);
			await UniTask.Delay(3000);
			V.SlotID = slotId;
			V.CoreGameManager.ManagerUI.ShowGarageWindow();
		}

		private static async UniTask WaitForBackendStyle(string carInstanceId)
		{
			if (!string.IsNullOrEmpty(carInstanceId))
			{
				string vinylsPath = DirectoryHelper.GetCarVinylsPath(carInstanceId);
				float deadline = Time.unscaledTime + 8f;
				while (Time.unscaledTime < deadline && !EasySaveDataHandler.FileExists(vinylsPath))
				{
					await UniTask.Delay(200, DelayType.UnscaledDeltaTime);
				}
			}
		}

		private void RecordStyleForNewCar(CarInfo carInfo)
		{
			if (_newCarVisualData != null)
			{
				CarExtension.RecordStyleForNewPurchasedCar(carInfo, _newCarVisualData);
			}
		}

		private void RecordBodyKits(CarInfo carInfo)
		{
			InstalledBodyKits installedBodyKits = _allCarsInfo.GetVehicleStockTuningData(carInfo.CarID).CarInfoDefault.InstalledBodyKits;
			if (installedBodyKits != null)
			{
				CarExtension.RecordBodyKitsForNewPurchasedCar(carInfo, installedBodyKits);
			}
		}
	}
}
