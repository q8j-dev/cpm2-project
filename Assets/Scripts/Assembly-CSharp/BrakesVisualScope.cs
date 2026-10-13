using System.Linq;
using CPM.Database;
using CPM.Database.Constructors;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Racing.CameraManagement;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.VisualTuning;
using _Car_Parking.Scripts.Wallet;

public class BrakesVisualScope : VisualTuningScope
{
	private GameObject _car;

	private WheelTuningAxleType _axleType;

	private BrakesData _currentFrontBrakeData;

	private BrakesData _currentRearBrakeData;

	private CarInfo _carInfo;

	private VehicleData _vehicleData;

	private int _carId;

	private ICarInfoService _carInfoService;

	private IWheelsDataService _wheelsDataService;

	private BrakeTuningConfig _brakeTuningConfig;

	private CurrencyManagerController _currencyManagerController;

	private CameraManager _cameraManager;

	private MenuControl _menuControl;

	private ManagerUI _managerUI;

	private BrakesData CurrentBrakesData
	{
		get
		{
			if (_axleType == WheelTuningAxleType.Front || _axleType == WheelTuningAxleType.Both)
			{
				return _currentFrontBrakeData;
			}
			return _currentRearBrakeData;
		}
	}

	[Inject]
	public void Construct(ICarInfoService carInfoService, IWheelsDataService wheelsDataService, BrakeTuningConfig brakeTuningConfig, CurrencyManagerController currencyManagerController, CameraManager cameraManager, MenuControl menuControl, ManagerUI managerUI)
	{
		_managerUI = managerUI;
		_menuControl = menuControl;
		_cameraManager = cameraManager;
		_carInfoService = carInfoService;
		_wheelsDataService = wheelsDataService;
		_brakeTuningConfig = brakeTuningConfig;
		_currencyManagerController = currencyManagerController;
	}

	public override void Init()
	{
		_car = V.Car;
		_carId = V.CarId;
		_vehicleData = V.Car.GetComponent<VehicleData>();
		_carInfo = _carInfoService.GetFromSlot(V.SlotID);
		RevertCurrent();
		HideWheels(_axleType);
		SetCamera();
	}

	private void RevertCurrent()
	{
		Debug.Log("RevertCurrentWheel");
		_currentFrontBrakeData = _carInfo.TuningData.FrontWheelData.BrakesData.Clone() as BrakesData;
		_currentRearBrakeData = _carInfo.TuningData.RearWheelData.BrakesData.Clone() as BrakesData;
		SelectDiscTexture((int)CurrentBrakesData.DiscType, isReset: true);
		SelectBrakeSize(CurrentBrakesData.Diameter, isReset: true);
	}

	public override void Apply()
	{
		Debug.Log("Try apply wheel.");
		BrakeDiscType brakeDiscType = ((_axleType == WheelTuningAxleType.Front) ? _carInfo.TuningData.FrontWheelData.BrakesData.DiscType : _carInfo.TuningData.RearWheelData.BrakesData.DiscType);
		if (CurrentBrakesData.DiscType != brakeDiscType)
		{
			ApplyThisBrakeTexture(CurrentBrakesData.GetBrakeDiskTypeIndex());
		}
		else
		{
			ApplyFull();
		}
	}

	private void ApplyThisBrakeTexture(int selectedBrakeIndex)
	{
		BrakeDiskItemData brakeDiskItemData = _brakeTuningConfig.AllBrakes[selectedBrakeIndex];
		if (_wheelsDataService.IsBrakeStock(brakeDiskItemData.BrakeDiscType, _carId))
		{
			ApplyFull();
			InvokeOnValuesChanged();
			return;
		}
		bool alreadyOwned = _wheelsDataService.IsBrakeBought(brakeDiskItemData.index);
		VisualItemPurchase.Purchase(_currencyManagerController, SpendCoinsType.Brakes, brakeDiskItemData.index, brakeDiskItemData.price, alreadyOwned, () =>
		{
			BuyBrake(brakeDiskItemData.index);
		});
	}

	private void SetCamera()
	{
	}

	public override void Revert()
	{
		RevertCurrent();
	}

	private void BuyBrake(int selectedWheelIndex)
	{
		_wheelsDataService.BuyBrake(selectedWheelIndex);
		_wheelsDataService.Save();
		ApplyFull();
		InvokeOnValuesChanged();
	}

