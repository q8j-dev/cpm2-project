using System.Collections.Generic;
using System.Linq;
using CPM.Database;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using Unity.Mathematics;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.DataStorageService.Homes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ApartmentWindow : PropertyWindowBase<HomesConfig, HomesConfig.HomeDataItem>
	{
		[SerializeField]
		private ApartmentPlaceUIItem[] spawnPlaceItems = default;

		private List<PlaceUIItem<HomesConfig, HomesConfig.HomeDataItem>> _cachedPlaceItems;

		private ICarInfoService _carInfoService;

		private AllCarsInfo _allCarsInfo;

		private CoreGameManager _coreGameManager;

		private ITimerTuningService _tuningService;

		private IGameFactManager _gameFactManager;

		private InputMapSetEvent _inputMapSetEvent;

		protected override List<PlaceUIItem<HomesConfig, HomesConfig.HomeDataItem>> PlaceUIItems
		{
			get
			{
				if (_cachedPlaceItems == null)
				{
					_cachedPlaceItems = spawnPlaceItems.Cast<PlaceUIItem<HomesConfig, HomesConfig.HomeDataItem>>().ToList();
				}
				return _cachedPlaceItems;
			}
		}

		private ApartmentPlaceUIItem CurrentApartment => spawnPlaceItems[base.SelectedItem];

		[Inject]
		public void Construct(AllCarsInfo allCarsInfo, ICarInfoService carInfoService, CoreGameManager coreGameManager, InputMapSetEvent inputMapSetEvent, ITimerTuningService tuningService, IGameFactManager gameFactManager)
		{
			_carInfoService = carInfoService;
			_allCarsInfo = allCarsInfo;
			_coreGameManager = coreGameManager;
			_inputMapSetEvent = inputMapSetEvent;
			_tuningService = tuningService;
			_gameFactManager = gameFactManager;
		}

		protected override void Init()
		{
			base.Init();
			SelectButton.interactable = true;
			SetStateCheck();
			UpdateButton(animate: false);
		}

		protected override void OnSelectItem()
		{
			base.OnSelectItem();
			StartGame();
		}

		protected override void OnBuyItem()
		{
			V.CoreGameManager.ManagerUI.ShowApartmentGarageBuyWindow(base.SelectedItem);
		}

		protected override void UpdateButton(bool animate = true)
		{
			if (base.SelectedItem == -1)
			{
				BuyButton.SetActive(active: false);
				SelectButton.SetActive(active: false);
			}
			else
			{
				bool state = ObscuredPrefs.GetInt("homeOpen" + base.SelectedItem) == 1;
				SetButtonsVisibility(state, animate);
			}
		}

		private void StartGame(bool beforeBuying = false)
		{
			if (!_gameFactManager.IsTrue(GameFact.Carless) && !_coreGameManager.HasFirstCar())
			{
				if (!beforeBuying)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("Selectapartmentwindow.YouNeedCar"), 3f);
				}
				return;
			}
			if ((bool)V.Car)
			{
				_tuningService.IsCarBlocked(V.SlotID).Get(out var value);
				if (value)
				{
					if (!beforeBuying)
					{
						V.CoreGameManager.ManagerUI.ShowCarBlockNotification(NotificationMessages.CarInService, 3f, NotificationType.Error);
					}
					return;
				}
			}
			CarInfo fromSlot = _carInfoService.GetFromSlot(V.SlotID);
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				component.CheckerEngineCheating(out var res);
				if (math.all(res == Utils.NotValid))
				{
					V.CoreGameManager.ManagerUI.ShowCarBlockNotification(NotificationMessages.CheatEngine, 3f, NotificationType.Error);
					return;
				}
				component.CheckerPartsInEngine(out var partsLine, out var isIncompatible);
				if (math.all(isIncompatible == Utils.NotValid))
				{
					V.CoreGameManager.ManagerUI.ShowCarBlockNotification(NotificationMessages.IncompatibleParts, 3f, NotificationType.Error, partsLine);
					return;
				}
				_allCarsInfo.ResolveCarMeta(fromSlot.CarID, out var result);
				if (math.all(result == Utils.NotValid))
				{
					V.CoreGameManager.ManagerUI.ShowCarBlockNotification(NotificationMessages.CantDriveThisCar, 3f, NotificationType.Error);
					return;
				}
			}
			if (ObscuredPrefs.GetInt("homeOpen" + CurrentApartment.DataItem.Index) == 1)
			{
				V.PlaceToSpawn = CurrentApartment.DataItem.PlaceToSpawn;
				FlagsStorage.StartInHome = true;
				ObscuredPrefs.SetInt("homeSelected", CurrentApartment.DataItem.Index);
				V.CoreGameManager.menuControl.LoadFreeDrive();
				_inputMapSetEvent.TrySetGroup(InputGroupsEnum.CarFree, setRoot: true);
				_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Person, setRoot: true);
			}
		}

		private void SetStateCheck()
		{
			for (int i = 0; i < spawnPlaceItems.Length; i++)
			{
				int num = i;
				bool state = ObscuredPrefs.GetInt("homeOpen" + num) == 0;
				spawnPlaceItems[num].ChangeActiveState(state);
			}
		}
	}
}
