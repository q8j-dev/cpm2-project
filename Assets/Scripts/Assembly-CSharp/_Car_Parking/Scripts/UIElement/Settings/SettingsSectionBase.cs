using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gley.TrafficSystem;
using Gley.TrafficSystem.Internal;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Traffic;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public abstract class SettingsSectionBase : MonoBehaviour
	{
		private const string ScrollKeyPrefix = "SettingsSection.Scroll.";

		[SerializeField]
		protected Toggle SectionToggle;

		[SerializeField]
		protected ScrollRect SectionScroll;

		protected ManagerUI ManagerUI;

		private bool _isSelected;

		private bool _isScrollRestored;

		private bool _isTrafficChangedOnNotInitedStatus;

		protected FreeDriveControl FreeDriveControl;

		private OptionalDependency<ManagerUI> _managerUIDep;

		private OptionalDependency<FreeDriveControl> _freeDriveControlDep;

		[field: SerializeField]
		public SettingsWindow Settings { get; private set; }

		public bool IsAvailable
		{
			get
			{
				if ((bool)SectionToggle)
				{
					return SectionToggle.gameObject.activeInHierarchy;
				}
				return false;
			}
		}

		protected UserPreferenceAnalytic UserPreferenceAnalytic { get; private set; }

		private string ScrollKey => "SettingsSection.Scroll." + base.name;

		[Inject]
		private void ConstructBase(UserPreferenceAnalytic userPreferenceAnalytic, OptionalDependency<ManagerUI> managerUI, OptionalDependency<FreeDriveControl> freeDriveControl)
		{
			_freeDriveControlDep = freeDriveControl;
			_managerUIDep = managerUI;
			UserPreferenceAnalytic = userPreferenceAnalytic;
		}

		protected virtual void Awake()
		{
			FreeDriveControl = _freeDriveControlDep?.Value;
			ManagerUI = _managerUIDep?.Value;
			if (!SectionScroll)
			{
				SectionScroll = GetComponentInChildren<ScrollRect>(includeInactive: true);
			}
		}

		protected virtual void SelectSection(bool isOn)
		{
			base.gameObject.SetActive(isOn);
			if (!isOn)
			{
				if (_isSelected)
				{
					SaveScrollPosition();
				}
				_isSelected = false;
				DeselectSection();
			}
			else if (!_isSelected)
			{
				_isSelected = true;
				Settings.SetSection(this);
				RestoreScrollPosition();
			}
		}

		public void SaveScrollPosition()
		{
			if (_isScrollRestored)
			{
				ScrollPositionMemory.Save(SectionScroll, ScrollKey);
			}
		}

		private void RestoreScrollPosition()
		{
			if ((bool)SectionScroll)
			{
				RestoreScrollPositionAsync(this.GetCancellationTokenOnDestroy()).Forget();
			}
		}

		private async UniTaskVoid RestoreScrollPositionAsync(CancellationToken token)
		{
			await ScrollPositionMemory.RestoreAsync(SectionScroll, ScrollKey, token);
			_isScrollRestored = true;
		}

		public void SelectSectionManually(bool isOn)
		{
			SelectSection(isOn);
			SectionToggle.isOn = isOn;
		}

		protected virtual void DeselectSection()
		{
		}

		public virtual void Init()
		{
			SectionToggle.onValueChanged.AddListener(SelectSection);
			SelectSection(SectionToggle.isOn);
		}

		public virtual void SaveChanges()
		{
		}

		public virtual void ResetChanges()
		{
		}

		public virtual void Restore()
		{
		}

		public void OnTrafficCarDensityChanged(float value)
		{
			UserPreferenceAnalytic.LogUserPreference(UserPreferenceType.traffic_car_density, PlayerPrefs.GetInt("CarQuantity").ToString(), value.ToString(CultureInfo.InvariantCulture));
			SteamTrafficLimits.MarkDensityChosenByUser();
			OnTrafficCarDensityChanged(value, withMessage: true);
		}

		public void OnTrafficCarDensityChanged(float value, bool withMessage)
		{
			PlayerPrefs.SetInt("CarQuantity", (int)value);
			if (FlagsStorage.IsMultiFree)
			{
				if (withMessage)
				{
					ManagerUI.ShowNotificationWindow(new LocalizedString("SettingsSectionBase.Notrafficcarsonmultiplayer"), 3f).Forget();
				}
			}
			else
			{
				if (!ScenesHelper.IsFreeDriveSceneActive() || FlagsStorage.InGarageFree)
				{
					return;
				}
				value = PlayerPrefs.GetInt("CarQuantity", 50);
				int num = (int)value;
				bool flag = num > 0;
				if (TrafficManager.Exists && TrafficManager.Instance.Initialized)
				{
					API.SetTrafficDensity(num);
					TrafficManager.Instance.gameObject.SetActive(flag);
				}
				else if (!_isTrafficChangedOnNotInitedStatus & flag)
				{
					_isTrafficChangedOnNotInitedStatus = true;
					if ((bool)FreeDriveControl && (bool)FreeDriveControl.TrafficComponent)
					{
						FreeDriveControl.TrafficComponent.Init(Camera.main.transform).Forget();
					}
					if ((bool)V.Car && !V.Car.GetComponent<PlayerComponent>())
					{
						V.Car.AddComponent<PlayerComponent>();
					}
					else if ((bool)V.LastUsedCar && !V.LastUsedCar.GetComponent<PlayerComponent>())
					{
						V.LastUsedCar.AddComponent<PlayerComponent>();
					}
				}
				if ((bool)FreeDriveControl && (bool)FreeDriveControl.TrafficComponent)
				{
					FreeDriveControl.TrafficComponent.SetNpcTrafficBank(flag);
				}
				if ((bool)FreeDriveControl)
				{
					FreeDriveControl.SwitchLightControllers(isOn: false);
				}
			}
		}
	}
}
