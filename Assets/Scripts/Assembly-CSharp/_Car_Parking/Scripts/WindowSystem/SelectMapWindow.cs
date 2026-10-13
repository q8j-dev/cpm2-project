using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class SelectMapWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private SpawnPlaceItemUI[] SpawnPlaces;

		[SerializeField]
		private Button StartButton;

		[SerializeField]
		private Button BackButton;

		private MultiPlayerMenu _multiPlayerMenu;

		private ITimerTuningService _timerTuningService;

		private UnityAction<bool> _onPlaceToggleChanged;

		public Action<SpawnPlaceItemUI> OnSingleplayerStarted { get; set; }

		[Inject]
		public void Construct(ITimerTuningService timerTuningService, MultiPlayerMenu multiPlayerMenu)
		{
			_multiPlayerMenu = multiPlayerMenu;
			_timerTuningService = timerTuningService;
		}

		protected override void Start()
		{
			InitBackButton();
			base.Start();
			_managerUI.CloseWindow("UserBalanceWindow");
			StartButton.onClick.AddListener(StartSinglePlayer);
			BackButton.onClick.AddListener(Back);
			SubscribePlaceToggles();
		}

		private void SubscribePlaceToggles()
		{
			_onPlaceToggleChanged = OnPlaceToggleChanged;
			SpawnPlaceItemUI[] spawnPlaces = SpawnPlaces;
			foreach (SpawnPlaceItemUI spawnPlaceItemUI in spawnPlaces)
			{
				if ((bool)spawnPlaceItemUI && spawnPlaceItemUI.TryGetComponent<Toggle>(out var component))
				{
					component.onValueChanged.AddListener(_onPlaceToggleChanged);
				}
			}
			UpdateStartButtonNavigation();
		}

		private void UnsubscribePlaceToggles()
		{
			if (_onPlaceToggleChanged == null)
			{
				return;
			}
			SpawnPlaceItemUI[] spawnPlaces = SpawnPlaces;
			foreach (SpawnPlaceItemUI spawnPlaceItemUI in spawnPlaces)
			{
				if ((bool)spawnPlaceItemUI && spawnPlaceItemUI.TryGetComponent<Toggle>(out var component))
				{
					component.onValueChanged.RemoveListener(_onPlaceToggleChanged);
				}
			}
			_onPlaceToggleChanged = null;
		}

		private void OnPlaceToggleChanged(bool isOn)
		{
			if (isOn)
			{
				UpdateStartButtonNavigation();
			}
		}

		private void UpdateStartButtonNavigation()
		{
			SpawnPlaceItemUI spawnPlaceItemUI = SpawnPlaces.FirstOrDefault((SpawnPlaceItemUI place) => (bool)place && place.IsChosen);
			if (!spawnPlaceItemUI || !spawnPlaceItemUI.TryGetComponent<Selectable>(out var component))
			{
				return;
			}
			float x = spawnPlaceItemUI.transform.position.x;
			Selectable selectOnLeft = null;
			Selectable selectOnRight = null;
			float num = float.NegativeInfinity;
			float num2 = float.PositiveInfinity;
			SpawnPlaceItemUI[] spawnPlaces = SpawnPlaces;
			foreach (SpawnPlaceItemUI spawnPlaceItemUI2 in spawnPlaces)
			{
				if ((bool)spawnPlaceItemUI2 && !(spawnPlaceItemUI2 == spawnPlaceItemUI) && spawnPlaceItemUI2.gameObject.activeInHierarchy && spawnPlaceItemUI2.TryGetComponent<Selectable>(out var component2) && component2.IsInteractable())
				{
					float x2 = spawnPlaceItemUI2.transform.position.x;
					if (x2 < x && x2 > num)
					{
						num = x2;
						selectOnLeft = component2;
					}
					else if (x2 > x && x2 < num2)
					{
						num2 = x2;
						selectOnRight = component2;
					}
				}
			}
			Navigation navigation = StartButton.navigation;
			if (navigation.mode != Navigation.Mode.Explicit)
			{
				navigation.selectOnDown = StartButton.FindSelectableOnDown();
			}
			navigation.mode = Navigation.Mode.Explicit;
			navigation.selectOnUp = component;
			navigation.selectOnLeft = selectOnLeft;
			navigation.selectOnRight = selectOnRight;
			StartButton.navigation = navigation;
		}

		protected override void OnDestroy()
		{
			UnsubscribePlaceToggles();
			base.OnDestroy();
		}

		private void StartSinglePlayer()
		{
			SpawnPlaceItemUI spawnPlaceItemUI = SpawnPlaces.FirstOrDefault((SpawnPlaceItemUI place) => place.IsChosen);
			if (!spawnPlaceItemUI)
			{
				return;
			}
			V.PlaceToSpawn = spawnPlaceItemUI.SpawnPlace;
			GameObject gameObject = V.LastUsedCar;
			if (!gameObject)
			{
				gameObject = V.Car;
			}
			if ((bool)gameObject && gameObject.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				if (component.CarEntity.GetValue().IsBroken)
				{
					_managerUI.ShowNotificationWindow(new LocalizedString("SelectMapWindow.CarIsBrokenPleaseRepair"), 3f);
					return;
				}
				if (_timerTuningService.IsCarBlocked(V.SlotID))
				{
					_managerUI.ShowNotificationWindow(new LocalizedString("SelectMapWindow.Yourcarcurrentlyisinservice"), 3f);
					return;
				}
			}
			_multiPlayerMenu.StartSingleFreeDrive();
			PlayerPrefs.SetInt("NewPlaceIconSingle" + spawnPlaceItemUI.SpawnPlace, 1);
		}

		protected override void OnEnable()
		{
			BackButton.gameObject.SetActive(value: false);
			base.OnEnable();
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			_managerUI.ShowGameModeWindow();
			Close();
		}
	}
}
