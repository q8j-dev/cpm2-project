using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using RacingScripts.RaceControllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Racing_Scripts;

public class CircuitPitStopWindow : MonoBehaviour
{
	[SerializeField]
	private GameObject SelectMainternancePanel;

	[SerializeField]
	private GameObject LoadingMaintenancePanel;

	[SerializeField]
	private Image ImageMaintenanceLoadingCircle;

	[SerializeField]
	private Image MaintenanceLoadingImage;

	[SerializeField]
	private Button[] MaintenanceButtons;

	[SerializeField]
	private TextMeshProUGUI[] PriceTexts;

	[SerializeField]
	private TextMeshProUGUI[] MaintenanceTexts;

	[SerializeField]
	private CarTuningUISlider[] CarTuningUISliders;

	[SerializeField]
	private Sprite[] MaintenanceSprites;

	[SerializeField]
	private GameObject UIForRide;

	[SerializeField]
	private float WaitMaintenanceTime;

	[SerializeField]
	private Color[] MaintenanceColors;

	[SerializeField]
	private CircuitPlayWindow CircuitPlayWindow;

	private ManagerUI _managerUI;

	private CircuitRacingUIControl _circuitRacingUIControl;

	private CircuitRaceControlller _circuitRaceControlller;

	private VehicleEntityProvider _vehicleEntityProvider;

	private Powertrain _powertrain;

	private Axles _axles;

	private float _fuelTankCapacity;

	private RaceModeController _raceModeController;

	[Inject]
	public void Construct(RaceModeController raceModeController, ManagerUI managerUI)
	{
		_managerUI = managerUI;
		_raceModeController = raceModeController;
	}

	private void Start()
	{
		InitRefsIfNeed();
		MaintenanceButtons[0].onClick.AddListener(ShowFuelUI);
		MaintenanceButtons[1].onClick.AddListener(() =>
		{
			LoadingMaintenance(MaintenanceType.AllFix);
		});
		MaintenanceButtons[2].onClick.AddListener(() =>
		{
			LoadingMaintenance(MaintenanceType.Wheel);
		});
	}

	private void OnEnable()
	{
		SelectMainternancePanel.SetActive(value: true);
		Init();
	}

	public void Init()
	{
		InitRefsIfNeed();
		LapRaceCar userLapRaceCar = _circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar();
		if (!_vehicleEntityProvider)
		{
			_vehicleEntityProvider = userLapRaceCar.GetComponent<VehicleEntityProvider>();
		}
		if (!_axles)
		{
			_axles = userLapRaceCar.GetComponent<Axles>();
		}
		if ((bool)_vehicleEntityProvider)
		{
			_fuelTankCapacity = _vehicleEntityProvider.CarEntity.ChassisData.FuelTankCapacity;
			int num = Mathf.RoundToInt(_vehicleEntityProvider.CarEntity.MaintenanceInfo.FuelInTank);
			float num2 = _fuelTankCapacity / 100f;
			int num3 = (int)((float)num / num2);
			ParseMaintenancePanel(0, $"{num3} %", num3);
			CheckWheels(out var _);
		}
	}

	private void SetMinMaxValues(Slider slider, float minVal, float maxVal)
	{
		slider.minValue = minVal;
		slider.maxValue = maxVal;
	}

	private void CheckWheels(out int wheelsPercent)
	{
		_axles.GetTyreHealth(_axles.FrontAxle, out var left, out var right);
		_axles.GetTyreHealth(_axles.RearAxle, out var left2, out var right2);
		int num = (wheelsPercent = (int)((left + right + left2 + right2) / 4f * 100f));
		ParseMaintenancePanel(2, $"{num} %", num);
	}

	private void LoadingMaintenance(MaintenanceType maintenanceId)
	{
		SelectMainternancePanel.SetActive(value: false);
		LoadingMaintenancePanel.SetActive(value: true);
		CircuitPlayWindow.EnableHideLoadingObjects(state: false);
		switch (maintenanceId)
		{
		case MaintenanceType.Fuel:
			_circuitRaceControlller.RaceCrewTalkSoundController.PlayFuelRefillSound();
			_vehicleEntityProvider.Refuel(_fuelTankCapacity);
			break;
		case MaintenanceType.Wheel:
			_circuitRaceControlller.RaceCrewTalkSoundController.PlayWheelsChangeSound();
			_axles.SetTyreHealth(_axles.FrontAxle, 1f, _axles.FrontAxle.LeftWheel.TyreType);
			_vehicleEntityProvider.CarEntity.SetNewFrontTyres();
			_axles.SetTyreHealth(_axles.RearAxle, 1f, _axles.RearAxle.LeftWheel.TyreType);
			_vehicleEntityProvider.CarEntity.SetNewFrontTyres();
			CircuitPlayWindow.HideWheelWarning();
			break;
		}
		MaintenanceLoadingImage.sprite = MaintenanceSprites[(int)maintenanceId];
		StartProgress().Forget();
	}

	private void ShowFuelUI()
	{
		_circuitRaceControlller.RaceCrewTalkSoundController.PlayFuelPutSound();
		_circuitRacingUIControl.CircuitPlayWindow.ShowUI(state: false);
		_ = _managerUI.ShowWindow(WindowScriptType.FuelDriveUIWindow.ToString(), alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false);
	}

	private async UniTaskVoid StartProgress()
	{
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		for (int i = 0; i < 100; i++)
		{
			await UniTask.Delay(10, ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			ImageMaintenanceLoadingCircle.fillAmount = (float)i * 0.1f;
		}
		SelectMainternancePanel.SetActive(value: true);
		LoadingMaintenancePanel.SetActive(value: false);
		ImageMaintenanceLoadingCircle.fillAmount = 0f;
		Init();
	}

	public void ShowPanel(bool state)
	{
		SelectMainternancePanel.SetActive(state);
	}

	private void ParseMaintenancePanel(int index, string text, float value)
	{
		Color color = Color.green;
		if (value > 40f && value < 80f)
		{
			color = Color.yellow;
		}
		if (value <= 40f)
		{
			color = Color.red;
		}
		CarTuningUISliders[index].Init(color, text, value);
	}

	private void InitRefsIfNeed()
	{
		if (!_circuitRaceControlller)
		{
			_circuitRaceControlller = _raceModeController.SpawnedObject.GetComponent<CircuitRaceControlller>();
			_circuitRacingUIControl = _raceModeController.SpawnedObject.GetComponent<CircuitRacingUIControl>();
			LapRaceCar userLapRaceCar = _circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar();
			_vehicleEntityProvider = userLapRaceCar.GetComponent<VehicleEntityProvider>();
			_axles = userLapRaceCar.GetComponent<Axles>();
			_powertrain = userLapRaceCar.GetComponent<Powertrain>();
			_fuelTankCapacity = _vehicleEntityProvider.CarEntity.ChassisData.FuelTankCapacity;
		}
	}
}
