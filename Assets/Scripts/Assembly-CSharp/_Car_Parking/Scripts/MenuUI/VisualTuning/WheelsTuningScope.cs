using System.Linq;
using CPM.Database;
using CPM.Database.Constructors;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Racing.CameraManagement;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class WheelsTuningScope : VisualTuningScope
	{
		public const int RimSizeCount = 7;

		private CurrencyManagerController _currencyManagerController;

		private WheelsDiskTuningConfig _wheelsDiskTuningConfig;

		private FrontWheelData CurrentFrontWheelData;

		private RearWheelData CurrentRearWheelData;

		private ObscuredInt _carId;

		private GameObject _car;

		private VehicleData _vehicleData;

		private Powertrain _powertrain;

		private DynamicCanvasTire _dynamicCanvasTire;

		private ObscuredInt _carSlot;

		private CarInfo _carInfo;

		private ICarInfoService _carInfoService;

		private WheelTuningAxleType _axleType = WheelTuningAxleType.Both;

		private int _lastFontTireSelected;

		private IWheelsDataService _wheelsDataService;

		private CameraManager _cameraManager;

		private MenuControl _menuControl;

		private ManagerUI _managerUI;

		private CarDefaultTuningDataConfig _defaultTuningDataConfig;

		private ICarTuningFactory _carTuningFactory;

		private string _notificationMessage;

		private ISteamAchievementService _steamAchievementService;

		private WheelData CurrentWheelData
		{
			get
			{
				if (_axleType != WheelTuningAxleType.Front && _axleType != WheelTuningAxleType.Both)
				{
					return CurrentRearWheelData;
				}
				return CurrentFrontWheelData;
			}
		}

		[Inject]
		public void Construct(ManagerUI managerUI, MenuControl menuControl, CameraManager cameraManager, IWheelsDataService wheelsDataService, ICarInfoService carInfoService, WheelsDiskTuningConfig wheelsDiskTuningConfig, CurrencyManagerController currencyManagerController, ICarTuningFactory carTuningFactory, ISteamAchievementService steamAchievementService)
		{
			_managerUI = managerUI;
			_menuControl = menuControl;
			_cameraManager = cameraManager;
			_wheelsDataService = wheelsDataService;
			_carInfoService = carInfoService;
			_wheelsDiskTuningConfig = wheelsDiskTuningConfig;
			_currencyManagerController = currencyManagerController;
			_carTuningFactory = carTuningFactory;
			_steamAchievementService = steamAchievementService;
		}

		public override void Init()
		{
			_car = V.Car;
			_carId = V.CarId;
			_carSlot = V.SlotID;
			_vehicleData = V.Car.GetComponent<VehicleData>();
			_powertrain = V.Car.GetComponent<Powertrain>();
			_dynamicCanvasTire = WheelConstructor.DynamicCanvasTire;
			_carInfo = _carInfoService.GetFromSlot(V.SlotID);
			VehicleStockTuningData vehicleStockTuningData = _carTuningFactory.CreateDefault(_carInfo.CarID);
			_defaultTuningDataConfig = vehicleStockTuningData.Tuning;
			SetCamera();
			RevertCurrent();
		}

		public override void Apply()
		{
			Debug.Log("Try apply wheel.");
			int num = ((_axleType == WheelTuningAxleType.Front || _axleType == WheelTuningAxleType.Both) ? _carInfo.TuningData.FrontWheelData.RimModelId : _carInfo.TuningData.RearWheelData.RimModelId);
			if (CurrentWheelData.RimModelId != num)
			{
				ApplyThisWheel(CurrentWheelData.RimModelId);
				return;
			}
			_notificationMessage = new LocalizedString("BodyKitsVisualScope.Detailinstalled!");
			ApplyFull();
		}

		public override void Revert()
		{
			Debug.Log("Try Revert wheel.");
			if ((bool)_car && !(_car != V.Car))
			{
				WheelConstructor.SetWheel(_car.GetComponent<VehicleData>(), _carInfo.TuningData).Forget();
				RevertCurrent();
				_powertrain.SetWheelParams(AxleType.Front, _carInfo.TuningData.FrontWheelData);
				_powertrain.SetWheelParams(AxleType.Rear, _carInfo.TuningData.RearWheelData);
			}
		}

		public static WheelDiskData GetWheelData(int index, WheelDiskData[] oneWheelData)
		{
			return oneWheelData.FirstOrDefault((WheelDiskData wData) => index == wData.index);
		}

		public static BrakeDiskItemData GetBrakeDataData(int index, BrakeDiskItemData[] oneBrakeDatas)
		{
			return oneBrakeDatas.FirstOrDefault((BrakeDiskItemData wData) => index == wData.index);
		}

		public void SetAxleType(WheelTuningAxleType axleType)
		{
			_axleType = axleType;
			Revert();
			Observable.TimerFrame(1).Subscribe((long _) =>
			{
				SetCamera();
			}).AddTo(_car);
		}

		public void SelectTyreWidth(TyreWidth select)
		{
			CurrentWheelData.tyreWidth = select;
			if (_axleType == WheelTuningAxleType.Both)
			{
				_powertrain.SetWheelParams(AxleType.Front, CurrentWheelData);
				WheelConstructor.SetWheelVisual(AxleType.Front, V.Car, CurrentWheelData);
				CurrentFrontWheelData.tyreWidth = select;
				_powertrain.SetWheelParams(AxleType.Rear, CurrentWheelData);
				WheelConstructor.SetWheelVisual(AxleType.Rear, V.Car, CurrentWheelData);
				CurrentRearWheelData.tyreWidth = select;
			}
			else
			{
				_powertrain.SetWheelParams((AxleType)_axleType, CurrentWheelData);
				WheelConstructor.SetWheelVisual((AxleType)_axleType, V.Car, CurrentWheelData);
			}
		}

		private void ClampPressureToTyreType(AxleType axle, WheelData data, TyreType type)
		{
			float pressure = data.Pressure;
			data.Pressure = TyreTypeRanges.ClampPressureToType(type, data.Pressure);
			if (!Mathf.Approximately(pressure, data.Pressure))
			{
				_powertrain.SetTyrePressure(axle, data.Pressure);
			}
		}

		public void SelectTyreType(TyreType select)
		{
			CurrentWheelData.tyreType = select;
			if (_axleType == WheelTuningAxleType.Both)
			{
				_powertrain.SetWheelParams(AxleType.Front, CurrentWheelData);
				WheelConstructor.SetWheelVisual(AxleType.Front, V.Car, CurrentWheelData, updateTyreType: true);
				CurrentFrontWheelData.tyreType = select;
				CurrentRearWheelData.tyreType = select;
				WheelData wheelData = (F1TyreRules.IsF1(_carId) ? CurrentRearWheelData : CurrentWheelData);
				_powertrain.SetWheelParams(AxleType.Rear, wheelData);
				WheelConstructor.SetWheelVisual(AxleType.Rear, V.Car, wheelData, updateTyreType: true);
				ClampPressureToTyreType(AxleType.Front, CurrentFrontWheelData, select);
				ClampPressureToTyreType(AxleType.Rear, CurrentRearWheelData, select);
			}
			else
			{
				ClampPressureToTyreType((AxleType)_axleType, CurrentWheelData, select);
				_powertrain.SetWheelParams((AxleType)_axleType, CurrentWheelData);
				WheelConstructor.SetWheelVisual((AxleType)_axleType, V.Car, CurrentWheelData, updateTyreType: true);
			}
		}

		public void SelectTyreProfile(TyreProfile select)
		{
			CurrentWheelData.tyreProfile = select;
			if (_axleType == WheelTuningAxleType.Both)
			{
				_powertrain.SetWheelParams(AxleType.Front, CurrentWheelData);
				WheelConstructor.SetWheelVisual(AxleType.Front, V.Car, CurrentWheelData);
				CurrentFrontWheelData.tyreProfile = select;
				_powertrain.SetWheelParams(AxleType.Rear, CurrentWheelData);
				WheelConstructor.SetWheelVisual(AxleType.Rear, V.Car, CurrentWheelData);
				CurrentRearWheelData.tyreProfile = select;
			}
			else
			{
				_powertrain.SetWheelParams((AxleType)_axleType, CurrentWheelData);
				WheelConstructor.SetWheelVisual((AxleType)_axleType, V.Car, CurrentWheelData);
			}
		}

		public void SelectRimWidth(RimWidth select)
		{
			CurrentWheelData.rimWidth = select;
			if (_axleType == WheelTuningAxleType.Both)
			{
				_powertrain.SetWheelParams(AxleType.Front, CurrentWheelData);
				WheelConstructor.SetWheelVisual(AxleType.Front, V.Car, CurrentWheelData);
				CurrentFrontWheelData.rimWidth = select;
				_powertrain.SetWheelParams(AxleType.Rear, CurrentWheelData);
				WheelConstructor.SetWheelVisual(AxleType.Rear, V.Car, CurrentWheelData);
				CurrentRearWheelData.rimWidth = select;
			}
			else
			{
				_powertrain.SetWheelParams((AxleType)_axleType, CurrentWheelData);
				WheelConstructor.SetWheelVisual((AxleType)_axleType, V.Car, CurrentWheelData);
			}
		}

		public void SelectWheelDiameter(WheelDiameter select)
		{
			CurrentWheelData.wheelSize = select;
			if (_axleType == WheelTuningAxleType.Both)
			{
				_powertrain.SetWheelParams(AxleType.Front, CurrentWheelData);
				WheelConstructor.SetWheelVisual(AxleType.Front, V.Car, CurrentWheelData);
				CurrentFrontWheelData.wheelSize = select;
				_powertrain.SetWheelParams(AxleType.Rear, CurrentWheelData);
				WheelConstructor.SetWheelVisual(AxleType.Rear, V.Car, CurrentWheelData);
				CurrentRearWheelData.wheelSize = select;
			}
			else
			{
				_powertrain.SetWheelParams((AxleType)_axleType, CurrentWheelData);
				WheelConstructor.SetWheelVisual((AxleType)_axleType, V.Car, CurrentWheelData);
			}
		}

		public override void Select(int select)
		{
			if (LastSelectOperation != null)
			{
				LastSelectOperation = () =>
				{
					Select(select);
				};
			}
			Debug.Log("SelectWheel: " + select);
			CurrentWheelData.RimModelId = select;
			if (_axleType == WheelTuningAxleType.Both)
			{
				CurrentWheelData.BrakesData.CaliperMeshIndex = _carInfo.TuningData.FrontWheelData.BrakesData.CaliperMeshIndex;
				CurrentWheelData.BrakesData.DiscType = _carInfo.TuningData.FrontWheelData.BrakesData.DiscType;
				WheelConstructor.SetWheelModel(_car, AxleType.Front, CurrentWheelData).Forget();
				CurrentFrontWheelData.RimModelId = select;
				CurrentWheelData.BrakesData.CaliperMeshIndex = _carInfo.TuningData.RearWheelData.BrakesData.CaliperMeshIndex;
				CurrentWheelData.BrakesData.DiscType = _carInfo.TuningData.RearWheelData.BrakesData.DiscType;
				WheelConstructor.SetWheelModel(_car, AxleType.Rear, CurrentWheelData).Forget();
				CurrentRearWheelData.RimModelId = select;
			}
			else
			{
				WheelConstructor.SetWheelModel(_car, (AxleType)_axleType, CurrentWheelData).Forget();
			}
			LastSelectOperation?.Invoke();
			LastSelectOperation = null;
		}

		public override void SetDefault()
		{
			FrontWheelData source = _defaultTuningDataConfig.FrontWheelData.Clone() as FrontWheelData;
			RearWheelData source2 = _defaultTuningDataConfig.RearWheelData.Clone() as RearWheelData;
			CopyWheelSize(source, CurrentFrontWheelData);
			CopyWheelSize(source2, CurrentRearWheelData);
			ClampPressureToTyreType(AxleType.Front, CurrentFrontWheelData, CurrentFrontWheelData.tyreType);
			ClampPressureToTyreType(AxleType.Rear, CurrentRearWheelData, CurrentRearWheelData.tyreType);
			_powertrain.SetWheelParams(AxleType.Front, CurrentFrontWheelData);
			WheelConstructor.SetWheelVisual(AxleType.Front, V.Car, CurrentFrontWheelData, updateTyreType: true);
			_powertrain.SetWheelParams(AxleType.Rear, CurrentRearWheelData);
			WheelConstructor.SetWheelVisual(AxleType.Rear, V.Car, CurrentRearWheelData, updateTyreType: true);
			_notificationMessage = new LocalizedString("WheelsTuningScope.Detailreseted!");
			if (CurrentFrontWheelData.RimModelId == _carInfo.TuningData.FrontWheelData.RimModelId)
			{
				ApplyFull();
			}
			else
			{
				_carInfo.Save();
				RevertCurrent();
			}
			InvokeOnValuesChanged();
		}

		private void CopyWheelSize(WheelData source, WheelData target)
		{
			target.wheelSize = source.wheelSize;
			target.rimWidth = source.rimWidth;
			target.tyreProfile = source.tyreProfile;
			target.tyreWidth = source.tyreWidth;
			target.tyreType = source.tyreType;
		}

		private void BuyWheel(int selectedWheelIndex, VehicleData vehicleData, WheelData wheelData, ObscuredInt carId)
		{
			if (!_wheelsDataService.BoughtWheels.Contains(selectedWheelIndex))
			{
				_wheelsDataService.BoughtWheels.Add(selectedWheelIndex);
			}
			_wheelsDataService.Save();
			_notificationMessage = new LocalizedString("BodyKitsVisualScope.Detailinstalled!");
			ApplyFull();
			InvokeOnValuesChanged();
		}

		private void ApplyFull()
		{
			CurrentFrontWheelData.BrakesData = _carInfo.TuningData.FrontWheelData.BrakesData;
			CurrentRearWheelData.BrakesData = _carInfo.TuningData.RearWheelData.BrakesData;
			_carInfo.TuningData.FrontWheelData = CurrentFrontWheelData;
			_carInfo.TuningData.RearWheelData = CurrentRearWheelData;
			_powertrain.SetWheelParams(AxleType.Front, CurrentFrontWheelData);
			_powertrain.SetWheelParams(AxleType.Rear, CurrentRearWheelData);
			_carInfo.Save();
			if (IsStaggeredFitment(CurrentFrontWheelData, CurrentRearWheelData))
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQGw", "wheels_tuning");
			}
			if (CurrentFrontWheelData != null && CurrentRearWheelData != null && (CurrentFrontWheelData.wheelSize != CurrentRearWheelData.wheelSize || CurrentFrontWheelData.rimWidth != CurrentRearWheelData.rimWidth || CurrentFrontWheelData.tyreWidth != CurrentRearWheelData.tyreWidth || CurrentFrontWheelData.tyreProfile != CurrentRearWheelData.tyreProfile || CurrentFrontWheelData.tyreType != CurrentRearWheelData.tyreType))
			{
				_steamAchievementService?.Unlock("ACH_PERFECT_FITMENT");
			}
			_managerUI.ShowNotificationWindow(_notificationMessage, 3f, NotificationType.Success, SoundEventsDataHelper.InstallVisualTuningSwitch).Forget();
			RevertCurrent();
		}

		private static bool IsStaggeredFitment(WheelData front, WheelData rear)
		{
			if (front == null || rear == null)
			{
				return false;
			}
			if (front.wheelSize == rear.wheelSize && front.rimWidth == rear.rimWidth && front.tyreWidth == rear.tyreWidth && front.tyreProfile == rear.tyreProfile)
			{
				return front.tyreType != rear.tyreType;
			}
			return true;
		}

		private void SetCamera()
		{
		}

		private void RevertCurrent()
		{
			Debug.Log("RevertCurrentWheel");
			CurrentFrontWheelData = _carInfo.TuningData.FrontWheelData.Clone() as FrontWheelData;
			CurrentRearWheelData = _carInfo.TuningData.RearWheelData.Clone() as RearWheelData;
		}

		private void ApplyThisWheel(int selectedWheelIndex)
		{
			bool allowZero = false;
			WheelDiskData wheelData = GetWheelData(selectedWheelIndex, _wheelsDiskTuningConfig.AllWheels);
			bool isCoinPrice = wheelData.isCoinPrice;
			if (isCoinPrice)
			{
				wheelData.price = _wheelsDiskTuningConfig.WheelsCoinsPrice;
			}
			int num = wheelData.price;
			if (selectedWheelIndex == _carInfo.TuningData.FrontWheelData.RimModelId || selectedWheelIndex == _carInfo.TuningData.RearWheelData.RimModelId)
			{
				num = 0;
				allowZero = true;
			}
			if (_wheelsDataService.BoughtWheels.Contains(selectedWheelIndex))
			{
				num = 0;
				allowZero = true;
			}
			AdvancedInt price = new AdvancedInt(num);
			if (isCoinPrice)
			{
				_currencyManagerController.TryCoinsSpend(SpendCoinsType.Wheels, price, () =>
				{
					BuyWheel(selectedWheelIndex, _vehicleData, new WheelData(), _carId);
				}, null, allowZero);
			}
			else
			{
				_currencyManagerController.TryMoneySpend(out var _, price, () =>
				{
					BuyWheel(selectedWheelIndex, _vehicleData, new WheelData(), _carId);
				});
			}
		}

		private void ChangeFont(GameObject parentIndex)
		{
			int fontSetIndex = int.Parse(parentIndex.transform.name);
			parentIndex.transform.parent.GetChild(1).gameObject.SetActive(value: true);
			ChangeFontIndex(fontSetIndex);
		}

		private int ChangeFontIndex(int fontSetIndex)
		{
			_lastFontTireSelected = fontSetIndex;
			_dynamicCanvasTire.ChangeFont(fontSetIndex);
			_dynamicCanvasTire.RenderAfter(0.1f);
			ObscuredInt carId = _carId;
			ObscuredPrefs.SetInt("tireFont" + carId.ToString(), _lastFontTireSelected);
			return fontSetIndex;
		}

		private void OnTireTextEnterFinished(string text)
		{
			OnTireTextEntered(text);
			ObscuredInt carId = _carId;
			ObscuredPrefs.SetString("tireText" + carId.ToString(), text);
		}

		private void OnTireTextEntered(string text)
		{
			_dynamicCanvasTire.text.text = text;
			_dynamicCanvasTire.text2.text = text;
			WheelConstructor.DynamicCanvasNormal.CopyText(_dynamicCanvasTire.text, 1f - _dynamicCanvasTire.degradationLayer.color.a);
			_dynamicCanvasTire.RenderAfter(0.1f);
		}
	}
}
