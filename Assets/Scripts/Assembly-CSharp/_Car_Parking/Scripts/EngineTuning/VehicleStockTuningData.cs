using System.Collections.Generic;
using System.Text.RegularExpressions;
using CPM.Balance;
using CPM.Database;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Serialization;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.EngineTuning
{
	[CreateAssetMenu(fileName = "car0", menuName = "CarConfigs/CarTuningData")]
	public class VehicleStockTuningData : ScriptableObject
	{
		[SerializeField]
		private PurchaseItemType PurchaseItem = PurchaseItemType.None;

		[SerializeField]
		private VehicleClass Class;

		[SerializeField]
		private ObscuredInt Score;

		[SerializeField]
		private ObscuredInt StyleDefault = -1;

		[Header("Not For Sale")]
		public bool Nfs;

		[FormerlySerializedAs("IsDailyRewards")]
		public ObscuredBool IsRewardCar;

		[SerializeField]
		[Header("New car spec")]
		private int Year = -1;

		[Header("Filters")]
		public List<CarType> CarTypes = new List<CarType>();

		public CarRegion region;

		[Header("Parts")]
		public CarDefaultTuningDataConfig Tuning;

		[Header("Offers")]
		public VehiclePresetTuningData[] Presets;

		[Header("Other car data")]
		public CarInfoMain CarInfoDefault;

		private int _sn = int.MinValue;

		public string GetClassName => Class.ToString();

		public VehicleClass GetClass => Class;

		public int GetPerformance => Score;

		public PurchaseItemType PurchaseItemType => PurchaseItem;

		public ObscuredInt DefaultStyle => StyleDefault;

		private int Sn
		{
			get
			{
				if (_sn == int.MinValue)
				{
					Match match = Regex.Match(base.name, "\\d+");
					if (match.Success)
					{
						_sn = int.Parse(match.Value);
					}
					else
					{
						Debug.LogError("[VehicleStockTuningData] Bad asset name '" + base.name + "'", this);
						_sn = -1;
					}
				}
				return _sn;
			}
		}

		public ObscuredInt Serial => Sn;

		public AdvancedInt GetPriceUI
		{
			get
			{
				CarPriceProvider.Get(Sn, out var price, out var _);
				return price;
			}
		}

		public AdvancedBoolean GetPriceTypeUI
		{
			get
			{
				CarPriceProvider.Get(Sn, out var _, out var isCoinPrice);
				return isCoinPrice;
			}
		}

		public void GetPrice(out AdvancedInt priceValue)
		{
			CarPriceProvider.Get(Sn, out priceValue, out var _);
		}

		public void GetPriceType(out AdvancedBoolean type)
		{
			CarPriceProvider.Get(Sn, out var _, out type);
		}
	}
}
