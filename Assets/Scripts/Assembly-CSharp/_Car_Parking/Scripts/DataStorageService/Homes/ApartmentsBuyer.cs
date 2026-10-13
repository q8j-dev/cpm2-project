using System;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.DataStorageService.Homes
{
	public class ApartmentsBuyer
	{
		public static event Action<int> HouseBoughtFree;

		public static async UniTask<bool> BuyHouseFromMenu(CurrencyManagerController currencyManagerController, HomesConfig.HomeDataItem homeDataItem, Action _result = null)
		{
			bool result = false;
			GlobalCost getGlobalCost = homeDataItem.GetGlobalCost;
			switch (getGlobalCost.PriceType)
			{
			case CostType.Money:
			{
				AdvancedInt price2 = new AdvancedInt(getGlobalCost.Price);
				currencyManagerController.TryMoneySpend(out result, price2, () =>
				{
					OpenHome(homeDataItem, _result);
				});
				break;
			}
			case CostType.Coins:
			{
				AdvancedInt price = new AdvancedInt(getGlobalCost.Price);
				(await currencyManagerController.TryCoinsSpend(SpendCoinsType.Apartment, price, () =>
				{
					OpenHome(homeDataItem, _result);
				}, null, allowZero: false)).Get(out result);
				break;
			}
			}
			return result;
		}

		private static void OpenHome(HomesConfig.HomeDataItem homeDataItem, Action _result = null)
		{
			ObscuredPrefs.SetInt("homeOpen" + homeDataItem.Index, 1);
			V.CoreGameManager.SetMessage(new LocalizedString("ApartmentsBuyer.YouHaveAHouse"), 5f);
			_result?.Invoke();
		}

		public static bool IsHomeBought(int id)
		{
			return ObscuredPrefs.GetInt("homeOpen" + id) == 1;
		}

		public static async UniTask<bool> BuyHomeInFree(CurrencyManagerController currencyManagerController, HomesConfig.HomeDataItem homeDataItem)
		{
			bool result = false;
			switch (homeDataItem.PriceType)
			{
			case CostType.Money:
			{
				AdvancedInt price2 = new AdvancedInt(homeDataItem.GetPrice);
				currencyManagerController.TryMoneySpend(out result, price2, () =>
				{
					ObscuredPrefs.SetInt("homeOpen" + homeDataItem.Index, 1);
					V.CoreGameManager.SetMessage(new LocalizedString("ApartmentsBuyer.YouHaveANewHouse"), 4f);
					V.SceneControl.HomeBuy(homeDataItem.Index);
					if (homeDataItem.Index == 3)
					{
						V.SceneControl.CreateDog(0);
					}
				});
				break;
			}
			case CostType.Coins:
			{
				AdvancedInt price = new AdvancedInt(homeDataItem.GetPrice);
				(await currencyManagerController.TryCoinsSpend(SpendCoinsType.Apartment, price, () =>
				{
					ObscuredPrefs.SetInt("homeOpen" + homeDataItem.Index, 1);
					V.CoreGameManager.SetMessage(new LocalizedString("ApartmentsBuyer.YouHaveANewHouse"), 4f);
					V.SceneControl.HomeBuy(homeDataItem.Index);
					if (homeDataItem.Index == 3)
					{
						V.SceneControl.CreateDog(0);
					}
				}, null, allowZero: false)).Get(out result);
				break;
			}
			}
			if (result)
			{
				HouseBoughtFree?.Invoke(homeDataItem.Index);
			}
			return result;
		}
	}
}
