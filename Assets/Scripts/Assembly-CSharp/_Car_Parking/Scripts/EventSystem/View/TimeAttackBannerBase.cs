using System;
using System.Collections.Generic;
using System.Linq;
using CPM.RemoteConfig;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Car.CarEligibilityService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class TimeAttackBannerBase : EventView
	{
		[SerializeField]
		private V.placeToSpawn PlaceToSpawn;

		[SerializeField]
		protected List<TextMeshProUGUI> Points;

		[SerializeField]
		private TextMeshProUGUI QualificationCountText;

		[SerializeField]
		private Button StartButton;

		[SerializeField]
		private CarSelectorPanel CarSelectorPanel;

		[SerializeField]
		private TimeAttackBannerHelper TimeAttackBannerHelper;

		private VehicleClass _currentVehicleClass;

		private Camera _mainCamera;

		private EventStorage _eventStorage;

		private ManagerUI _managerUI;

		private ICarEligibilityService _carEligibilityService;

		protected EventRestrictionService _eventRestrictionService { get; private set; }

		[UnityEngine.Scripting.Preserve]
		[Inject]
		private void Construct(Camera mainCamera, EventStorage eventStorage, EventRestrictionService eventRestrictionService, ManagerUI managerUI, IRemoteConfigManager remoteConfigManager, ICarEligibilityService carEligibilityService)
		{
			_mainCamera = mainCamera;
			_eventStorage = eventStorage;
			_eventRestrictionService = eventRestrictionService;
			_managerUI = managerUI;
			_carEligibilityService = carEligibilityService;
		}

		public override void Init(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			base.Init(@event);
			UpdateTimer();
			UIInitialize();
			base.ClaimedPanel.SetActive(value: false);
			SetPanelsActive();
			SetupBanner().Forget();
		}

		protected override void SendFirstShowEvent()
		{
			List<VehicleClass> vehicleClasses = _eventRestrictionService.GetVehicleClasses(base.Event.Type);
			string alowedCarClases = ((vehicleClasses != null) ? string.Join(",", vehicleClasses) : string.Empty);
			_eventsAnalytic.LogEventFirstShow(base.Event, alowedCarClases);
		}

		private async UniTaskVoid Start()
		{
			if (!V.Car)
			{
				SetCarSelectPanelsState(state: false);
			}
			try
			{
				if (!ScenesHelper.IsFreeDriveSceneActive())
				{
					await UniTask.WaitUntil(() => (int)V.CarId != -1);
					await UniTask.WaitUntil(() => V.Car);
					if ((bool)this && (bool)CarSelectorPanel)
					{
						SetCarSelectPanelsState(base.Event.Status == EventStatus.InProgress);
						_currentVehicleClass = CarSelectorPanel.ParseCarValues();
					}
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		protected void SetCarSelectPanelsState(bool state)
		{
			CarSelectorPanel.gameObject.SetActive(state);
			StartButton.interactable = state;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			CarSelectorPanel.SelectClicked += ShowSelectCarWindow;
			StartButton.onClick.AddListener(OnStart);
		}

		private void ShowSelectCarWindow()
		{
			TimeAttackBannerHelper.ShowSelectCarWindow(base.Event.Type);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			CarSelectorPanel.SelectClicked -= ShowSelectCarWindow;
			StartButton.onClick.RemoveListener(OnStart);
		}

		protected override void SetupClaimedPanel()
		{
			base.SetupClaimedPanel();
			if ((bool)StartButton && (bool)CarSelectorPanel)
			{
				CanvasGroup component = StartButton.GetComponent<CanvasGroup>();
				CanvasGroup component2 = CarSelectorPanel.GetComponent<CanvasGroup>();
				if (component != null)
				{
					component.alpha = 0.4f;
					component.interactable = false;
				}
				if (component2 != null)
				{
					component2.alpha = 0.4f;
					component2.interactable = false;
				}
			}
		}

		public void ShowStartButton(bool show)
		{
			StartButton.gameObject.SetActive(show);
		}

		protected virtual void UIInitialize()
		{
			if ((bool)QualificationCountText)
			{
				QualificationCountText.text = $"{base.Event.Progress}/{base.Event.MaxProgress}";
			}
			EventConfig eventConfig = _eventStorage.EventsConfigs.First((EventConfig x) => x.Type == base.Event.Type);
			for (int num = 0; num < Points.Count; num++)
			{
				Points[num].text = new LocalizedString(eventConfig.PointsKeys[num]);
			}
		}

		protected void SetPanelsActive()
		{
			if (base.Event.Status == EventStatus.Finished)
			{
				base.ClaimButtonAnimationParent.gameObject.SetActive(value: true);
				SetActiveForPanels(active: false);
			}
			else
			{
				base.ClaimButtonAnimationParent.gameObject.SetActive(value: false);
				SetActiveForPanels(active: true);
			}
			if (ScenesHelper.IsFreeDriveSceneActive())
			{
				SetActiveForPanels(active: false);
			}
		}

		private void SetActiveForPanels(bool active)
		{
			StartButton.SetActive(active);
			CarSelectorPanel.SetActive(active);
		}

		protected void OnStart()
		{
			StartButton.interactable = false;
			if (!_eventRestrictionService.IsVehicleAllowed(base.Event.Type, _currentVehicleClass))
			{
				V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("EventViewBigBanner.SelectClass"), 2f).Forget();
			}
			else if (!V.CoreGameManager.HasFirstCar())
			{
				V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("Mainmenuwindow.YouNeedCar"), 2f).Forget();
				StartButton.interactable = true;
			}
			else if (!_carEligibilityService.CanUseCar())
			{
				StartButton.interactable = true;
			}
			else
			{
				TimeAttackBannerHelper.OnStart(PlaceToSpawn);
			}
		}
	}
}