	private void ApplyFull()
	{
		_managerUI.ShowNotificationWindow(new LocalizedString("BrakeDiskItemData.Detailinstalled!"), 3f, NotificationType.Success, SoundEventsDataHelper.InstallVisualTuningSwitch).Forget();
		_carInfo.TuningData.FrontWheelData.BrakesData = _currentFrontBrakeData;
		_carInfo.TuningData.RearWheelData.BrakesData = _currentRearBrakeData;
		_carInfo.Save();
		RevertCurrent();
	}

	public override void Select(int index)
	{
	}

	public void SelectDiscTexture(int brakeDiscTypeIndex, bool isReset = false)
	{
		CurrentBrakesData.DiscType = (BrakeDiscType)brakeDiscTypeIndex;
		if (isReset)
		{
			WheelConstructor.SetBrakesTexture(AxleType.Front, _car, _currentFrontBrakeData);
			WheelConstructor.SetBrakesTexture(AxleType.Rear, _car, _currentRearBrakeData);
		}
		else if (_axleType == WheelTuningAxleType.Both)
		{
			_currentFrontBrakeData.DiscType = CurrentBrakesData.DiscType;
			WheelConstructor.SetBrakesTexture(AxleType.Front, _car, _currentFrontBrakeData);
			_currentRearBrakeData.DiscType = CurrentBrakesData.DiscType;
			WheelConstructor.SetBrakesTexture(AxleType.Rear, _car, _currentRearBrakeData);
		}
		else
		{
			WheelConstructor.SetBrakesTexture((AxleType)_axleType, _car, CurrentBrakesData);
		}
	}

	public void SetAxleType(WheelTuningAxleType axleType)
	{
		_axleType = axleType;
		Observable.TimerFrame(1).Subscribe((long _) =>
		{
			Revert();
			SetCamera();
		}).AddTo(_car);
		HideWheels(_axleType);
	}

	public void SelectBrakeSize(float value, bool isReset = false)
	{
		if (isReset)
		{
			WheelConstructor.ChangeBrakeSize(AxleType.Front, _car, _currentFrontBrakeData.Diameter, _currentFrontBrakeData);
			WheelConstructor.ChangeBrakeSize(AxleType.Rear, _car, _currentRearBrakeData.Diameter, _currentRearBrakeData);
		}
		else if (_axleType == WheelTuningAxleType.Both)
		{
			_currentFrontBrakeData.Diameter = value;
			WheelConstructor.ChangeBrakeSize(AxleType.Front, _car, value, _currentFrontBrakeData);
			_currentRearBrakeData.Diameter = value;
			WheelConstructor.ChangeBrakeSize(AxleType.Rear, _car, value, _currentRearBrakeData);
		}
		else
		{
			WheelConstructor.ChangeBrakeSize((AxleType)_axleType, _car, value, CurrentBrakesData);
		}
	}

	private void HideWheels(WheelTuningAxleType axleType)
	{
		if (!_car)
		{
			return;
		}
		FlagsStorage.IsCarInService = true;
		if (!_car.TryGetComponent<Axles>(out var component))
		{
			return;
		}
		Axle axle;
		switch (axleType)
		{
		case WheelTuningAxleType.Both:
		{
			foreach (WheelProvider allWheel in component.AllWheels)
			{
				if ((bool)allWheel.WheelModel)
				{
					allWheel.WheelModel.HideWheel(set: true);
				}
			}
			return;
		}
		default:
			axle = component.RearAxle;
			break;
		case WheelTuningAxleType.Front:
			axle = component.FrontAxle;
			break;
		}
		ShowWheels();
		foreach (WheelProvider wheel in axle.Wheels)
		{
			if ((bool)wheel.WheelModel)
			{
				wheel.WheelModel.HideWheel(set: true);
			}
		}
		if (axleType != WheelTuningAxleType.Rear || !component.DualAxles)
		{
			return;
		}
		foreach (WheelProvider wheel2 in component.Rear1Axle.Wheels)
		{
			wheel2.WheelModel.HideWheel(set: true);
		}
		foreach (WheelProvider wheel3 in component.Rear2Axle.Wheels)
		{
			wheel3.WheelModel.HideWheel(set: true);
		}
	}

	private void ShowWheels()
	{
		if (!_car || !_car.TryGetComponent<Axles>(out var component))
		{
			return;
		}
		foreach (WheelProvider item in component.AllWheels.Where((WheelProvider wheel) => wheel.WheelModel))
		{
			item.WheelModel.HideWheel(set: false);
		}
	}

	public override void OnChangeScope()
	{
		ShowWheels();
		RevertCurrent();
	}
}
