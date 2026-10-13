using System;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;

public class FuelTankEntity : MonoBehaviour
{
	private Powertrain _powertrain;

	public bool FullingFuel;

	private float _tankCapacity;

	private float _tankWeight = 10f;

	private bool _showLessFuel;

	private bool _showNoFuel;

	public GasPistolMove lastGasPistol;

	private float _beforeLiter;

	private float _afterLiter;

	private bool _messageShowed;

	private ManagerUI _managerUI;

	private CurrencyManagerController _currencyManagerController;

	private CoreGameManager _coreGameManager;

	private FreeDriveProvider _freeDriveProvider;

	private LevelsProvider _levelsProvider;

	public Powertrain Powertrain
	{
		set
		{
			_powertrain = value;
		}
	}

	public float TankWeight
	{
		get
		{
			return _tankWeight;
		}
		set
		{
			_tankWeight = ((value <= 0f) ? 0.001f : value);
		}
	}

	public float TankCapacity
	{
		get
		{
			return _tankCapacity;
		}
		set
		{
			_tankCapacity = Mathf.Clamp(value, 0f, 250f);
			if ((bool)_powertrain)
			{
				_powertrain.CurrentFuel = Mathf.Clamp(_powertrain.CurrentFuel, 0f, _tankCapacity);
			}
		}
	}

	public float CurrentFuel
	{
		get
		{
			if (!_powertrain)
			{
				return 0f;
			}
			return Mathf.Clamp(_powertrain.CurrentFuel, 0f, _tankCapacity);
		}
		set
		{
			if ((bool)_powertrain)
			{
				_powertrain.CurrentFuel = value;
				_showLessFuel = value < _tankCapacity * 0.15f;
				if (!_showLessFuel)
				{
					_messageShowed = false;
				}
				_showNoFuel = value < 0.1f;
			}
		}
	}

	[Inject]
	public void Construct(ManagerUI managerUI, CurrencyManagerController currencyManagerController, CoreGameManager coreGameManager, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider)
	{
		_levelsProvider = levelsProvider;
		_freeDriveProvider = freeDriveProvider;
		_coreGameManager = coreGameManager;
		_currencyManagerController = currencyManagerController;
		_managerUI = managerUI;
	}

	public void Init(Powertrain powertrain)
	{
		_powertrain = powertrain;
		base.gameObject.layer = _powertrain.transform.gameObject.layer;
	}

	public void Setup(float tankCapacity)
	{
		TankCapacity = tankCapacity;
	}

	public void SetupForRace(float fuel, float capacity)
	{
		TankCapacity = capacity;
		CurrentFuel = fuel;
	}

	private void FullFuel()
	{
		if (!FlagsStorage.IsPassenger)
		{
			FullingFuel = false;
			_afterLiter = CurrentFuel;
			int num = (int)Mathf.Abs(_beforeLiter - _afterLiter);
			int num2 = num * 2;
			if ((bool)lastGasPistol)
			{
				lastGasPistol.StopFillFuel(full: true);
			}
			int num3 = ObscuredPrefs.GetInt("allFuelConsumed");
			if ((bool)_freeDriveProvider.FreeDriveDB)
			{
				_freeDriveProvider.FreeDriveDB.fuelConsumed = num;
			}
			ObscuredPrefs.SetInt("allFuelConsumed", num3 + num);
			V.CoreGameManager.SetMessage(string.Concat(new LocalizedString("FuelTankEntity.Tankisfull"), $"\n-{num2}"), 2f);
			AdvancedInt price = new AdvancedInt(num2);
			_currencyManagerController.TryMoneySpend(out var _, price);
		}
	}

	public void StartFullingFuel()
	{
		_levelsProvider.LevelScript.CleanScreen();
		FlagsStorage.IsFreeFuelWindow = false;
		_managerUI.ShowFuelDriveUIWindow();
	}

	public void ReleaseGasPistol()
	{
		if ((bool)lastGasPistol)
		{
			lastGasPistol.StopFillFuel(full: true);
			lastGasPistol = null;
		}
	}

	public void StopFullingFuel()
	{
		if (!FlagsStorage.IsPassenger && FullingFuel)
		{
			FullingFuel = false;
			_afterLiter = CurrentFuel;
			int num = (int)Mathf.Abs(_beforeLiter - _afterLiter);
			int num2 = num * 2;
			int num3 = ObscuredPrefs.GetInt("allFuelConsumed");
			ObscuredPrefs.SetInt("allFuelConsumed", num3 + num);
			if ((bool)_freeDriveProvider.FreeDriveDB)
			{
				_freeDriveProvider.FreeDriveDB.fuelConsumed = num;
			}
			_showLessFuel = false;
			_showNoFuel = false;
			V.CoreGameManager.SetMessage($"-{num2}", 2f);
			AdvancedInt price = new AdvancedInt(num2);
			_currencyManagerController.TryMoneySpend(out var _, price);
		}
	}

	private void Update()
	{
		if (!_powertrain || _powertrain.OnLineCar || _tankCapacity <= 0f)
		{
			return;
		}
		if (FullingFuel)
		{
			CurrentFuel += 2f * Time.deltaTime;
			if (CurrentFuel >= _tankCapacity)
			{
				FullFuel();
			}
		}
		else if (FlagsStorage.IsFreeDriveMode)
		{
			CheckForMessage();
		}
	}

	private void CheckForMessage()
	{
		if (_messageShowed || V.Car != _powertrain.gameObject || FlagsStorage.IsPassenger)
		{
			return;
		}
		if (_showLessFuel)
		{
			V.CoreGameManager.SetMessage(new LocalizedString("FuelTankEntity.RefuelCar"), 2f);
			_messageShowed = true;
		}
		if (!_showNoFuel)
		{
			return;
		}
		V.CoreGameManager.SetMessage(new LocalizedString("FuelTankEntity.NoFuel"), 2f);
		_messageShowed = true;
		if (PlayerPrefs.GetInt("ShowGetFuelMenuOneTime", 0) == 0 && V.MenuIndex != 11)
		{
			PlayerPrefs.SetInt("ShowGetFuelMenuOneTime", 1);
			Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				_freeDriveProvider.FreeDriveControl.ShowGetFuelMenu();
			}).AddTo(this);
		}
	}

	public void FullFuelImmediately()
	{
		int num = (int)Mathf.Abs(TankCapacity - CurrentFuel);
		CurrentFuel = TankCapacity;
		int num2 = ObscuredPrefs.GetInt("allFuelConsumed");
		ObscuredPrefs.SetInt("allFuelConsumed", num2 + num);
		_freeDriveProvider.FreeDriveDB.fuelConsumed = num;
		if (!_powertrain.IsEngineWorks)
		{
			_powertrain.StartEngine(starterSound: true);
		}
	}
}
