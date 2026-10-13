using CPM.Database;
using CPMEngine;
using DG.Tweening;
using Racing.CameraManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

public class DynoWindow : WindowScript, IBackableWindow
{
	public TextMeshProUGUI maxTorqueLabel;

	public TextMeshProUGUI maxPowerLabel;

	public TextMeshProUGUI pullCountLabel;

	public TextMeshProUGUI ClassText;

	public Button RideCarButton;

	public Button CameraButton;

	private DynoWindowPanels _activeDynoWindowPanel;

	private CameraManager _cameraManager;

	private Tween _tween;

	[SerializeField]
	private AFController AFController;

	[SerializeField]
	private RPMController RPMController;

	[SerializeField]
	private BoostControllerGadget BoostController;

	[SerializeField]
	private GameObject WarningPanel;

	[SerializeField]
	private TextMeshProUGUI WarningText;

	[SerializeField]
	private CanvasGroup[] PanelsCanvasArray;

	private ITimerTuningService _timerTuningService;

	private Powertrain _powertrain;

	private float _maxTq;

	private bool _carTransmission;

	private float _oldInertia;

	private int _recomendedGear;

	private ThrottleSliderControl _throttleSliderControl;

	private bool _manualHelper;

	private bool _isAutomaticGearbox;

	private SoundControllerExtension _carInOutSound;

	private EventRegistry _eventRegistry;

	private IDatabaseCarsService _databaseCarsService;

	private RegistryHandle<BoolEvent> _onThrottleHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnThrottle);

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.DynoRun;

	[Inject]
	public void Construct(CameraManager cameraManager, ManagerUI managerUI, ITimerTuningService timerTuningService, SoundControllerExtension carInOutSound, EventRegistry eventRegistry, IDatabaseCarsService databaseCarsService)
	{
		_carInOutSound = carInOutSound;
		_eventRegistry = eventRegistry;
		_databaseCarsService = databaseCarsService;
		_timerTuningService = timerTuningService;
		_cameraManager = cameraManager;
		_managerUI = managerUI;
		_cameraManager.ActivateCamera(RacingCameraType.DynoCar, V.Car ? V.Car.transform : null, _cameraManager.CutBlend);
	}

	protected override void Start()
	{
		base.Start();
		V.CoreGameManager.menuControl.DynoRunController.dynoWindow = this;
		base.gameObject.TryGetComponentInChildren<ThrottleSliderControl>(out _throttleSliderControl);
		_carInOutSound.SetOutCarSound();
		InitBackButton();
		_cameraManager.garageCam.ResetCamForDyno();
		CameraButton.onClick.AddListener(CameraButtonToggle);
		RideCarButton.onClick.AddListener(RideCar);
		BoostController.Check();
		AFController.Init();
		if (V.LastUsedCar.TryGetComponent<Powertrain>(out _powertrain))
		{
			_powertrain.CalcMaxPowerMaxTorque(out var _, out var _, out _maxTq, out var _, out var _);
			_recomendedGear = FindDynoGear(_powertrain.GetGearRatios());
			_manualHelper = _powertrain.ManualHelper;
			_carTransmission = _powertrain.Automatic;
			_isAutomaticGearbox = _powertrain.IsAutomaticGearbox;
			_oldInertia = _powertrain.EngineInertiaRatio;
			if (!_powertrain.IsEngineWorks)
			{
				_powertrain.StartEngine(starterSound: true);
			}
			_powertrain.IsAutomaticGearbox = true;
			_powertrain.ResetTransmission();
			_powertrain.ManualHelper = false;
		}
		_throttleSliderControl.gameObject.SetActive(value: false);
		_onThrottleHandle.Get(_eventRegistry).Subscribe(HideOrShowPanels);
	}

	protected override void OnDestroy()
	{
		_onThrottleHandle.Get(_eventRegistry)?.Unsubscribe(HideOrShowPanels);
		base.OnDestroy();
	}

	private int FindDynoGear(float[] arr)
	{
		if (arr == null || arr.Length == 0)
		{
			return -1;
		}
		int result = 0;
		float num = Mathf.Abs(arr[0] - 1f);
		for (int i = 1; i < arr.Length; i++)
		{
			float num2 = Mathf.Abs(arr[i] - 1f);
			if (num2 < num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}

	public void HideOrShowPanels(bool state)
	{
		DelayedHideOrShowPanels(state);
		if (state)
		{
			_powertrain.Automatic = false;
			_powertrain.Gear = _recomendedGear;
		}
		else
		{
			_powertrain.Automatic = true;
		}
	}

	public void DelayedHideOrShowPanels(bool state)
	{
		if (_cameraManager.CurrentCameraType != RacingCameraType.DynoGraph)
		{
			state = false;
		}
		_tween?.Kill();
		for (int i = 0; i < PanelsCanvasArray.Length; i++)
		{
			_tween = PanelsCanvasArray[i].DOFade((!state) ? 1 : 0, 0.5f);
		}
	}

	private void ResetPanels()
	{
		BoostController.SetActive(active: false);
		AFController.SetActive(active: false);
		RPMController.SetActive(active: false);
	}

	public void OnChangeWindow(DynoWindowPanels dynoWindowPanels)
	{
		ShowWarningPanel(state: false);
		_activeDynoWindowPanel = dynoWindowPanels;
		ResetPanels();
		switch (dynoWindowPanels)
		{
		case DynoWindowPanels.AFR:
			AFController.Init();
			break;
		case DynoWindowPanels.RPM:
			WarningText.text = new LocalizedString("Dynowindow.rpmwarningtext");
			RPMController.Init();
			break;
		case DynoWindowPanels.BOOST:
			WarningText.text = new LocalizedString("Dynowindow.boostwarningtext");
			BoostController.Init();
			break;
		case DynoWindowPanels.LAUNCH:
			break;
		}
	}

	public void ShowWarningPanel(bool state)
	{
		WarningPanel.SetActive(state);
	}

	private void CameraButtonToggle()
	{
		_cameraManager.ActivateCamera((_cameraManager.CurrentCameraType != RacingCameraType.GarageLookAround) ? RacingCameraType.GarageLookAround : RacingCameraType.DynoGraph);
	}

	private void RideCar()
	{
		if (_timerTuningService.IsCarBlocked(V.SlotID))
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("DynoWindow.Yourcarcurrentlyisinservice"), 3f);
			return;
		}
		_managerUI.ShowMenuGarageControllerHUD();
		V.CoreGameManager.menuControl.DynoRunController.OnDynoClosed(getToMenu: false);
		_cameraManager.garageCam.ResetCamToDefault();
		RestoreCarSettings();
		_managerUI.OnActivateBalancePanel?.Invoke(obj: false);
		Close();
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, V.Car.transform);
		_managerUI.ShowCarTuningWindow();
		V.CoreGameManager.menuControl.DynoRunController.OnDynoClosed();
		_cameraManager.garageCam.ResetCamToDefault();
		RestoreCarSettings();
		Close();
	}

	private void RestoreCarSettings()
	{
		_powertrain.Automatic = _carTransmission;
		_powertrain.IsAutomaticGearbox = _isAutomaticGearbox;
		_powertrain.ManualHelper = _manualHelper;
		_powertrain.ResetTransmission();
		CarInfoDeferredSaver.FlushDebounced();
		_databaseCarsService.SaveCurrentCar(saveNow: true);
	}
}
