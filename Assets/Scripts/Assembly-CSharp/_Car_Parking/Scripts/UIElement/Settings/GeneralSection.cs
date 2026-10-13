using System.Globalization;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using Traffic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.AssetBundles;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Traffic;
using _Car_Parking.Scripts.UIElement.HUD;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class GeneralSection : SettingsSectionBase
	{
		private const string TotalLocalizeString = "Settingsmenuwindow.Total";

		private const string TitleRemoveUnused = "Settingsmenuwindow.TitleRemoveUnused";

		private const string TitleRemoveAll = "Settingsmenuwindow.TitleRemoveAll";

		private const string ContentRemoveUnused = "Settingsmenuwindow.ContentRemoveUnused";

		private const string ContentRemoveAll = "Settingsmenuwindow.ContentRemoveAll";

		[SerializeField]
		private Toggle ShowFpsToggle = default;

		[SerializeField]
		private Toggle DarkButtonsToggle = default;

		[SerializeField]
		private Toggle VibrateToggle = default;

		[SerializeField]
		private Toggle KphToggle = default;

		[SerializeField]
		private Toggle MphToggle = default;

		[SerializeField]
		private SliderValueHandler TrafficPeopleDensitySlider = default;

		[SerializeField]
		private SliderValueHandler TrafficCarDensitySlider = default;

		[SerializeField]
		private Button RemoveUnusedAssetsButton = default;

		[SerializeField]
		private Button RemoveAllAseetsButton = default;

		[SerializeField]
		private TMP_Text SizeAllDownloadedAssets = default;

		[SerializeField]
		private GameObject DownloadedFilesPanel = default;

		private Camera _camera;

		private OptionalDependency<Camera> _cameraDep;

		private FPSDisplay2 _fpsDisplay;

		private InputItemsProvider _inputItemsProvider;

		private OptionalDependency<HudColorChanger> _hudColorChangerDep;

		private RacingProvider _racingProvider;

		private DragRacingUI _dragRacingUI;

		private OptionalDependency<DragRacingUI> _dragRacingUIDep;

		private FreeDriveProvider _freeDriveProvider;

		private CarInputManager _carInputManager;

		private AssetBundleManager _assetBundleManager;

		private ManagerUI _managerUI;

		private HudColorChanger HudColorChanger
		{
			get
			{
				if (_racingProvider == null || !_racingProvider.HudColorChanger)
				{
					return _hudColorChangerDep?.Value;
				}
				return _racingProvider.HudColorChanger;
			}
		}

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider, CarInputManager carInputManager, AssetBundleManager assetBundleManager, ManagerUI managerUI, RacingProvider racingProvider, InputItemsProvider inputItemsProvider, OptionalDependency<Camera> currentCamera, OptionalDependency<HudColorChanger> hudColorChanger, OptionalDependency<DragRacingUI> dragRacingUI)
		{
			_freeDriveProvider = freeDriveProvider;
			_racingProvider = racingProvider;
			_inputItemsProvider = inputItemsProvider;
			_dragRacingUIDep = dragRacingUI;
			_hudColorChangerDep = hudColorChanger;
			_cameraDep = currentCamera;
			_carInputManager = carInputManager;
			_assetBundleManager = assetBundleManager;
			_managerUI = managerUI;
		}

		protected override void Awake()
		{
			base.Awake();
			_camera = _cameraDep?.Value;
			_dragRacingUI = _dragRacingUIDep?.Value;
		}

		private void Start()
		{
			Restore();
			if (!_camera)
			{
				_camera = Camera.main;
			}
			if ((bool)_camera)
			{
				_fpsDisplay = _camera.GetComponent<FPSDisplay2>();
			}
			VibrateToggle.gameObject.SetActive(value: false);
			DarkButtonsToggle.gameObject.SetActive(value: false);
			DownloadedFilesPanel.SetActive(value: false);
			ShowFpsToggle.onValueChanged.AddListener(ShowFps);
			KphToggle.onValueChanged.AddListener(SetKph);
			TrafficPeopleDensitySlider.OnReleased += OnPeopleTrafficDensityChanged;
			TrafficCarDensitySlider.OnReleased += base.OnTrafficCarDensityChanged;
			RemoveUnusedAssetsButton.onClick.AddListener(RemoveUnusedDownloadedAssets);
			RemoveAllAseetsButton.onClick.AddListener(RemoveAllDownloadedAssets);
		}

		private void RemoveUnusedDownloadedAssets()
		{
			_managerUI.ShowConfirmWindow(() =>
			{
				RemoveUnusedDownloadedAssetsAsync(this.GetCancellationTokenOnDestroy()).Forget();
			}, new LocalizedString("Settingsmenuwindow.TitleRemoveUnused"), new LocalizedString("Settingsmenuwindow.ContentRemoveUnused"), () =>
			{
				_managerUI.CloseWindow("ConfirmWindow");
			}).Forget();
		}

		private async UniTaskVoid RemoveUnusedDownloadedAssetsAsync(CancellationToken ct)
		{
			await _managerUI.ShowWaitingWindow(set: true);
			await _assetBundleManager.RemoveUnusedDownloadedAssets();
			if (ct.IsCancellationRequested)
			{
				await _managerUI.ShowWaitingWindow(set: false, backButtonState: true);
				return;
			}
			UpdateTotalSizeAssets();
			await _managerUI.ShowWaitingWindow(set: false, backButtonState: true);
		}

		private void RemoveAllDownloadedAssets()
		{
			_managerUI.ShowConfirmWindow(() =>
			{
				RemoveAllDownloadedAssetsAsync(this.GetCancellationTokenOnDestroy()).Forget();
			}, new LocalizedString("Settingsmenuwindow.TitleRemoveAll"), new LocalizedString("Settingsmenuwindow.ContentRemoveAll"), () =>
			{
				_managerUI.CloseWindow("ConfirmWindow");
			}).Forget();
		}

		private async UniTaskVoid RemoveAllDownloadedAssetsAsync(CancellationToken ct)
		{
			await _managerUI.ShowWaitingWindow(set: true);
			await _assetBundleManager.RemoveAllDownloadedAssets();
			if (ct.IsCancellationRequested)
			{
				await _managerUI.ShowWaitingWindow(set: false, backButtonState: true);
				return;
			}
			UpdateTotalSizeAssets();
			await _managerUI.ShowWaitingWindow(set: false, backButtonState: true);
		}

		private void OnPeopleTrafficDensityChanged(float value)
		{
			SteamTrafficLimits.MarkDensityChosenByUser();
			(from _ in Observable.EveryUpdate()
				where PeopleTraffic.Instance
				select _).Take(1).Subscribe((long _) =>
			{
				base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.traffic_people_density, PlayerPrefs.GetInt("peopleTrafficDensity").ToString(), value.ToString(CultureInfo.InvariantCulture));
				if ((bool)PeopleTraffic.Instance)
				{
					PeopleTraffic.Instance.ChangeTrafficDensity((int)value);
				}
			}).AddTo(this);
			PlayerPrefs.SetInt("peopleTrafficDensity", (int)value);
		}

		private void SetKph(bool isOn)
		{
			if (isOn)
			{
				base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.unit_speed, "mph", "kph");
			}
			else
			{
				base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.unit_speed, "kph", "mph");
			}
			PlayerPrefs.SetInt("kph", isOn ? 1 : 0);
			FlagsStorage.Kph = isOn;
			SpeedometerUI speedometerUI = _inputItemsProvider?.SpeedometerUI;
			if ((bool)speedometerUI)
			{
				speedometerUI.SetKph();
				speedometerUI.SetMileageLabel(isOn);
			}
			if ((bool)_freeDriveProvider.BannerManager)
			{
				_freeDriveProvider.BannerManager.UpdateAllBanners();
			}
			if ((bool)_dragRacingUI)
			{
				_dragRacingUI.SetOwnInfoCurrentMileageType();
			}
			GlobalEventSystem.InvokeSpeedUnitChanged(isOn);
		}

		private void MakeButtonsDark(bool isOn)
		{
			base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.dark_buttons, (!isOn).ToString(), isOn.ToString());
			PlayerPrefs.SetInt("DarkButtons", isOn ? 1 : 0);
			if ((bool)HudColorChanger)
			{
				HudColorChanger.SetDarkColor(isOn);
			}
		}

		private void ShowFps(bool isOn)
		{
			base.UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.show_fps, (!isOn).ToString(), isOn.ToString());
			PlayerPrefs.SetInt("ShowFps", isOn ? 1 : 0);
			if ((bool)_fpsDisplay)
			{
				_fpsDisplay.showFps = isOn;
			}
		}

		private void VibrateIsOnChangedCallback(bool isOn)
		{
			Utils.SetVibrateIsOn(isOn);
		}

		public override void Restore()
		{
			ShowFpsToggle.isOn = PlayerPrefs.GetInt("ShowFps", 0) != 0;
			VibrateToggle.SetIsOnWithoutNotify(Utils.VibrateIsOn);
			SteamRefreshTrafficSliders();
			DarkButtonsToggle.isOn = PlayerPrefs.GetInt("DarkButtons", 0) == 1;
			bool flag = PlayerPrefs.GetInt("kph", 1) == 1;
			KphToggle.isOn = flag;
			MphToggle.isOn = !flag;
		}

		private void UpdateTotalSizeAssets()
		{
			SizeAllDownloadedAssets.text = string.Format("{0}: ", new LocalizedString("Settingsmenuwindow.Total")) + $"{_assetBundleManager.GetSizeAllDownloadedAssets():F2} MB";
		}

		private void OnEnable()
		{
			UpdateTotalSizeAssets();
			DownloadedFilesPanel.SetActive(value: false);
			SteamRefreshTrafficSliders();
		}

		private void SteamRefreshTrafficSliders()
		{
			SteamTrafficLimits.EnsureDefaultsApplied();
			TrafficPeopleDensitySlider.CurrentSlider.maxValue = 100f;
			TrafficCarDensitySlider.CurrentSlider.maxValue = 100f;
			TrafficPeopleDensitySlider.SetValue(PlayerPrefs.GetInt("peopleTrafficDensity", 50));
			TrafficCarDensitySlider.SetValue(PlayerPrefs.GetInt("CarQuantity", 50));
		}

		private void OnDestroy()
		{
			ShowFpsToggle.onValueChanged.RemoveAllListeners();
			VibrateToggle.onValueChanged.RemoveAllListeners();
			DarkButtonsToggle.onValueChanged.RemoveAllListeners();
			KphToggle.onValueChanged.RemoveAllListeners();
			TrafficPeopleDensitySlider.OnReleased -= OnPeopleTrafficDensityChanged;
			TrafficCarDensitySlider.OnReleased -= base.OnTrafficCarDensityChanged;
			RemoveUnusedAssetsButton.onClick.RemoveAllListeners();
			RemoveAllAseetsButton.onClick.RemoveAllListeners();
		}
	}
}
