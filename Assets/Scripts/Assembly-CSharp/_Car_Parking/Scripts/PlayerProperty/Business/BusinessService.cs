using CodeStage.AntiCheat.Storage;
using MVPFree.MapIcons.Enum;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	[Preserve]
	public class BusinessService : IBusinessService
	{
		public const int BusinessCount = 10;

		private const string FastFoodBuilding = "FastFoodBuilding";

		private readonly CurrencyManagerController _currencyManagerController;

		private readonly BusinessStorage _businessStorage;

		private IEarnMoneyService _buyMoneyService;

		public BusinessService(CurrencyManagerController currencyManagerController, BusinessStorage businessStorage, IEarnMoneyService buyMoneyService)
		{
			_currencyManagerController = currencyManagerController;
			_businessStorage = businessStorage;
			_buyMoneyService = buyMoneyService;
		}

		public bool HasBought(int businessId)
		{
			return ObscuredPrefs.GetInt("FastFoodBuilding" + businessId) == 1;
		}

		public BusinessModel GetBusinessData(int businessId)
		{
			BusinessConfig businessById = _businessStorage.GetBusinessById(businessId);
			if (businessById == null)
			{
				return default;
			}
			return new BusinessModel
			{
				BusinessId = businessById.BusinessId,
				Name = businessById.Name,
				ProfitPerSecond = businessById.ProfitPerSecond,
				MaxProfitStorage = _buyMoneyService.BusinessProfitMaxPerSession,
				IconType = DefineIcon(businessById.Name),
				Location = businessById.Location,
				PriceModel = new BusinessPriceModel
				{
					Cost = businessById.Price
				}
			};
		}

		private IconType DefineIcon(NameType name)
		{
			switch (name)
			{
			case NameType.TeddyDonat:
			case NameType.RedRocksHot:
			case NameType.RealBurgers:
				return IconType.BuyRestaurant;
			case NameType.Ten10:
				return IconType.BuyGasStation;
			case NameType.StarbooksCoffee:
				return IconType.BuyRestaurant;
			case NameType.GrandRockMotel:
				return IconType.BuyMotel;
			default:
				return IconType.None;
			}
		}

		public bool BuyBusiness(int businessId)
		{
			AdvancedInt price = new AdvancedInt(200000);
			_currencyManagerController.TryMoneySpend(out var result, price, () =>
			{
				ObscuredPrefs.SetInt("FastFoodBuilding" + businessId, 1);
				V.CoreGameManager.SetMessage(new LocalizedString("FreeDriveControl.YouHaveABusiness"), 4f);
			});
			return result;
		}
	}
}
