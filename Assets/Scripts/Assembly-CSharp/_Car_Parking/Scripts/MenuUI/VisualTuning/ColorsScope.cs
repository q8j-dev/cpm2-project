using System.Collections.Generic;
using CPM.Database;
using CPM.Database.Constructors;
using Cysharp.Threading.Tasks;
using Racing.CameraManagement;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.DataTypes;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class ColorsScope : VisualTuningScope
	{
		public enum ColorsType
		{
			Car = 1,
			Wheels = 2,
			Windows = 3,
			BodyKits = 4,
			Smoke = 5,
			None = 99
		}

		public enum WheelsColorType
		{
			Both1 = 0,
			Front1 = 1,
			Rear1 = 2,
			Both2 = 3,
			Front2 = 4,
			Rear2 = 5,
			TextColor = 6,
			Both = 7,
			Front = 8,
			Rear = 9
		}

		private VehicleData _vehicleData;

		private IColorPicker _colorPicker;

		private string colorPrevTitle;

		private string colorTitle;

		private ColorsService _colorsService;

		private ColorsType _activeColorType;

		private CarPaintColor _lastCarPaintColor;

		private CarPaintColor _wheelFirstColor;

		private CarPaintColor _wheelSecondColor;

		private ICarInfoService _carInfoService;

		private CarInfo _carInfo;

		private WheelsColorType _activeWheelColorType;

		private BodyKitType _activeBodyKit;

		private CameraManager _cameraManager;

		private CurrencyManagerController _currencyManagerController;

		private IPaintDataService _paintDataService;

		private VisualTuningPricesService _visualTuningPricesService;

		private ManagerUI _managerUI;

		private List<ColorsScopeController> _scopeControllers = new List<ColorsScopeController>();

		private ColorsScopeController _currentScopeController;

		private ISteamAchievementService _steamAchievementService;

		private readonly HashSet<CarMatType> _appliedMatTypes = new HashSet<CarMatType>();

		public int CurrentIndexColor { get; set; }

		public int SubIndexColor { get; set; }

		[Inject]
		public void Construct(ColorsService colorsService, ICarInfoService carInfoService, CameraManager cameraManager, CurrencyManagerController currencyManagerController, IPaintDataService paintDataService, VisualTuningPricesService visualTuningPricesService, ManagerUI managerUI, ColorsScopeSmokeConfig colorsScopeSmokeConfig, ISteamAchievementService steamAchievementService)
		{
			_steamAchievementService = steamAchievementService;
			_managerUI = managerUI;
			_visualTuningPricesService = visualTuningPricesService;
			_paintDataService = paintDataService;
			_currencyManagerController = currencyManagerController;
			_cameraManager = cameraManager;
			_carInfoService = carInfoService;
			_colorsService = colorsService;
			_scopeControllers.Add(new ColorsScopeControllerSmoke(colorsScopeSmokeConfig, managerUI));
			_scopeControllers.Add(new ColorsScopeControllerBodyKits(managerUI, _paintDataService, _currencyManagerController, _visualTuningPricesService));
		}

		public override void Init()
		{
			_vehicleData = V.Car.GetComponent<VehicleData>();
			_activeColorType = ColorsType.None;
			_carInfo = _carInfoService.GetFromSlot(V.SlotID);
			RevertLastCarColor(_carInfo);
			_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, _vehicleData.transform);
			_appliedMatTypes.Clear();
		}

		public override void Apply()
		{
			CarPaintColor paintColor = _lastCarPaintColor;
			WheelsColorType wheelColorType = _activeWheelColorType;
			CarPaintColor wheelFirstColor = _wheelFirstColor;
			CarPaintColor wheelSecondColor = _wheelSecondColor;
			CarInfo carInfo = _carInfo;
			switch (_activeColorType)
			{
			case ColorsType.Car:
				if (_paintDataService.CheckPaintBought(paintColor.CarMatType))
				{
					SaveCarColor(paintColor, carInfo);
					break;
				}
				VisualItemPurchase.Purchase(_currencyManagerController, SpendCoinsType.Colors, (int)paintColor.CarMatType, _visualTuningPricesService.GetColorPrice(), alreadyOwned: false, () =>
				{
					_paintDataService.SetPaintBought(paintColor.CarMatType);
					SaveCarColor(paintColor, carInfo);
				});
				break;
			case ColorsType.Wheels:
				if (_paintDataService.CheckPaintBought(paintColor.CarMatType))
				{
					SaveWheelsColor(wheelColorType, paintColor, carInfo, wheelFirstColor, wheelSecondColor);
					break;
				}
				VisualItemPurchase.Purchase(_currencyManagerController, SpendCoinsType.Colors, (int)paintColor.CarMatType, _visualTuningPricesService.GetColorPrice(), alreadyOwned: false, () =>
				{
					_paintDataService.SetPaintBought(paintColor.CarMatType);
					SaveWheelsColor(wheelColorType, paintColor, carInfo, wheelFirstColor, wheelSecondColor);
				});
				break;
			case ColorsType.Windows:
				SaveWindowColor(paintColor, carInfo);
				break;
			}
			_currentScopeController?.Apply(paintColor);
		}

		public override void Revert()
		{
			_lastCarPaintColor = ((_carInfo != null && _carInfo.Colors != null) ? _carInfo.Colors.CarColor : CarPaintColor.Default);
			switch (_activeColorType)
			{
			case ColorsType.Car:
				_colorsService.CreateFirstMat(_vehicleData);
				RevertLastCarColor(_carInfo);
				break;
			case ColorsType.Wheels:
				RevertWheelColors(_carInfo, _vehicleData);
				RevertLastWheelPaintColor(_carInfo);
				break;
			case ColorsType.Windows:
				RevertWindowColor(_carInfo);
				break;
			}
			_currentScopeController?.Revert();
		}

		private void RevertLastWheelPaintColor(CarInfo carInfo)
		{
			if (carInfo?.TuningData == null)
			{
				return;
			}
			WheelData frontWheelData = carInfo.TuningData.FrontWheelData;
			WheelData rearWheelData = carInfo.TuningData.RearWheelData;
			if (frontWheelData != null && rearWheelData != null)
			{
				CarPaintColor lastCarPaintColor;
				switch (_activeWheelColorType)
				{
				case WheelsColorType.Rear1:
				case WheelsColorType.Rear:
					lastCarPaintColor = rearWheelData.Color1;
					break;
				case WheelsColorType.Rear2:
					lastCarPaintColor = rearWheelData.Color2;
					break;
				case WheelsColorType.Both2:
				case WheelsColorType.Front2:
					lastCarPaintColor = frontWheelData.Color2;
					break;
				default:
					lastCarPaintColor = frontWheelData.Color1;
					break;
				}
				_lastCarPaintColor = lastCarPaintColor;
			}
		}

		public override void Discard()
		{
			if (_activeColorType != ColorsType.Smoke)
			{
				base.Discard();
			}
			_currentScopeController?.Discard();
		}

		public void SetDefaultColor(VehicleStockTuningData vehicleStockTuningData, WheelsColorType wheelsColorType)
		{
			switch (_activeColorType)
			{
			case ColorsType.Car:
				_lastCarPaintColor = vehicleStockTuningData.CarInfoDefault.Colors.CarColor;
				SelectCarColor(_lastCarPaintColor);
				_carInfo.Colors.CarColor = _lastCarPaintColor;
				break;
			case ColorsType.Wheels:
				switch (wheelsColorType)
				{
				case WheelsColorType.Both1:
				case WheelsColorType.Both2:
				case WheelsColorType.Both:
					_lastCarPaintColor = vehicleStockTuningData.Tuning.FrontWheelData.Color1;
					ColorsService.SetCarPaintColor(_vehicleData.WheelComposition.WheelMatFront1, _carInfo.TuningData.FrontWheelData.Color1 = vehicleStockTuningData.Tuning.FrontWheelData.Color1);
					ColorsService.SetCarPaintColor(_vehicleData.WheelComposition.WheelMatFront2, _carInfo.TuningData.FrontWheelData.Color2 = vehicleStockTuningData.Tuning.FrontWheelData.Color2);
					PaintRearRims1(_vehicleData, _carInfo.TuningData.RearWheelData.Color1 = vehicleStockTuningData.Tuning.RearWheelData.Color1);
					PaintRearRims2(_vehicleData, _carInfo.TuningData.RearWheelData.Color2 = vehicleStockTuningData.Tuning.RearWheelData.Color2);
					break;
				case WheelsColorType.Front1:
				case WheelsColorType.Front2:
					_lastCarPaintColor = vehicleStockTuningData.Tuning.FrontWheelData.Color1;
					ColorsService.SetCarPaintColor(_vehicleData.WheelComposition.WheelMatFront1, _carInfo.TuningData.FrontWheelData.Color1 = vehicleStockTuningData.Tuning.FrontWheelData.Color1);
					ColorsService.SetCarPaintColor(_vehicleData.WheelComposition.WheelMatFront2, _carInfo.TuningData.FrontWheelData.Color2 = vehicleStockTuningData.Tuning.FrontWheelData.Color2);
					break;
				case WheelsColorType.Rear1:
				case WheelsColorType.Rear2:
					_lastCarPaintColor = vehicleStockTuningData.Tuning.RearWheelData.Color2;
					PaintRearRims1(_vehicleData, _carInfo.TuningData.RearWheelData.Color1 = vehicleStockTuningData.Tuning.RearWheelData.Color1);
					PaintRearRims2(_vehicleData, _carInfo.TuningData.RearWheelData.Color2 = vehicleStockTuningData.Tuning.RearWheelData.Color2);
					break;
				case WheelsColorType.TextColor:
					_lastCarPaintColor = CarPaintColor.Default;
					WheelConstructor.DynamicCanvasTire.SetTextColor(Color.white);
					break;
				}
				break;
			case ColorsType.Windows:
			{
				_lastCarPaintColor = CarPaintColor.Default;
				Color color = new Color(0f, 0f, 0f, 0f);
				_lastCarPaintColor.Color = color;
				_vehicleData.WindowComposition.WindowMat.color = color;
				_carInfo.Colors.WindowColor = color;
				break;
			}
			}
			_currentScopeController?.SetDefaultColor();
			_carInfo.Save();
		}

		public void SetColorType(ColorsType newColorsType)
		{
			Revert();
			_activeColorType = newColorsType;
		}

		public void SetWheelColorType(ColorsType newColorsType, WheelsColorType wheelsColorType)
		{
			Revert();
			_activeColorType = newColorsType;
			_activeWheelColorType = wheelsColorType;
			switch (wheelsColorType)
			{
			case WheelsColorType.Both1:
				_lastCarPaintColor = _carInfo.TuningData.FrontWheelData.Color1;
				break;
			case WheelsColorType.Both2:
				_lastCarPaintColor = _carInfo.TuningData.FrontWheelData.Color2;
				break;
			case WheelsColorType.Front1:
				_lastCarPaintColor = _carInfo.TuningData.FrontWheelData.Color1;
				break;
			case WheelsColorType.Front2:
				_lastCarPaintColor = _carInfo.TuningData.FrontWheelData.Color2;
				break;
			case WheelsColorType.Rear1:
				_lastCarPaintColor = _carInfo.TuningData.RearWheelData.Color1;
				break;
			case WheelsColorType.Rear2:
				_lastCarPaintColor = _carInfo.TuningData.RearWheelData.Color2;
				break;
			case WheelsColorType.TextColor:
				break;
			}
		}

		public override void Select(int i)
		{
		}

		public void SetActiveColorType(ColorsType colorsType)
		{
			_activeColorType = colorsType;
		}

		public void SelectBodyKitType(BodyKitType bodyKit)
		{
			if (_currentScopeController != null && _currentScopeController is ColorsScopeControllerBodyKits)
			{
				(_currentScopeController as ColorsScopeControllerBodyKits).SetBodyKitType(bodyKit);
			}
		}

		public void SelectBodyKitColor(CarPaintColor bodyKitColor)
		{
			SetController(ColorsType.BodyKits, bodyKitColor);
			_activeColorType = ColorsType.BodyKits;
		}

		public void SelectCarColor(CarPaintColor carPaintColor)
		{
			SetController(ColorsType.Car, carPaintColor);
			_activeColorType = ColorsType.Car;
			BodyKits component = V.Car.GetComponent<BodyKits>();
			if ((bool)component)
			{
				_lastCarPaintColor = carPaintColor;
				if (_carInfo.Colors.BodyKits.TryGet(BodyKitType.FrontBumper, out var color) && (bool)component.InstalledFrontBumper)
				{
					SetPaintColor(component.InstalledFrontBumper);
				}
				if (_carInfo.Colors.BodyKits.TryGet(BodyKitType.RearBumper, out color) && (bool)component.InstalledRearBumper)
				{
					SetPaintColor(component.InstalledRearBumper);
				}
				if (_carInfo.Colors.BodyKits.TryGet(BodyKitType.Skirt, out color) && (bool)component.installedSkirts)
				{
					SetPaintColor(component.installedSkirts);
				}
				if (_carInfo.Colors.BodyKits.TryGet(BodyKitType.Hood, out color) && (bool)component.installedHood)
				{
					SetPaintColor(component.installedHood);
				}
				if (_carInfo.Colors.BodyKits.TryGet(BodyKitType.Fender, out color) && (bool)component.installedFender)
				{
					SetPaintColor(component.installedFender);
				}
				ColorsService.SetCarPaintColor(_vehicleData.CarMat, _lastCarPaintColor);
				_vehicleData.OnCarColorChanged(_lastCarPaintColor.Color);
			}
		}

		private void SetPaintColor(GameObject _obj)
		{
			bodyKitValues component = _obj.GetComponent<bodyKitValues>();
			if ((bool)component)
			{
				ColorsService.SetCarPaintColor(component.MainMaterial, _lastCarPaintColor);
			}
		}

		public void SaveCarColor(CarPaintColor colorComplex, CarInfo carInfo)
		{
			carInfo.Colors.CarColor = colorComplex;
			carInfo.Save();
			TrackAppliedMaterial(colorComplex.CarMatType, carInfo);
			_managerUI.ShowNotificationWindow(new LocalizedString("ColorsScope.Detailinstalled!"), 3f, NotificationType.Success, SoundEventsDataHelper.InstallPaintAcceptSwitch).Forget();
			InvokeOnValuesChanged();
		}

		private void TrackAppliedMaterial(CarMatType matType, CarInfo carInfo)
		{
			if (carInfo != null && !string.IsNullOrEmpty(carInfo.CarInstanceId))
			{
				GooglePlayAchievements.ReportPaintMaterial(carInfo.CarInstanceId, (int)matType, "paint_tuning");
			}
		}

		private void TrackAppliedMaterialForSteam(CarMatType matType)
		{
			_appliedMatTypes.Add(matType);
			if (_appliedMatTypes.Count >= 3)
			{
				_steamAchievementService?.Unlock("ACH_PAINT_CHEMISTRY");
			}
		}

		private void RevertLastCarColor(CarInfo carInfo)
		{
			_lastCarPaintColor = carInfo.Colors.CarColor;
			ColorsService.SetCarPaintColor(_vehicleData.CarMat, _lastCarPaintColor);
			_vehicleData.OnCarColorChanged(_lastCarPaintColor.Color);
		}

		public void SelectWheelColor(CarPaintColor color, WheelsColorType wheelsColorType)
		{
			SetController(ColorsType.Wheels, color);
			_activeColorType = ColorsType.Wheels;
			if ((wheelsColorType == WheelsColorType.Both || wheelsColorType == WheelsColorType.Both1 || wheelsColorType == WheelsColorType.Both2) && _activeWheelColorType != wheelsColorType)
			{
				_activeWheelColorType = wheelsColorType;
				return;
			}
			_activeWheelColorType = wheelsColorType;
			SetWheelColor(wheelsColorType, color);
		}

		public void SaveWheelsColor(WheelsColorType activeWheelColorType, CarPaintColor lastColor, CarInfo carInfo)
		{
			SaveWheelsColor(activeWheelColorType, lastColor, carInfo, _wheelFirstColor, _wheelSecondColor);
		}

		private void SaveWheelsColor(WheelsColorType activeWheelColorType, CarPaintColor lastColor, CarInfo carInfo, CarPaintColor wheelFirstColor, CarPaintColor wheelSecondColor)
		{
			switch (activeWheelColorType)
			{
			case WheelsColorType.Both:
				carInfo.TuningData.FrontWheelData.Color1 = lastColor;
				carInfo.TuningData.RearWheelData.Color1 = lastColor;
				carInfo.TuningData.FrontWheelData.Color2 = lastColor;
				carInfo.TuningData.RearWheelData.Color2 = lastColor;
				break;
			case WheelsColorType.Front:
				carInfo.TuningData.FrontWheelData.Color1 = lastColor;
				carInfo.TuningData.FrontWheelData.Color2 = lastColor;
				break;
			case WheelsColorType.Rear:
				carInfo.TuningData.RearWheelData.Color1 = lastColor;
				carInfo.TuningData.RearWheelData.Color2 = lastColor;
				break;
			case WheelsColorType.Both1:
				carInfo.TuningData.FrontWheelData.Color1 = lastColor;
				carInfo.TuningData.RearWheelData.Color1 = lastColor;
				break;
			case WheelsColorType.Both2:
				carInfo.TuningData.FrontWheelData.Color2 = lastColor;
				carInfo.TuningData.RearWheelData.Color2 = lastColor;
				break;
			case WheelsColorType.Front1:
				carInfo.TuningData.FrontWheelData.Color1 = wheelFirstColor;
				carInfo.TuningData.FrontWheelData.Color2 = wheelSecondColor;
				break;
			case WheelsColorType.Front2:
				carInfo.TuningData.FrontWheelData.Color2 = wheelSecondColor;
				carInfo.TuningData.FrontWheelData.Color1 = wheelFirstColor;
				break;
			case WheelsColorType.Rear1:
				carInfo.TuningData.RearWheelData.Color1 = wheelFirstColor;
				carInfo.TuningData.RearWheelData.Color2 = wheelSecondColor;
				break;
			case WheelsColorType.Rear2:
				carInfo.TuningData.RearWheelData.Color2 = wheelSecondColor;
				carInfo.TuningData.RearWheelData.Color1 = wheelFirstColor;
				break;
			}
			carInfo.Save();
			TrackAppliedMaterialForSteam(lastColor.CarMatType);
			_managerUI.ShowNotificationWindow(new LocalizedString("ColorsScope.Detailinstalled!"), 3f, NotificationType.Success, SoundEventsDataHelper.InstallPaintAcceptSwitch).Forget();
			InvokeOnValuesChanged();
		}

		public void SetWheelColor(WheelsColorType wheelsColorType, CarPaintColor color)
		{
			_lastCarPaintColor = color;
			switch (wheelsColorType)
			{
			case WheelsColorType.Both:
				ColorsService.SetCarPaintColor(_vehicleData.WheelComposition.WheelMatFront1, color);
				PaintRearRims1(_vehicleData, color);
				ColorsService.SetCarPaintColor(_vehicleData.WheelComposition.WheelMatFront2, color);
				PaintRearRims2(_vehicleData, color);
				break;
			case WheelsColorType.Front:
				ColorsService.SetCarPaintColor(_vehicleData.WheelComposition.WheelMatFront1, color);
				ColorsService.SetCarPaintColor(_vehicleData.WheelComposition.WheelMatFront2, color);
				break;
			case WheelsColorType.Rear:
				PaintRearRims1(_vehicleData, color);
				PaintRearRims2(_vehicleData, color);
				break;
			case WheelsColorType.Both1:
				ColorsService.SetCarPaintColor(_vehicleData.WheelComposition.WheelMatFront1, color);
				PaintRearRims1(_vehicleData, color);
				break;
			case WheelsColorType.Front1:
				ColorsService.SetCarPaintColor(_vehicleData.WheelComposition.WheelMatFront1, color);
				_wheelFirstColor = color;
				break;
			case WheelsColorType.Rear1:
				PaintRearRims1(_vehicleData, color);
				_wheelFirstColor = color;
				break;
			case WheelsColorType.Both2:
				ColorsService.SetCarPaintColor(_vehicleData.WheelComposition.WheelMatFront2, color);
				PaintRearRims2(_vehicleData, color);
				break;
			case WheelsColorType.Front2:
				ColorsService.SetCarPaintColor(_vehicleData.WheelComposition.WheelMatFront2, color);
				_wheelSecondColor = color;
				break;
			case WheelsColorType.Rear2:
				PaintRearRims2(_vehicleData, color);
				_wheelSecondColor = color;
				break;
			case WheelsColorType.TextColor:
				WheelConstructor.DynamicCanvasTire.SetTextColor(color.Color);
				break;
			}
		}

		private static void PaintRearRims1(VehicleData vehicleData, CarPaintColor color)
		{
			ColorsService.SetCarPaintColor(vehicleData.WheelComposition.WheelMatRear1, color);
			foreach (Material item in vehicleData.WheelComposition.ExtraRearMaterials1)
			{
				ColorsService.SetCarPaintColor(item, color);
			}
		}

		private static void PaintRearRims2(VehicleData vehicleData, CarPaintColor color)
		{
			ColorsService.SetCarPaintColor(vehicleData.WheelComposition.WheelMatRear2, color);
			foreach (Material item in vehicleData.WheelComposition.ExtraRearMaterials2)
			{
				ColorsService.SetCarPaintColor(item, color);
			}
		}

		public void RevertWheelColors(CarInfo carInfo, VehicleData vehicleData)
		{
			ColorsService.SetCarPaintColor(vehicleData.WheelComposition.WheelMatFront1, carInfo.TuningData.FrontWheelData.Color1);
			ColorsService.SetCarPaintColor(vehicleData.WheelComposition.WheelMatFront2, carInfo.TuningData.FrontWheelData.Color2);
			PaintRearRims1(vehicleData, carInfo.TuningData.RearWheelData.Color1);
			PaintRearRims2(vehicleData, carInfo.TuningData.RearWheelData.Color2);
		}

		public void SelectWindowColor(CarPaintColor color)
		{
			SetController(ColorsType.Windows, color);
			_activeColorType = ColorsType.Windows;
			_lastCarPaintColor = color;
			_vehicleData.WindowComposition.WindowMat.color = color.Color;
		}

		private void SaveWindowColor(CarPaintColor lastCarPaintColor, CarInfo carInfo)
		{
			carInfo.Colors.WindowColor = lastCarPaintColor.Color;
			carInfo.Save();
			_managerUI.ShowNotificationWindow(new LocalizedString("ColorsScope.Detailinstalled!"), 3f, NotificationType.Success, SoundEventsDataHelper.InstallPaintAcceptSwitch).Forget();
		}

		private void RevertWindowColor(CarInfo carInfo)
		{
			CarPaintColor lastCarPaintColor = CarPaintColor.Default;
			lastCarPaintColor.Color = carInfo.Colors.WindowColor;
			_lastCarPaintColor = lastCarPaintColor;
			_vehicleData.WindowComposition.WindowMat.color = carInfo.Colors.WindowColor;
		}

		public void SelectSmokeColor(CarPaintColor color)
		{
			SetController(ColorsType.Smoke, color);
			_activeColorType = ColorsType.Smoke;
		}

		private void SetController(ColorsType colorsType, CarPaintColor carPaintColor)
		{
			_lastCarPaintColor = carPaintColor;
			if (_currentScopeController != null && _currentScopeController.ColorsType == colorsType)
			{
				_currentScopeController.SetColor(carPaintColor);
				return;
			}
			_currentScopeController?.Disable();
			_currentScopeController = null;
			for (int i = 0; i < _scopeControllers.Count; i++)
			{
				if (_scopeControllers[i].ColorsType == colorsType)
				{
					_currentScopeController = _scopeControllers[i];
				}
			}
			_currentScopeController?.Enable(_vehicleData, _carInfo);
			_currentScopeController?.SetColor(carPaintColor);
		}
	}
}
