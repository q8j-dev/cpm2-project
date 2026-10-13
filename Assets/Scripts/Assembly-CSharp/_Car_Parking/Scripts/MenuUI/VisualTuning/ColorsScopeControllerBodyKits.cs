using CPM.Database;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.DataTypes;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class ColorsScopeControllerBodyKits : ColorsScopeController
	{
		public static BodyKitType[] PossibleBodyKitTypes = new BodyKitType[3]
		{
			BodyKitType.Spoiler,
			BodyKitType.AirIntakeHood,
			BodyKitType.AirIntakeRoof
		};

		private BodyKitType _currentBodyKitType;

		private BodyKits _bodyKits;

		private IPaintDataService _paintDataService;

		private CurrencyManagerController _currencyManagerController;

		private VisualTuningPricesService _visualTuningPricesService;

		public override ColorsScope.ColorsType ColorsType => ColorsScope.ColorsType.BodyKits;

		public ColorsScopeControllerBodyKits(ManagerUI managerUI, IPaintDataService paintDataService, CurrencyManagerController currencyManagerController, VisualTuningPricesService visualTuningPricesService)
			: base(managerUI)
		{
			_paintDataService = paintDataService;
			_currencyManagerController = currencyManagerController;
			_visualTuningPricesService = visualTuningPricesService;
		}

		public override void Enable(VehicleData vehicleData, CarInfo carInfo)
		{
			base.Enable(vehicleData, carInfo);
			_currentBodyKitType = BodyKitType.None;
			_bodyKits = base.vehicleData.gameObject.GetComponent<BodyKits>();
		}

		public override void Disable()
		{
			SetDefaultColors();
		}

		public override void SetColor(CarPaintColor carPaintColor)
		{
			bodyKitValues installedBodyKitValues = _bodyKits.GetInstalledBodyKitValues(_currentBodyKitType);
			if ((bool)installedBodyKitValues && (bool)installedBodyKitValues.MainMaterial)
			{
				ColorsService.SetCarPaintColor(installedBodyKitValues.MainMaterial, carPaintColor);
			}
		}

		public override void Apply(CarPaintColor carPaintColor)
		{
			if (_paintDataService.CheckPaintBought(carPaintColor.CarMatType))
			{
				SaveColor(carPaintColor);
				return;
			}
			AdvancedInt price = new AdvancedInt(_visualTuningPricesService.GetColorPrice());
			_currencyManagerController.TryCoinsSpend(SpendCoinsType.Colors, price, () =>
			{
				_paintDataService.SetPaintBought(carPaintColor.CarMatType);
				SaveColor(carPaintColor);
			}, null, allowZero: false);
		}

		public override void Revert()
		{
			SetDefaultColors();
		}

		public override void Discard()
		{
			SetDefaultColors();
			managerUI.ShowNotificationWindow(new LocalizedString("ColorsScope.Detailinstalled!"), 3f, NotificationType.Success, SoundEventsDataHelper.InstallPaintAcceptSwitch).Forget();
		}

		public void SetBodyKitType(BodyKitType newBodyKitType)
		{
			_currentBodyKitType = newBodyKitType;
			SetDefaultColors();
		}

		public override void SetDefaultColor()
		{
			carInfo.Colors.BodyKits.Remove(_currentBodyKitType);
			SetDefaultColors();
			carInfo.Save();
			managerUI.ShowNotificationWindow(new LocalizedString("ColorsScope.Detailinstalled!"), 3f, NotificationType.Success, SoundEventsDataHelper.InstallPaintAcceptSwitch).Forget();
		}

		private void SetDefaultColors()
		{
			BodyKitType[] possibleBodyKitTypes = PossibleBodyKitTypes;
			foreach (BodyKitType bodyKitType in possibleBodyKitTypes)
			{
				bodyKitValues installedBodyKitValues = _bodyKits.GetInstalledBodyKitValues(bodyKitType);
				if ((bool)installedBodyKitValues)
				{
					installedBodyKitValues.ResetMainMaterial();
				}
			}
		}

		private void SaveColor(CarPaintColor carPaintColor)
		{
			carInfo.Colors.BodyKits.Set(_currentBodyKitType, carPaintColor);
			carInfo.Save();
			managerUI.ShowNotificationWindow(new LocalizedString("BodyKitsVisualScope.Detailinstalled!"), 3f, NotificationType.Success, SoundEventsDataHelper.InstallVisualTuningSwitch).Forget();
		}
	}
}
