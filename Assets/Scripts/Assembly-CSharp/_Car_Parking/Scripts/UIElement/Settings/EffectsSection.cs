using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.ScenesLogic.WorldTime;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class EffectsSection : SettingsSectionBase
	{
		[Header("Tonemapping toggles")]
		[SerializeField]
		private Toggle TonemappingOffToggle;

		[SerializeField]
		private Toggle TonemappingGtToggle;

		[SerializeField]
		private Toggle TonemappingAgxToggle;

		[SerializeField]
		private Toggle TonemappingAcesToggle;

		[SerializeField]
		private SliderValueHandler EVCompensationSlider;

		[SerializeField]
		private SliderValueHandler WBCompensationSlider;

		[SerializeField]
		[Header("Other stuff")]
		private Toggle AntiAliasingToggle;

		[SerializeField]
		private Toggle VignetteToggle;

		[SerializeField]
		private Toggle BloomToggle;

		[SerializeField]
		private Toggle BlurToggle;

		[SerializeField]
		private Toggle HideSmokeToggle;

		[SerializeField]
		private Toggle HideSkidMarksToggle;

		[SerializeField]
		private Toggle HideWheelDirtToggle;

		[SerializeField]
		private Toggle MirrorsStateToggle;

		[SerializeField]
		private Toggle MotionBlurToggle;

		[Header("Time of day")]
		[SerializeField]
		private Toggle Day;

		[SerializeField]
		private Toggle Evening;

		[SerializeField]
		private Toggle Night;

		[SerializeField]
		private Toggle DayNightCycle;

		[SerializeField]
		private Toggle CustomTimeToggle;

		[SerializeField]
		private Button ColorCorrectionButton;

		[SerializeField]
		private SliderValueHandler TimeSlider;

		[SerializeField]
		private GameObject TimesSection;

		[SerializeField]
		private Toggle CustomTimeFormat12;

		[SerializeField]
		private Toggle CustomTimeFormat24;

		[Header("Weather")]
		[SerializeField]
		private GameObject WeatherSection;

		[SerializeField]
		private Toggle DynamicWeatherToggle;

		[SerializeField]
		private Toggle ClearWeatherToggle;

		[SerializeField]
		private Toggle HazedWeatherToggle;

		[SerializeField]
		private Toggle CloudyWeatherToggle;

		[SerializeField]
		private Toggle OvercastWeatherToggle;

		[SerializeField]
		private Toggle RainyWeatherToggle;

		[SerializeField]
		private Toggle StormyWeatherToggle;

		private UserPreferenceAnalytic _userPreferenceAnalytic;

		[Inject]
		public void Construct(UserPreferenceAnalytic userPreferenceAnalytic)
		{
			_userPreferenceAnalytic = userPreferenceAnalytic;
		}

		private void Start()
		{
			Restore();
			AntiAliasingToggle.onValueChanged.AddListener(OnAntiAliasingChanged);
			VignetteToggle.onValueChanged.AddListener(OnVignetteChanged);
			BloomToggle.onValueChanged.AddListener(OnBloomChanged);
			BlurToggle.onValueChanged.AddListener(OnBlurChanged);
			TonemappingOffToggle.onValueChanged.AddListener(SetTonemappingOFF);
			TonemappingGtToggle.onValueChanged.AddListener(SetTonemappingGT);
			TonemappingAgxToggle.onValueChanged.AddListener(SetTonemappingAGX);
			TonemappingAcesToggle.onValueChanged.AddListener(SetTonemappingACES);
			HideSmokeToggle.onValueChanged.AddListener(HideSkidSmoke);
			HideSkidMarksToggle.onValueChanged.AddListener(HideSkidMarks);
			HideWheelDirtToggle.onValueChanged.AddListener(HideWheelDirt);
			MirrorsStateToggle.onValueChanged.AddListener(ShowCarMirror);
			ColorCorrectionButton.onClick.AddListener(OpenColorCorrection);
			MotionBlurToggle.onValueChanged.AddListener(OnMotionBlurChanged);
			Day.onValueChanged.AddListener((bool value) =>
			{
				if (value)
				{
					OnSunnySkyboxChanged(0);
				}
			});
			Evening.onValueChanged.AddListener((bool value) =>
			{
				if (value)
				{
					OnSunnySkyboxChanged(1);
				}
			});
			Night.onValueChanged.AddListener((bool value) =>
			{
				if (value)
				{
					OnSunnySkyboxChanged(2);
				}
			});
			DayNightCycle.onValueChanged.AddListener((bool value) =>
			{
				if (value)
				{
					OnSunnySkyboxChanged(3);
				}
			});
			CustomTimeToggle.onValueChanged.AddListener((bool value) =>
			{
				if (value)
				{
					OnSunnySkyboxChanged(5);
				}
			});
			CustomTimeFormat24.onValueChanged.AddListener((bool value) =>
			{
				if (value)
				{
					TimeProvider.CustomTimeFormat24 = true;
					UpdateTimeSlider();
				}
			});
			CustomTimeFormat12.onValueChanged.AddListener((bool value) =>
			{
				if (value)
				{
					TimeProvider.CustomTimeFormat24 = false;
					UpdateTimeSlider();
				}
			});
			if ((bool)DynamicWeatherToggle)
			{
				DynamicWeatherToggle.gameObject.SetActive(value: false);
			}
			BindWeatherPresetToggle(ClearWeatherToggle, "clear");
			BindWeatherPresetToggle(HazedWeatherToggle, "hazed");
			BindWeatherPresetToggle(CloudyWeatherToggle, "cloudy");
			BindWeatherPresetToggle(OvercastWeatherToggle, "overcast");
			BindWeatherPresetToggle(RainyWeatherToggle, "rainy");
			BindWeatherPresetToggle(StormyWeatherToggle, "stormy");
			if (!ScenesHelper.IsFreeDriveSceneActive())
			{
				TimesSection.SetActive(value: false);
			}
			if (!ScenesHelper.IsFreeDriveSceneActive())
			{
				WeatherSection.SetActive(value: false);
			}
			TimeSlider.AddListenerClamped(OnTimeSliderChanged);
			TimeSlider.Init(GetHoursToString);
			EVCompensationSlider.OnReleased += OnEVCompensationSliderChanged;
			WBCompensationSlider.OnReleased += OnWBCompensationSliderChanged;
		}

		private void BindWeatherPresetToggle(Toggle toggle, string presetId)
		{
			toggle.onValueChanged.AddListener((bool value) =>
			{
				if (value)
				{
					OnWeatherPresetSelected(presetId);
				}
			});
		}

		private void OnWeatherPresetSelected(string presetId)
		{
			PlayerPrefs.SetString("weatherPreset", presetId);
			GlobalEventSystem.RequestSunController()?.SetWeatherByName(presetId);
		}

		private void OnEVCompensationSliderChanged(float val)
		{
			CPMGraphicsSettingsManager.Instance.EvCompensation = val;
			CPMGraphicsSettingsManager.Instance.SaveConfig();
		}

		private void OnWBCompensationSliderChanged(float val)
		{
			CPMGraphicsSettingsManager.Instance.WBCompensation = val;
			CPMGraphicsSettingsManager.Instance.SaveConfig();
		}

		private void OpenColorCorrection()
		{
			OpenColorCorrectionAsync().Forget();
		}

		private async UniTaskVoid OpenColorCorrectionAsync()
		{
			base.Settings.Hide();
			WindowScript windowScript = await ManagerUI.ShowColorPresetWindow();
			if (windowScript == null)
			{
				if ((bool)base.Settings)
				{
					base.Settings.Show();
				}
				return;
			}
			windowScript.InitOnCloseCallback(() =>
			{
				if ((bool)base.Settings)
				{
					base.Settings.Show();
				}
			});
		}

		private void OnSunnySkyboxChanged(int value)
		{
			TimeProvider.SelectedSkyboxPrefs = value;
			TimeProvider.skyboxSelectedRuntime = value;
			V.OnSkyboxChanged?.Invoke();
			UpdateTimeSlider();
		}

		private void OnAntiAliasingChanged(bool isOn)
		{
			_userPreferenceAnalytic.LogUserPreference(UserPreferenceType.anti_aliasing, (!isOn).ToString(), isOn.ToString());
			CPMGraphicsSettingsManager.Instance.AAQuality = (isOn ? CPMGraphicsSettingsManager.Instance.GetPresetAAQuality() : CPMGraphicsSettings.AAQuality.Off);
			CPMGraphicsSettingsManager.Instance.SaveConfig();
		}

		private void OnVignetteChanged(bool isOn)
		{
			_userPreferenceAnalytic.LogUserPreference(UserPreferenceType.vignette, (!isOn).ToString(), isOn.ToString());
			CPMGraphicsSettingsManager.Instance.UseVignette = isOn;
			CPMGraphicsSettingsManager.Instance.SaveConfig();
		}

		private void OnMotionBlurChanged(bool isOn)
		{
			CPMGraphicsSettingsManager.Instance.UseMotionBlur = isOn;
			CPMGraphicsSettingsManager.Instance.SaveConfig();
		}

		private void OnBloomChanged(bool isOn)
		{
			_userPreferenceAnalytic.LogUserPreference(UserPreferenceType.bloom, (!isOn).ToString(), isOn.ToString());
			CPMGraphicsSettingsManager.Instance.UseBloom = isOn;
			CPMGraphicsSettingsManager.Instance.SaveConfig();
		}

		private void OnBlurChanged(bool isOn)
		{
			_userPreferenceAnalytic.LogUserPreference(UserPreferenceType.ui_blur, (!isOn).ToString(), isOn.ToString());
			CPMGraphicsSettingsManager.Instance.UseBlur = isOn;
			CPMGraphicsSettingsManager.Instance.SaveConfig();
		}

		private void SetTonemappingOFF(bool isOn)
		{
			if (isOn)
			{
				_userPreferenceAnalytic.LogTonemapping(TonemappingMode.None);
				CPMGraphicsSettingsManager.Instance.Tonemapping = TonemappingMode.None;
				CPMGraphicsSettingsManager.Instance.SaveConfig();
			}
		}

		private void SetTonemappingGT(bool isOn)
		{
			if (isOn)
			{
				_userPreferenceAnalytic.LogTonemapping(TonemappingMode.GranTurismo);
				CPMGraphicsSettingsManager.Instance.Tonemapping = TonemappingMode.GranTurismo;
				CPMGraphicsSettingsManager.Instance.SaveConfig();
			}
		}

		private void SetTonemappingAGX(bool isOn)
		{
			if (isOn)
			{
				_userPreferenceAnalytic.LogTonemapping(TonemappingMode.Custom);
				CPMGraphicsSettingsManager.Instance.Tonemapping = TonemappingMode.Custom;
				CPMGraphicsSettingsManager.Instance.SaveConfig();
			}
		}

		private void SetTonemappingACES(bool isOn)
		{
			if (isOn)
			{
				_userPreferenceAnalytic.LogTonemapping(TonemappingMode.ACES);
				CPMGraphicsSettingsManager.Instance.Tonemapping = TonemappingMode.ACES;
				CPMGraphicsSettingsManager.Instance.SaveConfig();
			}
		}

		private void HideSkidSmoke(bool isOn)
		{
			_userPreferenceAnalytic.LogUserPreference(UserPreferenceType.hide_tire_smoke, (!isOn).ToString(), isOn.ToString());
			PlayerPrefs.SetInt("hideSkidSmoke", isOn ? 1 : 0);
			FlagsStorage.HideSkidSmoke = isOn;
		}

		private void HideSkidMarks(bool isOn)
		{
			_userPreferenceAnalytic.LogUserPreference(UserPreferenceType.hide_skidmarks, (!isOn).ToString(), isOn.ToString());
			PlayerPrefs.SetInt("hideSkidmarks", isOn ? 1 : 0);
			FlagsStorage.HideSkidMarks = isOn;
			Skidmarks.AllSkidMarksSetActive(!isOn);
		}

		private void HideWheelDirt(bool isOn)
		{
			_userPreferenceAnalytic.LogUserPreference(UserPreferenceType.hide_wheel_dirt, (!isOn).ToString(), isOn.ToString());
			PlayerPrefs.SetInt("hideWheelDirt", isOn ? 1 : 0);
			FlagsStorage.HideWheelDirt = isOn;
		}

		private void ShowCarMirror(bool isOn)
		{
			_userPreferenceAnalytic.LogUserPreference(UserPreferenceType.real_view_mirrors, (!isOn).ToString(), isOn.ToString());
			PlayerPrefs.SetInt("showCarMirror", isOn ? 1 : 0);
			GameObject gameObject = (FlagsStorage.RacingGameMode ? V.LastUsedCar : V.Car);
			if ((bool)gameObject)
			{
				FlagsStorage.ShowCarMirror = isOn;
				gameObject.GetComponent<VehicleData>().CarMirrorController.SetMirror(isOn);
			}
		}

		public override void Restore()
		{
			TonemappingMode tonemapping = CPMGraphicsSettingsManager.Instance.Tonemapping;
			TonemappingOffToggle.SetIsOnWithoutNotify(tonemapping == TonemappingMode.None);
			TonemappingGtToggle.SetIsOnWithoutNotify(tonemapping == TonemappingMode.GranTurismo);
			TonemappingAgxToggle.SetIsOnWithoutNotify(tonemapping == TonemappingMode.Custom);
			TonemappingAcesToggle.SetIsOnWithoutNotify(tonemapping == TonemappingMode.ACES);
			AntiAliasingToggle.isOn = CPMGraphicsSettingsManager.Instance.AAQuality != CPMGraphicsSettings.AAQuality.Off;
			VignetteToggle.isOn = CPMGraphicsSettingsManager.Instance.UseVignette;
			MotionBlurToggle.isOn = CPMGraphicsSettingsManager.Instance.UseMotionBlur;
			BloomToggle.isOn = CPMGraphicsSettingsManager.Instance.UseBloom;
			BlurToggle.isOn = CPMGraphicsSettingsManager.Instance.UseBlur;
			HideSmokeToggle.isOn = PlayerPrefs.GetInt("hideSkidSmoke", 0) == 1;
			HideSkidMarksToggle.isOn = PlayerPrefs.GetInt("hideSkidmarks", 0) == 1;
			HideWheelDirtToggle.isOn = PlayerPrefs.GetInt("hideWheelDirt", 0) == 1;
			MirrorsStateToggle.isOn = PlayerPrefs.GetInt("showCarMirror", 0) != 0;
			float evCompensation = CPMGraphicsSettingsManager.Instance.EvCompensation;
			EVCompensationSlider.SetTextWithoutNotify(evCompensation);
			EVCompensationSlider.SetValueWithoutNotify(evCompensation);
			float wBCompensation = CPMGraphicsSettingsManager.Instance.WBCompensation;
			WBCompensationSlider.SetTextWithoutNotify(wBCompensation);
			WBCompensationSlider.SetValueWithoutNotify(wBCompensation);
			RestoreWeatherToggles();
			RestoreTimeToggles();
			UpdateTimeSlider();
		}

		private void RestoreWeatherToggles()
		{
			string text = GlobalEventSystem.RequestSunController()?.CurrentWeatherPresetId;
			if (string.IsNullOrEmpty(text))
			{
				text = PlayerPrefs.GetString("weatherPreset", "clear");
			}
			ClearWeatherToggle.SetIsOnWithoutNotify(text == "clear");
			HazedWeatherToggle.SetIsOnWithoutNotify(text == "hazed");
			CloudyWeatherToggle.SetIsOnWithoutNotify(text == "cloudy");
			OvercastWeatherToggle.SetIsOnWithoutNotify(text == "overcast");
			RainyWeatherToggle.SetIsOnWithoutNotify(text == "rainy");
			StormyWeatherToggle.SetIsOnWithoutNotify(text == "stormy");
		}

		private void RestoreTimeToggles()
		{
			int selectedSkyboxPrefs = TimeProvider.SelectedSkyboxPrefs;
			Day.SetIsOnWithoutNotify(selectedSkyboxPrefs == 0);
			Evening.SetIsOnWithoutNotify(selectedSkyboxPrefs == 1);
			Night.SetIsOnWithoutNotify(selectedSkyboxPrefs == 2);
			DayNightCycle.SetIsOnWithoutNotify(selectedSkyboxPrefs == 3);
			CustomTimeToggle.SetIsOnWithoutNotify(selectedSkyboxPrefs == 5);
			CustomTimeFormat12.SetIsOnWithoutNotify(!TimeProvider.CustomTimeFormat24);
			CustomTimeFormat24.SetIsOnWithoutNotify(TimeProvider.CustomTimeFormat24);
		}

		private void OnTimeSliderChanged(float hours)
		{
			TimeProvider.CustomHours = (int)hours;
			OnSunnySkyboxChanged(5);
			RestoreTimeToggles();
			TimeSlider.SetText(TimeProvider.CustomHours);
		}

		private void UpdateTimeSlider()
		{
			if (TimeProvider.SelectedSkyboxPrefs == 0)
			{
				TimeSlider.SetValueWithoutNotify(14f);
				TimeSlider.SetText(14f);
			}
			else if (TimeProvider.SelectedSkyboxPrefs == 1)
			{
				TimeSlider.SetValueWithoutNotify(16f);
				TimeSlider.SetText(16f);
			}
			else if (TimeProvider.SelectedSkyboxPrefs == 2)
			{
				TimeSlider.SetValueWithoutNotify(1f);
				TimeSlider.SetText(1f);
			}
			else if (TimeProvider.SelectedSkyboxPrefs == 3)
			{
				TimeSlider.SetValueWithoutNotify(12f);
				TimeSlider.SetText(12f);
			}
			else
			{
				TimeSlider.SetValueWithoutNotify(TimeProvider.CustomHours);
				TimeSlider.SetText(TimeProvider.CustomHours);
			}
		}

		private string GetHoursToString(float hours)
		{
			string text = "";
			if (TimeProvider.CustomTimeFormat24)
			{
				return $"{(int)hours:00}:00";
			}
			int num = (int)hours;
			string arg = ((num < 12) ? "AM" : "PM");
			if (num == 0)
			{
				num = 12;
			}
			else if (num > 12)
			{
				num -= 12;
			}
			return $"{num:00}:00 {arg}";
		}
	}
}
