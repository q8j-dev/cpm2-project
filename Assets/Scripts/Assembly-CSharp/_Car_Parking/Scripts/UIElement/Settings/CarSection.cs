using CPMEngine;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class CarSection : SettingsSectionBase
	{
		[SerializeField]
		private Toggle ParkingSensorOnToggle;

		[SerializeField]
		private Toggle ParkingSensorOffToggle;

		[SerializeField]
		private SliderValueHandler TcsSlider;

		[SerializeField]
		private SliderValueHandler AbsSlider;

		[SerializeField]
		private SliderValueHandler EspSlider;

		[SerializeField]
		private Toggle StabilizationToggle;

		[SerializeField]
		private CanvasGroup StabilizationCanvasGroup;

		[SerializeField]
		private Toggle DriftModeToggle;

		[SerializeField]
		private CanvasGroup DriftModeCanvasGroup;

		[SerializeField]
		private Toggle ArcadeModeToggle;

		[SerializeField]
		private CanvasGroup ArcaderCanvasGroup;

		[SerializeField]
		private Toggle ManualGearboxAssistToggle;

		[SerializeField]
		private CanvasGroup ManualGearboxAssistCanvasGroup;

		private bool _isOpened;

		private Powertrain _powertrain;

		private ParkTronicControll _parkTronicControl;

		private OptionalDependency<ParkTronicControll> _parkTronicControlDep;

		private CarInputManager _carInputManager;

		[Inject]
		public void Construct(CarInputManager carInputManager, OptionalDependency<ParkTronicControll> parkTronicControl)
		{
			_parkTronicControlDep = parkTronicControl;
			_carInputManager = carInputManager;
		}

		protected override void Awake()
		{
			base.Awake();
			_parkTronicControl = _parkTronicControlDep?.Value;
		}

		private void Start()
		{
			ParkingSensorOnToggle.onValueChanged.AddListener(ShowParktronic);
			if ((bool)TcsSlider)
			{
				TcsSlider.OnReleased += SetTcs;
			}
			if ((bool)AbsSlider)
			{
				AbsSlider.OnReleased += SetAbs;
			}
			if ((bool)EspSlider)
			{
				EspSlider.OnReleased += SetEsp;
			}
			StabilizationToggle.onValueChanged.AddListener(SetStabilization);
			DriftModeToggle.onValueChanged.AddListener(SetDriftMode);
			ManualGearboxAssistToggle.onValueChanged.AddListener(SetManualAssist);
			ArcadeModeToggle.SetActive(active: false);
			Restore();
		}

		private void OnEnable()
		{
			if (_isOpened)
			{
				Restore();
			}
		}

		private void ShowParktronic(bool isOn)
		{
			base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.parktronic_change, (!isOn).ToString(), isOn.ToString());
			PlayerPrefs.SetInt("parkingSensor", isOn ? 1 : 0);
			FlagsStorage.ShowParkingSensors = isOn;
			if ((bool)V.Car)
			{
				VehicleData component = V.Car.GetComponent<VehicleData>();
				if ((bool)component && component.ParkTronikComposition != null)
				{
					component.ParkTronikComposition.TrySetActiveParktronikParent(isOn);
				}
			}
			if (!isOn && (bool)_parkTronicControl)
			{
				_parkTronicControl.HideVisual();
			}
		}

		private void SetTcs(float value)
		{
			int level = (int)value;
			base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.tsc, PlayerPrefs.GetInt("TCS", 0).ToString(), level.ToString());
			if ((bool)_carInputManager)
			{
				_carInputManager.SetTrcLevel(level);
			}
		}

		private void SetAbs(float value)
		{
			int absLevel = (int)value;
			base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.abs, PlayerPrefs.GetInt("ABS", 5).ToString(), absLevel.ToString());
			if ((bool)_carInputManager)
			{
				_carInputManager.SetAbsLevel(absLevel);
			}
		}

		private void SetEsp(float value)
		{
			int level = (int)value;
			base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.esp, PlayerPrefs.GetInt("ESP", 0).ToString(), level.ToString());
			if ((bool)_carInputManager)
			{
				_carInputManager.SetEspLevel(level);
			}
		}

		private void SetStabilization(bool isOn)
		{
			base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.steering_assistant, (!isOn).ToString(), isOn.ToString());
			if ((bool)_carInputManager)
			{
				_carInputManager.SteerAssistance = isOn;
			}
			PlayerPrefs.SetInt("Stabilization", isOn ? 1 : 0);
		}

		private void SetDriftMode(bool isOn)
		{
			PlayerPrefs.SetInt("DriftMode", isOn ? 1 : 0);
			if ((bool)_carInputManager)
			{
				_carInputManager.DriftMode = isOn;
			}
		}

		private void SetArcade(bool isOn)
		{
			base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.arcade_mode, (!isOn).ToString(), isOn.ToString());
			if ((bool)_carInputManager)
			{
				_carInputManager.SetArcadeMode(isOn);
			}
		}

		private void SetManualAssist(bool isOn)
		{
			base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.manual_gearbox_assistant, (!isOn).ToString(), isOn.ToString());
			if ((bool)_carInputManager)
			{
				_carInputManager.SetMTAssist(isOn);
			}
		}

		public override void Restore()
		{
			bool flag = PlayerPrefs.GetInt("parkingSensor", 1) == 1;
			ParkingSensorOnToggle.isOn = flag;
			ParkingSensorOffToggle.isOn = !flag;
			EspSlider.SetValue(PlayerPrefs.GetInt("ESP", 0));
			TcsSlider.SetValue(PlayerPrefs.GetInt("TCS", 0));
			AbsSlider.SetValue(PlayerPrefs.GetInt("ABS", 5));
			ArcaderCanvasGroup.interactable = ArcaderCanvasGroup.alpha >= 1f;
			if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop)
			{
				if (_carInputManager.DesktopControllerType == DesktopControllerType.Keyboard)
				{
					StabilizationCanvasGroup.alpha = 1f;
					DriftModeCanvasGroup.alpha = 1f;
					StabilizationCanvasGroup.alpha = 1f;
					StabilizationToggle.isOn = _carInputManager.DesktopControllerType == DesktopControllerType.Keyboard && PlayerPrefs.GetInt("Stabilization", 1) == 1;
					DriftModeToggle.isOn = _carInputManager.DesktopControllerType == DesktopControllerType.Keyboard && PlayerPrefs.GetInt("DriftMode", 1) == 1;
				}
				else
				{
					DriftModeCanvasGroup.alpha = 0.2f;
					StabilizationCanvasGroup.alpha = 0.2f;
					StabilizationToggle.SetIsOnWithoutNotify(value: false);
					DriftModeToggle.SetIsOnWithoutNotify(value: false);
				}
			}
			else
			{
				int steering = V.Steering;
				bool flag2 = CarInputManager.SupportsSteerAssist((MobileControllerType)steering);
				StabilizationCanvasGroup.alpha = (flag2 ? 1f : 0.2f);
				if (flag2)
				{
					StabilizationToggle.isOn = PlayerPrefs.GetInt("Stabilization", 0) == 1;
				}
				else
				{
					StabilizationToggle.SetIsOnWithoutNotify(value: false);
				}
				bool flag3 = steering == 3;
				DriftModeCanvasGroup.alpha = (flag3 ? 1f : 0.2f);
				if (flag3)
				{
					DriftModeToggle.isOn = PlayerPrefs.GetInt("DriftMode", 0) == 1;
				}
				else
				{
					DriftModeToggle.SetIsOnWithoutNotify(value: false);
				}
			}
			DriftModeCanvasGroup.interactable = DriftModeCanvasGroup.alpha >= 1f;
			StabilizationCanvasGroup.interactable = StabilizationCanvasGroup.alpha >= 1f;
			if ((bool)V.Car)
			{
				if (V.Car.TryGetComponent<Powertrain>(out _powertrain) && !_powertrain.IsAutomaticGearbox)
				{
					if (FlagsStorage.RacingGameMode)
					{
						ManualGearboxAssistToggle.SetIsOnWithoutNotify(value: false);
						ManualGearboxAssistCanvasGroup.alpha = 0.2f;
					}
					else
					{
						ManualGearboxAssistToggle.isOn = PlayerPrefs.GetInt("ManualGearboxAssist", 0) == 1;
						ManualGearboxAssistCanvasGroup.alpha = 1f;
					}
				}
				else
				{
					ManualGearboxAssistToggle.SetIsOnWithoutNotify(value: false);
					ManualGearboxAssistCanvasGroup.alpha = 0.2f;
				}
			}
			else
			{
				ManualGearboxAssistToggle.SetIsOnWithoutNotify(value: false);
				ManualGearboxAssistCanvasGroup.alpha = 0.2f;
			}
			ManualGearboxAssistCanvasGroup.interactable = ManualGearboxAssistCanvasGroup.alpha >= 1f;
			_isOpened = true;
		}
	}
}
