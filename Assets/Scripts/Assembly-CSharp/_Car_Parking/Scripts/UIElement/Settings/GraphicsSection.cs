using System;
using System.Collections.Generic;
using System.Globalization;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Traffic;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class GraphicsSection : SettingsSectionBase
	{
		public const string GraphicsQualityKey = "graphics_quality";

		private const float DisabledRowAlpha = 0.35f;

		[Header("Quality toggles")]
		[SerializeField]
		private Toggle VeryLowSettingsToggle = default;

		[SerializeField]
		private Toggle LowSettingsToggle = default;

		[SerializeField]
		private Toggle NormalSettingsToggle = default;

		[SerializeField]
		private Toggle HighSettingsToggle = default;

		[SerializeField]
		[Header("Display mode toggles")]
		private Toggle WindowedToggle = default;

		[SerializeField]
		private Toggle FullscreenWindowToggle = default;

		[SerializeField]
		private Toggle FullscreenToggle = default;

		[Header("Shadow toggles")]
		[SerializeField]
		private Toggle ShadowOffToggle = default;

		[SerializeField]
		private Toggle LowShadowToggle = default;

		[SerializeField]
		private Toggle NormalShadowToggle = default;

		[SerializeField]
		private Toggle HighShadowToggle = default;

		[SerializeField]
		[Header("Fog toggles")]
		private Toggle FogOffToggle = default;

		[SerializeField]
		private Toggle FogOnToggle = default;

		[Header("Shader toggles")]
		[SerializeField]
		private Toggle ShadersLowToggle = default;

		[SerializeField]
		private Toggle ShadersHighToggle = default;

		[SerializeField]
		private Toggle ShadersUltraToggle = default;

		[Header("Additional Detail toggles")]
		[SerializeField]
		private Toggle DetailsLowToggle = default;

		[SerializeField]
		private Toggle DetailMidToggle = default;

		[SerializeField]
		private Toggle DetailHighToggle = default;

		[SerializeField]
		[Header("Decal toggles")]
		private Toggle DecalsOnToggle = default;

		[SerializeField]
		private Toggle DecalsOffToggle = default;

		[SerializeField]
		[Tooltip("Визуальная деформация шины в пятне контакта. Пресет Ultra включает её сам, остальные выключают, но переключить можно на любом.")]
		[Header("Tyre deformation toggles")]
		private Toggle TyreDeformationOnToggle = default;

		[SerializeField]
		private Toggle TyreDeformationOffToggle = default;

		[SerializeField]
		[Header("Rain effects toggles")]
		[Tooltip("Off — только лужи и мокрые поверхности; Minimal — рипплы, всплески от колёс, конус капель; All — плюс капли на кузове/стекле, дворники и капли на экране.")]
		private Toggle RainEffectsOffToggle = default;

		[SerializeField]
		private Toggle RainEffectsMinimalToggle = default;

		[SerializeField]
		private Toggle RainEffectsAllToggle = default;

		[Header("Battery save toggles")]
		[SerializeField]
		private Toggle BatterySavingOnToggle = default;

		[SerializeField]
		private Toggle BatterySavingOffToggle = default;

		[SerializeField]
		[Header("Resolution toggles")]
		private Toggle[] ResolutionToggles = default;

		[Header("Resolution texts")]
		[SerializeField]
		private TextMeshProUGUI[] ResolutionTexts = default;

		[Header("Resolution")]
		[SerializeField]
		private SliderValueHandler ResolutionSlider = default;

		[Header("Choose display")]
		[SerializeField]
		private TMP_Dropdown DisplayDropdown = default;

		[SerializeField]
		private GameObject DisplayDropdownObject = default;

		[SerializeField]
		[Header("Distance sliders")]
		private SliderValueHandler RenderDistanceSlider = default;

		[SerializeField]
		private SliderValueHandler PropsDistanceSlider = default;

		[SerializeField]
		private SliderValueHandler CarDetailDistanceSlider = default;

		[Header("Framerate")]
		[SerializeField]
		private SliderValueHandler FrameRate = default;

		[SerializeField]
		[Tooltip("Строка слайдера кадров целиком: гаснет, пока включён VSync.")]
		private CanvasGroup FrameRateCanvasGroup = default;

		[Header("VSync toggles")]
		[SerializeField]
		[Tooltip("При включённом VSync кадры синхронизируются с развёрткой монитора, а слайдер Frame rate ни на что не влияет.")]
		private Toggle VSyncOnToggle = default;

		[SerializeField]
		private Toggle VSyncOffToggle = default;

		[Header("Canvas groups")]
		[SerializeField]
		private CanvasGroup FarClippingCanvasGroup = default;

		[SerializeField]
		private CanvasGroup ShortClippingCanvasGroup = default;

		[SerializeField]
		private CanvasGroup CarDetailCanvasGroup = default;

		[SerializeField]
		private CanvasGroup DisplayModeCanvasGroup = default;

		[SerializeField]
		private CanvasGroup ResolutionNumbersCanvasGroup = default;

		[Header("Real-time Reflection")]
		[SerializeField]
		private Toggle RealtimeReflectionOnToggle = default;

		[SerializeField]
		private Toggle RealtimeReflectionOffToggle = default;

		[SerializeField]
		[Header("Static Reflection")]
		private Toggle StaticReflectionOnToggle = default;

		[SerializeField]
		private Toggle StaticReflectionOffToggle = default;

		private float _renderDistanceStartValue;

		private float _propsRenderDistanceStartValue;

		private float _carRenderDistanceStartValue;

		private Camera _camera;

		private OptionalDependency<Camera> _cameraDep;

		private FPSDisplay2 _fpsDisplay;

		private UserPreferenceAnalytic _userPreferenceAnalytic;

		private CPMGraphicsSettingsManager _graphicSettings => CPMGraphicsSettingsManager.Instance;

		[Inject]
		public void Construct(UserPreferenceAnalytic userPreferenceAnalytic, OptionalDependency<Camera> currentCamera)
		{
			_cameraDep = currentCamera;
			_userPreferenceAnalytic = userPreferenceAnalytic;
		}

		protected override void Awake()
		{
			base.Awake();
			_camera = _cameraDep?.Value;
		}

		private void OnEnable()
		{
			SaveStartValues();
		}

		private void OnDisable()
		{
			SendAnalytics();
		}

		private void Start()
		{
			Restore();
			if ((bool)_camera)
			{
				_fpsDisplay = _camera.GetComponent<FPSDisplay2>();
			}
			VeryLowSettingsToggle.onValueChanged.AddListener(SetLowQuality);
			LowSettingsToggle.onValueChanged.AddListener(SetNormalQuality);
			NormalSettingsToggle.onValueChanged.AddListener(SetHighQuality);
			HighSettingsToggle.onValueChanged.AddListener(SetUltraQuality);
			ShadowOffToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetShadows(isOn, CPMGraphicsSettings.ShadowsQuality.Off);
			});
			LowShadowToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetShadows(isOn, CPMGraphicsSettings.ShadowsQuality.Low);
			});
			NormalShadowToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetShadows(isOn, CPMGraphicsSettings.ShadowsQuality.Mid);
			});
			HighShadowToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetShadows(isOn, CPMGraphicsSettings.ShadowsQuality.Ultra);
			});
			FogOffToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetFog(isOn, CPMGraphicsSettings.FogQuality.Off);
			});
			FogOnToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetFog(isOn, CPMGraphicsSettings.FogQuality.On);
			});
			ShadersLowToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetShaders(isOn, CPMGraphicsSettings.ShadersQuality.Potato);
			});
			ShadersHighToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetShaders(isOn, CPMGraphicsSettings.ShadersQuality.High);
			});
			ShadersUltraToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetShaders(isOn, CPMGraphicsSettings.ShadersQuality.Ultra);
			});
			DetailsLowToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetAdditionalDetails(isOn, CPMGraphicsSettings.AdditionalEffects.Low);
			});
			DetailMidToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetAdditionalDetails(isOn, CPMGraphicsSettings.AdditionalEffects.Mid);
			});
			DetailHighToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetAdditionalDetails(isOn, CPMGraphicsSettings.AdditionalEffects.High);
			});
			DecalsOnToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetDecals(isOn, val: true);
			});
			DecalsOffToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetDecals(isOn, val: false);
			});
			TyreDeformationOnToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetTyreDeformation(isOn, val: true);
			});
			TyreDeformationOffToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetTyreDeformation(isOn, val: false);
			});
			RainEffectsOffToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetRainEffects(isOn, CPMGraphicsSettings.RainQuality.Off);
			});
			RainEffectsMinimalToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetRainEffects(isOn, CPMGraphicsSettings.RainQuality.Minimal);
			});
			RainEffectsAllToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetRainEffects(isOn, CPMGraphicsSettings.RainQuality.All);
			});
			BatterySavingOnToggle.onValueChanged.AddListener(SetBatterySaveMode);
			if ((bool)VSyncOnToggle)
			{
				VSyncOnToggle.onValueChanged.AddListener((bool isOn) =>
				{
					SetVSync(isOn, val: true);
				});
			}
			if ((bool)VSyncOffToggle)
			{
				VSyncOffToggle.onValueChanged.AddListener((bool isOn) =>
				{
					SetVSync(isOn, val: false);
				});
			}
			ResolutionSlider.OnReleased += SetResolutionPercent;
			RenderDistanceSlider.OnReleased += OnFarClipChanged;
			SliderValueHandler renderDistanceSlider = RenderDistanceSlider;
			renderDistanceSlider.OnPointerDownAction = (Action)Delegate.Combine(renderDistanceSlider.OnPointerDownAction, new Action(OnFarClipPressed));
			RenderDistanceSlider.AddListener(OnFarClipChanging);
			PropsDistanceSlider.OnReleased += OnShortClipChanged;
			SliderValueHandler propsDistanceSlider = PropsDistanceSlider;
			propsDistanceSlider.OnPointerDownAction = (Action)Delegate.Combine(propsDistanceSlider.OnPointerDownAction, new Action(OnShortClipPressed));
			PropsDistanceSlider.AddListener(OnShortClipChanging);
			CarDetailDistanceSlider.AddListener(OnDetailDistanceChanging);
			CarDetailDistanceSlider.OnReleased += OnDetailDistanceChanged;
			SliderValueHandler carDetailDistanceSlider = CarDetailDistanceSlider;
			carDetailDistanceSlider.OnPointerDownAction = (Action)Delegate.Combine(carDetailDistanceSlider.OnPointerDownAction, new Action(OnCarDetailClipPressed));
			FrameRate.OnReleased += OnTargetFrameRateChanged;
			RealtimeReflectionOnToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetRealtimeReflection(isOn, val: true);
			});
			RealtimeReflectionOffToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetRealtimeReflection(isOn, val: false);
			});
			StaticReflectionOnToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetStaticReflection(isOn, val: true);
			});
			StaticReflectionOffToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetStaticReflection(isOn, val: false);
			});
			DisplayModeCanvasGroup.gameObject.SetActive(value: true);
			WindowedToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetDisplayMode(isOn, FullScreenMode.Windowed);
			});
			FullscreenWindowToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetDisplayMode(isOn, FullScreenMode.FullScreenWindow);
			});
			FullscreenToggle.onValueChanged.AddListener((bool isOn) =>
			{
				SetDisplayMode(isOn, FullScreenMode.ExclusiveFullScreen);
			});
			DisplayDropdownObject.SetActive(value: true);
			ResolutionNumbersCanvasGroup.gameObject.SetActive(value: true);
			for (int num = 0; num < ResolutionToggles.Length; num++)
			{
				if (num < _graphicSettings._availableResolutions.Count)
				{
					int j = num;
					ResolutionToggles[num].gameObject.SetActive(value: true);
					ResolutionToggles[num].onValueChanged.AddListener((bool isOn) =>
					{
						SetResolution(isOn, new int[2]
						{
							_graphicSettings._availableResolutions[j].width,
							_graphicSettings._availableResolutions[j].height
						});
					});
				}
			}
			if ((bool)DisplayDropdown)
			{
				DisplayDropdown.onValueChanged.AddListener(SetDisplay);
			}
		}

		private void SetDisplayMode(bool isOn, FullScreenMode fullScreenMode)
		{
			if (isOn)
			{
				_userPreferenceAnalytic.LogDisplayMode(fullScreenMode, _graphicSettings.DisplayMode);
				_graphicSettings.DisplayMode = fullScreenMode;
				_graphicSettings.SaveConfig();
			}
		}

		private void SetResolution(bool isOn, int[] resolution)
		{
			if (isOn)
			{
				_userPreferenceAnalytic.LogResolution(resolution, _graphicSettings.Resolution);
				_graphicSettings.Resolution = resolution;
				_graphicSettings.SaveConfig();
			}
		}

		private void PopulateDisplays()
		{
			if ((bool)DisplayDropdown)
			{
				List<DisplayInfo> displays = _graphicSettings.GetDisplays();
				DisplayDropdown.ClearOptions();
				List<string> list = new List<string>(displays.Count);
				for (int i = 0; i < displays.Count; i++)
				{
					DisplayInfo displayInfo = displays[i];
					list.Add(string.IsNullOrEmpty(displayInfo.name) ? $"Display {i + 1}" : displayInfo.name);
				}
				DisplayDropdown.AddOptions(list);
			}
		}

		private void SetDisplay(int index)
		{
			_graphicSettings.DisplayIndex = index;
			_graphicSettings.SaveConfig();
		}

		private void OnTargetFrameRateChanged(float value)
		{
			base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.frame_rate, _graphicSettings.MaxFramerate.ToString(), (value * 30f).ToString(CultureInfo.InvariantCulture));
			_graphicSettings.MaxFramerate = (int)value * 30;
			if (value > 1f)
			{
				BatterySavingOffToggle.SetIsOnWithoutNotify(value: true);
				BatterySavingOnToggle.SetIsOnWithoutNotify(value: false);
			}
			_graphicSettings.SaveConfig();
		}

		private void SetShadows(bool isOn, CPMGraphicsSettings.ShadowsQuality shadowsQuality)
		{
			if (isOn)
			{
				_userPreferenceAnalytic.LogShadowsQuality(shadowsQuality, _graphicSettings.ShadowsQuality);
				ManagerUI.ShowWaitingWindow(set: true).Forget();
				_graphicSettings.ShadowsQuality = shadowsQuality;
				_graphicSettings.SaveConfig();
				Observable.TimerFrame(10).Subscribe((long _) =>
				{
					ManagerUI.CloseWaitingWindow();
					ManagerUI.OnBackButtonActivated?.Invoke(obj: true);
				}).AddTo(this);
			}
		}

		private void SetFog(bool isOn, CPMGraphicsSettings.FogQuality quality)
		{
			if (isOn)
			{
				_userPreferenceAnalytic.LogFogQuality(quality, _graphicSettings.FogQuality);
				_graphicSettings.FogQuality = quality;
				_graphicSettings.SaveConfig();
			}
		}

		private void SetShaders(bool isOn, CPMGraphicsSettings.ShadersQuality quality)
		{
			if (isOn)
			{
				_userPreferenceAnalytic.LogShadersQuality(quality, _graphicSettings.ShadingQuality);
				_graphicSettings.ShadingQuality = quality;
				_graphicSettings.SaveConfig();
			}
		}

		private void SetAdditionalDetails(bool isOn, CPMGraphicsSettings.AdditionalEffects quality)
		{
			if (isOn)
			{
				_userPreferenceAnalytic.LogGraphicsQuality(quality, _graphicSettings.AdditionalEffects);
				_graphicSettings.AdditionalEffects = quality;
				_graphicSettings.SaveConfig();
			}
		}

		private void SetDecals(bool isOn, bool val)
		{
			if (isOn)
			{
				base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.decals_change, _graphicSettings.UseDecalShadows.ToString(), val.ToString());
				_graphicSettings.UseDecalShadows = val;
				_graphicSettings.SaveConfig();
			}
		}

		private void SetTyreDeformation(bool isOn, bool val)
		{
			if (isOn)
			{
				base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.tyre_deformation, _graphicSettings.UseTyreDeformation.ToString(), val.ToString());
				_graphicSettings.UseTyreDeformation = val;
				_graphicSettings.SaveConfig();
			}
		}

		private void SetVSync(bool isOn, bool val)
		{
			if (isOn)
			{
				_graphicSettings.VSync = val;
				_graphicSettings.SaveConfig();
				RefreshFrameRateAvailability();
			}
		}

		private void RefreshFrameRateAvailability()
		{
			if (!FrameRateCanvasGroup)
			{
				return;
			}
			bool flag = !_graphicSettings.VSync;
			FrameRateCanvasGroup.alpha = (flag ? 1f : 0.35f);
			FrameRateCanvasGroup.interactable = flag;
			FrameRateCanvasGroup.blocksRaycasts = flag;
			if (!flag)
			{
				UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
				if ((bool)current && (bool)current.currentSelectedGameObject && current.currentSelectedGameObject.transform.IsChildOf(FrameRateCanvasGroup.transform))
				{
					current.SetSelectedGameObject(VSyncOnToggle ? VSyncOnToggle.gameObject : null);
				}
			}
		}

		private void SetRainEffects(bool isOn, CPMGraphicsSettings.RainQuality quality)
		{
			if (isOn)
			{
				base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.rain_effects, _graphicSettings.RainQuality.ToString(), quality.ToString());
				_graphicSettings.RainQuality = quality;
				_graphicSettings.SaveConfig();
			}
		}

		private void SetBatterySaveMode(bool isOn)
		{
			base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.battery_saving, _graphicSettings.BatterySaveMode.ToString(), isOn.ToString());
			_graphicSettings.BatterySaveMode = isOn;
			if (isOn)
			{
				FrameRate.SetValue(1f);
			}
			if (Application.isEditor && !isOn)
			{
				_graphicSettings.MaxFramerate = 150;
			}
			_graphicSettings.SaveConfig();
		}

		private void SetRealtimeReflection(bool isOn, bool val)
		{
			if (isOn)
			{
				base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.real_time_reflection, AnalyticKeyMapper.GetMappedValue(_graphicSettings.RealtimeReflections), val ? AnalyticKeyMapper.GetMappedValue(CPMGraphicsSettings.RealtimeRefections.Medium) : AnalyticKeyMapper.GetMappedValue(CPMGraphicsSettings.RealtimeRefections.Off));
				_graphicSettings.RealtimeReflections = (val ? CPMGraphicsSettings.RealtimeRefections.Medium : CPMGraphicsSettings.RealtimeRefections.Off);
				_graphicSettings.SaveConfig();
			}
		}

		private void SetStaticReflection(bool isOn, bool val)
		{
			if (isOn)
			{
				base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.static_reflection, _graphicSettings.UseStaticReflections.ToString(), val.ToString());
				_graphicSettings.UseStaticReflections = val;
				_graphicSettings.SaveConfig();
			}
		}

		private void OnDetailDistanceChanging(float value)
		{
			V.DetailDistance = value;
		}

		private void OnDetailDistanceChanged(float value)
		{
			HideUI(mustHide: false, CarDetailCanvasGroup);
			_graphicSettings.CarDetailDistance = value;
			_graphicSettings.SaveConfig();
		}

		private void OnCarDetailClipPressed()
		{
			HideUI(mustHide: true, CarDetailCanvasGroup);
		}

		private void OnFarClipPressed()
		{
			HideUI(mustHide: true, FarClippingCanvasGroup);
		}

		private void HideUI(bool mustHide, CanvasGroup canvasToIgnore)
		{
			canvasToIgnore.ignoreParentGroups = mustHide;
			base.Settings.AllElements.alpha = (mustHide ? 0.03f : 1f);
			base.Settings.AllElements.interactable = !mustHide;
		}

		private void OnFarClipChanging(float farClip)
		{
			_graphicSettings.RenderDistanceController.SetSettings(GraphicsDistanceController.PriorityType.Default, farClip);
		}

		private void OnFarClipChanged(float farClip)
		{
			HideUI(mustHide: false, FarClippingCanvasGroup);
			_graphicSettings.RenderDistanceController.SetSettings(GraphicsDistanceController.PriorityType.Default, farClip);
			CPMGraphicsSettingsManager.Instance.SaveConfig();
			ObscuredPrefs.SetBool("AutoFarClip", value: false);
			if ((bool)_fpsDisplay)
			{
				_fpsDisplay.AutoFarClip = false;
			}
		}

		private void OnShortClipPressed()
		{
			HideUI(mustHide: true, ShortClippingCanvasGroup);
		}

		private void OnShortClipChanging(float clipDistance)
		{
			_graphicSettings.RenderPropsDistanceController.SetSettings(GraphicsDistanceController.PriorityType.Default, clipDistance);
		}

		private void OnShortClipChanged(float value)
		{
			HideUI(mustHide: false, ShortClippingCanvasGroup);
			_graphicSettings.SaveConfig();
		}

		private void SetResolutionPercent(float value)
		{
			float num = value * 0.01f;
			base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.game_resolution, (_graphicSettings.RenderScale * 100f).ToString(CultureInfo.InvariantCulture), (num * 100f).ToString(CultureInfo.InvariantCulture));
			_graphicSettings.RenderScale = num;
			_graphicSettings.SaveConfig();
		}

		private void SetLowQuality(bool isOn)
		{
			_userPreferenceAnalytic.LogGraphicsQuality(GraphicsQuality.Fast);
			if (isOn)
			{
				CheckTrafficCarDensityForCurrentQuality(0f);
				SteamApplyTrafficPresetDefaults(0);
				SetQualityUrp(0).Forget();
				OnDetailDistanceChanged(35f);
			}
		}

		private void SetNormalQuality(bool isOn)
		{
			_userPreferenceAnalytic.LogGraphicsQuality(GraphicsQuality.Medium);
			if (isOn)
			{
				CheckTrafficCarDensityForCurrentQuality(10f);
				SteamApplyTrafficPresetDefaults(1);
				SetQualityUrp(1).Forget();
			}
		}

		private void SetHighQuality(bool isOn)
		{
			_userPreferenceAnalytic.LogGraphicsQuality(GraphicsQuality.High);
			if (isOn)
			{
				CheckTrafficCarDensityForCurrentQuality(20f);
				SteamApplyTrafficPresetDefaults(2);
				SetQualityUrp(2).Forget();
			}
		}

		private void SetUltraQuality(bool isOn)
		{
			_userPreferenceAnalytic.LogGraphicsQuality(GraphicsQuality.Ultra);
			if (isOn)
			{
				CheckTrafficCarDensityForCurrentQuality(20f);
				SteamApplyTrafficPresetDefaults(4);
				SetQualityUrp(4).Forget();
			}
		}

		private async UniTask SetQualityUrp(int quality)
		{
			base.Settings.Hide();
			await ManagerUI.ShowWaitingWindow(set: true, backButtonState: true);
			Observable.NextFrame().Subscribe((Unit _) =>
			{
				_graphicSettings.ApplyPreset(quality);
			}).AddTo(this);
			Observable.TimerFrame(4).Subscribe((long _) =>
			{
				Restore();
				ManagerUI.CloseWaitingWindow();
				base.Settings.Show();
			}).AddTo(this);
		}

		private void SteamApplyTrafficPresetDefaults(int quality)
		{
			if (!SteamTrafficLimits.DensityChosenByUser)
			{
				SteamTrafficLimits.ApplyPresetDefaults(quality);
				OnTrafficCarDensityChanged(SteamTrafficLimits.DefaultForQuality(quality), withMessage: false);
			}
		}

		private void CheckTrafficCarDensityForCurrentQuality(float newDensity)
		{
		}

		public override void Restore()
		{
			int quality = _graphicSettings.Quality;
			VeryLowSettingsToggle.SetIsOnWithoutNotify(quality == 0);
			LowSettingsToggle.SetIsOnWithoutNotify(quality == 1);
			NormalSettingsToggle.SetIsOnWithoutNotify(quality == 2);
			HighSettingsToggle.SetIsOnWithoutNotify(quality == 4);
			CPMGraphicsSettings.ShadowsQuality shadowsQuality = _graphicSettings.ShadowsQuality;
			ShadowOffToggle.SetIsOnWithoutNotify(shadowsQuality == CPMGraphicsSettings.ShadowsQuality.Off);
			LowShadowToggle.SetIsOnWithoutNotify(shadowsQuality == CPMGraphicsSettings.ShadowsQuality.Low);
			NormalShadowToggle.SetIsOnWithoutNotify(shadowsQuality == CPMGraphicsSettings.ShadowsQuality.Mid);
			HighShadowToggle.SetIsOnWithoutNotify(shadowsQuality == CPMGraphicsSettings.ShadowsQuality.Ultra);
			CPMGraphicsSettings.FogQuality fogQuality = _graphicSettings.FogQuality;
			FogOffToggle.SetIsOnWithoutNotify(fogQuality == CPMGraphicsSettings.FogQuality.Off);
			FogOnToggle.SetIsOnWithoutNotify(fogQuality == CPMGraphicsSettings.FogQuality.On);
			CPMGraphicsSettings.ShadersQuality shadingQuality = _graphicSettings.ShadingQuality;
			ShadersLowToggle.SetIsOnWithoutNotify(shadingQuality == CPMGraphicsSettings.ShadersQuality.Potato);
			ShadersHighToggle.SetIsOnWithoutNotify(shadingQuality == CPMGraphicsSettings.ShadersQuality.High);
			ShadersUltraToggle.SetIsOnWithoutNotify(shadingQuality == CPMGraphicsSettings.ShadersQuality.Ultra);
			CPMGraphicsSettings.AdditionalEffects additionalEffects = _graphicSettings.AdditionalEffects;
			DetailsLowToggle.SetIsOnWithoutNotify(additionalEffects == CPMGraphicsSettings.AdditionalEffects.Low);
			DetailMidToggle.SetIsOnWithoutNotify(additionalEffects == CPMGraphicsSettings.AdditionalEffects.Mid);
			DetailHighToggle.SetIsOnWithoutNotify(additionalEffects == CPMGraphicsSettings.AdditionalEffects.High);
			DecalsOffToggle.SetIsOnWithoutNotify(!_graphicSettings.UseDecalShadows);
			DecalsOnToggle.SetIsOnWithoutNotify(_graphicSettings.UseDecalShadows);
			TyreDeformationOffToggle.SetIsOnWithoutNotify(!_graphicSettings.UseTyreDeformation);
			TyreDeformationOnToggle.SetIsOnWithoutNotify(_graphicSettings.UseTyreDeformation);
			CPMGraphicsSettings.RainQuality rainQuality = _graphicSettings.RainQuality;
			RainEffectsOffToggle.SetIsOnWithoutNotify(rainQuality == CPMGraphicsSettings.RainQuality.Off);
			RainEffectsMinimalToggle.SetIsOnWithoutNotify(rainQuality == CPMGraphicsSettings.RainQuality.Minimal);
			RainEffectsAllToggle.SetIsOnWithoutNotify(rainQuality == CPMGraphicsSettings.RainQuality.All);
			RenderDistanceSlider.SetValueWithoutNotify(_graphicSettings.RenderDistanceController.GetDistanceSettings(GraphicsDistanceController.PriorityType.Default));
			RenderDistanceSlider.SetTextRounded(_graphicSettings.RenderDistanceController.GetDistanceSettings(GraphicsDistanceController.PriorityType.Default));
			PropsDistanceSlider.SetValueWithoutNotify(_graphicSettings.ClipDistance);
			PropsDistanceSlider.SetTextRounded(_graphicSettings.ClipDistance);
			CarDetailDistanceSlider.SetValueWithoutNotify(_graphicSettings.CarDetailDistance);
			CarDetailDistanceSlider.SetTextRounded(_graphicSettings.CarDetailDistance);
			int num = _graphicSettings.MaxFramerate / 30;
			FrameRate.SetText(num);
			FrameRate.SetValueWithoutNotify(num);
			if ((bool)VSyncOnToggle)
			{
				VSyncOnToggle.SetIsOnWithoutNotify(_graphicSettings.VSync);
			}
			if ((bool)VSyncOffToggle)
			{
				VSyncOffToggle.SetIsOnWithoutNotify(!_graphicSettings.VSync);
			}
			RefreshFrameRateAvailability();
			BatterySavingOnToggle.SetIsOnWithoutNotify(_graphicSettings.BatterySaveMode);
			BatterySavingOffToggle.SetIsOnWithoutNotify(!_graphicSettings.BatterySaveMode);
			RealtimeReflectionOnToggle.SetIsOnWithoutNotify(_graphicSettings.RealtimeReflections != CPMGraphicsSettings.RealtimeRefections.Off);
			RealtimeReflectionOffToggle.SetIsOnWithoutNotify(_graphicSettings.RealtimeReflections == CPMGraphicsSettings.RealtimeRefections.Off);
			StaticReflectionOnToggle.SetIsOnWithoutNotify(_graphicSettings.UseStaticReflections);
			StaticReflectionOffToggle.SetIsOnWithoutNotify(!_graphicSettings.UseStaticReflections);
			float num2 = _graphicSettings.RenderScale * 100f;
			ResolutionSlider.SetTextRounded(num2);
			ResolutionSlider.SetValueWithoutNotify(num2);
			FullScreenMode displayMode = _graphicSettings.DisplayMode;
			WindowedToggle.SetIsOnWithoutNotify(displayMode == FullScreenMode.Windowed);
			FullscreenWindowToggle.SetIsOnWithoutNotify(displayMode == FullScreenMode.FullScreenWindow);
			FullscreenToggle.SetIsOnWithoutNotify(displayMode == FullScreenMode.ExclusiveFullScreen);
			int[] resolution = _graphicSettings.Resolution;
			if (resolution != null && resolution.Length == 2)
			{
				for (int i = 0; i < ResolutionToggles.Length; i++)
				{
					bool flag = false;
					if (i < _graphicSettings._availableResolutions.Count)
					{
						Resolution resolution2 = _graphicSettings._availableResolutions[i];
						flag = resolution[0] == resolution2.width && resolution[1] == resolution2.height;
						ResolutionToggles[i].SetIsOnWithoutNotify(flag);
						ResolutionTexts[i].text = $"{_graphicSettings._availableResolutions[i].width}×{_graphicSettings._availableResolutions[i].height}";
					}
				}
			}
			if ((bool)DisplayDropdown)
			{
				PopulateDisplays();
				int valueWithoutNotify = Mathf.Clamp(_graphicSettings.DisplayIndex, 0, Mathf.Max(0, DisplayDropdown.options.Count - 1));
				DisplayDropdown.SetValueWithoutNotify(valueWithoutNotify);
				DisplayDropdown.RefreshShownValue();
			}
		}

		private void SaveStartValues()
		{
			_renderDistanceStartValue = _graphicSettings.RenderDistanceController.GetDistanceSettings(GraphicsDistanceController.PriorityType.Default);
			_propsRenderDistanceStartValue = _graphicSettings.RenderPropsDistanceController.GetDistanceSettings(GraphicsDistanceController.PriorityType.Default);
			_carRenderDistanceStartValue = V.DetailDistance;
		}

		private void SendAnalytics()
		{
			LogAnalytics(UserPreferenceType.car_detail_distance, _carRenderDistanceStartValue, V.DetailDistance);
			float distanceSettings = _graphicSettings.RenderPropsDistanceController.GetDistanceSettings(GraphicsDistanceController.PriorityType.Default);
			LogAnalytics(UserPreferenceType.props_distance, _propsRenderDistanceStartValue, distanceSettings);
			float distanceSettings2 = _graphicSettings.RenderDistanceController.GetDistanceSettings(GraphicsDistanceController.PriorityType.Default);
			LogAnalytics(UserPreferenceType.render_distance, _renderDistanceStartValue, distanceSettings2);
		}

		private void LogAnalytics(UserPreferenceType type, float startValue, float currentValue)
		{
			if (!Mathf.Approximately(startValue, currentValue))
			{
				base.UserPreferenceAnalytic.LogUserPreference(type, startValue.ToString(CultureInfo.InvariantCulture), currentValue.ToString(CultureInfo.InvariantCulture));
			}
		}

		private void OnDestroy()
		{
			RenderDistanceSlider.OnReleased -= OnFarClipChanged;
			SliderValueHandler renderDistanceSlider = RenderDistanceSlider;
			renderDistanceSlider.OnPointerDownAction = (Action)Delegate.Remove(renderDistanceSlider.OnPointerDownAction, new Action(OnFarClipPressed));
			RenderDistanceSlider.RemoveAllListeners();
			PropsDistanceSlider.OnReleased -= OnShortClipChanged;
			SliderValueHandler propsDistanceSlider = PropsDistanceSlider;
			propsDistanceSlider.OnPointerDownAction = (Action)Delegate.Remove(propsDistanceSlider.OnPointerDownAction, new Action(OnShortClipPressed));
			PropsDistanceSlider.RemoveAllListeners();
			CarDetailDistanceSlider.RemoveAllListeners();
			CarDetailDistanceSlider.OnReleased -= OnDetailDistanceChanged;
			FrameRate.OnReleased -= OnTargetFrameRateChanged;
			if ((bool)DisplayDropdown)
			{
				DisplayDropdown.onValueChanged.RemoveListener(SetDisplay);
			}
		}
	}
}
