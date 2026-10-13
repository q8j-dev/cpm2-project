using System;
using System.Collections.Generic;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using Cysharp.Threading.Tasks;
using DataEntities.Vinyl;
using Persistence.Systems;
using Photon.Pun;
using UnityEngine;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CommonUtility.Contracts;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.DataTypes;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.SellExchangeFree
{
	public class SellExchangeFreeHelperAddCar
	{
		private readonly ISlotService _slotService;

		private readonly IPoliceSystem _policeSystem;

		public SellExchangeFreeHelperAddCar(ISlotService slotService, IPoliceSystem policeSystem)
		{
			_slotService = slotService;
			_policeSystem = policeSystem;
		}

		public async UniTask BuyMultiplayerCar(GameObject car, string tuningData)
		{
			if (!car || string.IsNullOrEmpty(tuningData))
			{
				return;
			}
			if (!TryParseCar(car, tuningData, out var carInfo, out var carInfoContainer))
			{
				ManagerUIHelper.SetMessage(new LocalizedString("CarChangeLogic.ErrorWhileParseCar"), 5f);
				return;
			}
			carInfo.Save();
			if (_slotService.TryGetEmptySlot(out var res))
			{
				int iD = res.ID;
				_slotService.SetSlot(iD, carInfo.CarInstanceId);
				_slotService.GetSlotCollectionBySlotId(iD).LastSlotId = iD;
				PlayerPrefs.SetInt("SendCarForRating", 1);
				try
				{
					SaveVinylsFromCarWhenBuy(carInfoContainer.VinylsData, carInfo.CarInstanceId);
				}
				catch (Exception ex)
				{
					Debug.LogError("Error while save vinyls: " + ex);
				}
				_policeSystem.SaveInstalledData(carInfo);
			}
			else
			{
				Debug.LogError("No empty slot. Can't buy car.");
			}
		}

		private bool TryParseCar(GameObject car, string data, out CarInfo carInfo, out CarInfoContainer carInfoContainer)
		{
			try
			{
				carInfoContainer = car.GetComponent<CarInfoContainer>();
				carInfo = carInfoContainer.CarInfo;
				CarTuningData tuningDataByString = PartDataExtensions.GetTuningDataByString(data, carInfo.CarID, out var carInstanceId);
				tuningDataByString.FrontWheelData = carInfo.TuningData.FrontWheelData.Clone() as FrontWheelData;
				tuningDataByString.RearWheelData = carInfo.TuningData.RearWheelData.Clone() as RearWheelData;
				tuningDataByString.Maintenance = ((carInfoContainer.vehicleDataResult.MaintenanceData.Clone() is MaintenanceInfo) ? ((MaintenanceInfo)carInfoContainer.vehicleDataResult.MaintenanceData.Clone()) : default(MaintenanceInfo));
				carInfo.CarInstanceId = carInstanceId;
				carInfo.TuningData = tuningDataByString;
				object[] instantiationData = car.GetComponent<PhotonView>().InstantiationData;
				carInfoContainer.VinylsData = CarVinylsConverter.Get(instantiationData[6]);
				carInfo.BoughtBodyKits = new BoughtBodyKits(carInfo.InstalledBodyKits);
			}
			catch (Exception ex)
			{
				Debug.LogError("Error while parse car: " + ex);
				carInfo = null;
				carInfoContainer = null;
				return false;
			}
			return true;
		}

		public void SaveVinylsFromCarWhenBuy(MultiplayerDataSending.CarVinyls carVinyls, string carInstanceId)
		{
			TrySaveVinyls(carVinyls.CarBodyVinyls, DirectoryHelper.GetCarVinylsPath(carInstanceId));
			TrySaveVinyls(carVinyls.WindowVinyls, DirectoryHelper.GetWindowVinylsPath(carInstanceId));
		}

		private static void TrySaveVinyls(object[] carVinyls, string getCarVinylsPath)
		{
			if (carVinyls == null)
			{
				Debug.Log("Empty car vinyls by path: " + getCarVinylsPath);
				return;
			}
			Dictionary<string, object> vinylsDictionary = GetVinylsDictionary(carVinyls);
			if (vinylsDictionary.Count > 0)
			{
				EasySaveDataHandler.SaveValues(vinylsDictionary, getCarVinylsPath);
			}
		}

		private static Dictionary<string, object> GetVinylsDictionary(object[] carBodyVinyls)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			for (int i = 0; i < carBodyVinyls.Length; i++)
			{
				object[] array = (object[])carBodyVinyls[i];
				if (array != null)
				{
					VinylData vinylData = VinylsUtility.UnpackVinylInstantiationData(array);
					string key = Convert.ToInt32(vinylData.position.z).ToString();
					dictionary.Add(key, vinylData);
				}
			}
			return dictionary;
		}
	}
}
