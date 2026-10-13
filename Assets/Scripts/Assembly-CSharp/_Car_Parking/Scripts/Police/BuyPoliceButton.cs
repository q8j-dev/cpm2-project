using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.Police
{
	public class BuyPoliceButton : PoliceButtonBase
	{
		[SerializeField]
		private CombinedPriceView PriceView;

		private ManagerUI _managerUI;

		private PoliceSystemConfig _policeConfig;

		private CurrencyManagerController _currencyManagerController;

		[Inject]
		public void Construct(ManagerUI managerUI, PoliceSystemConfig policeConfig, CurrencyManagerController currencyManagerController)
		{
			_managerUI = managerUI;
			_policeConfig = policeConfig;
			_currencyManagerController = currencyManagerController;
		}

		protected override void ButtonOnClick()
		{
			base.ButtonOnClick();
			UpdateUI(isShow: false);
			ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData(new CombinedPrice(1000, 0));
			_managerUI.ShowConfirmWindow(() =>
			{
				TrySpendCoins();
			}, new LocalizedString("BuyPolice.Title"), new LocalizedString("BuyPolice.Description"), () =>
			{
				UpdateUI(isShow: true);
			}, "", null, additionalData).Forget();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if ((bool)PriceView)
			{
				PriceView.Init(new CombinedPrice(1000, 0));
			}
		}

		private void TrySpendCoins()
		{
			AdvancedInt price = new AdvancedInt(1000);
			_currencyManagerController.TryCoinsSpend(SpendCoinsType.Others, price, () =>
			{
				UpdateUI(isShow: true);
				if ((bool)V.Car)
				{
					policeSystem.TryBuy(V.Car.GetComponent<VehicleData>().carInfo);
				}
			}, () =>
			{
				UpdateUI(isShow: true);
			}).Forget();
		}

		private void UpdateUI(bool isShow)
		{
			WindowScript componentInParent = GetComponentInParent<WindowScript>();
			if ((bool)componentInParent)
			{
				componentInParent.ShowUI(isShow);
			}
		}
	}
}
