using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Economics;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class VisualTuningPricesService
	{
		private IWheelsDataService _wheelsDataService;

		private WheelsDiskTuningConfig _wheelsDiskTuningConfig;

		private BrakeTuningConfig _brakeTuningConfig;

		private static ObscuredInt _paintPrice = 1000;

		private IPricesService _pricesService;

		public const float UsedDetailPriceСoefficient = 0.5f;

		[Inject]
		public VisualTuningPricesService(IWheelsDataService wheelsDataService, WheelsDiskTuningConfig wheelsDiskTuningConfig, BrakeTuningConfig brakeTuningConfig, IPricesService pricesService)
		{
			_pricesService = pricesService;
			_wheelsDiskTuningConfig = wheelsDiskTuningConfig;
			_wheelsDataService = wheelsDataService;
			_brakeTuningConfig = brakeTuningConfig;
		}

		public ObscuredInt GetColorPrice()
		{
			return _paintPrice;
		}

		public GlobalCost GetWheelPrice(int index)
		{
			GlobalCost result = default;
			WheelDiskData wheelData = WheelsTuningScope.GetWheelData(index, _wheelsDiskTuningConfig.AllWheels);
			bool isCoinPrice = wheelData.isCoinPrice;
			result.PriceType = (isCoinPrice ? CostType.Coins : CostType.Money);
			if (_wheelsDataService.BoughtWheels.Contains(wheelData.index))
			{
				result.Price = 0;
				return result;
			}
			ObscuredInt price = ((result.PriceType != CostType.Money) ? _wheelsDiskTuningConfig.WheelsCoinsPrice : ((ObscuredInt)MoneyPrice(wheelData)));
			result.Price = price;
			return result;
		}

		public GlobalCost GetBrakePrice(int index)
		{
			GlobalCost result = default;
			BrakeDiskItemData brakeDataData = WheelsTuningScope.GetBrakeDataData(index, _brakeTuningConfig.AllBrakes);
			bool flag = brakeDataData.isCoinPrice;
			result.PriceType = (flag ? CostType.Coins : CostType.Money);
			if (_wheelsDataService.IsBrakeBought(brakeDataData.index))
			{
				result.Price = 0;
				return result;
			}
			result.Price = brakeDataData.price;
			return result;
		}

		public GlobalCost GetCalipersPrice(int index)
		{
			GlobalCost result = default;
			BrakeDiskItemData brakeDataData = WheelsTuningScope.GetBrakeDataData(index, _brakeTuningConfig.AllBrakesCaliperDatas);
			bool flag = brakeDataData.isCoinPrice;
			result.PriceType = (flag ? CostType.Coins : CostType.Money);
			if (_wheelsDataService.IsCaliperBought(brakeDataData.index))
			{
				result.Price = 0;
				return result;
			}
			result.Price = brakeDataData.price;
			return result;
		}

		private int MoneyPrice(WheelDiskData wheelDiskData)
		{
			return Mathf.RoundToInt(Mathf.Clamp(Mathf.RoundToInt((float)(wheelDiskData.index * _wheelsDiskTuningConfig.AllWheels.Length) * 0.1f + (float)(int)_pricesService.RimsStep), _pricesService.RimsMin, _pricesService.RimsMax));
		}
	}
}
