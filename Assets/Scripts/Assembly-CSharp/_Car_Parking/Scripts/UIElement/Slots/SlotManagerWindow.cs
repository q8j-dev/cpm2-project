using System;
using System.Collections.Generic;
using CPM.Database;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.FreeDriverService;
using CPM.Scripts.Garage.GarageBuyer;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement.ScrollViews;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class SlotManagerWindow : WindowScript, IBackableWindow
	{
		private class CarSlotChangedData
		{
			public CarSlot CarSlot;

			public string NewCarFilePath;

			public CarSlotChangedData(CarSlot carSlot, string newCarFilePath)
			{
				CarSlot = carSlot;
				NewCarFilePath = newCarFilePath;
			}
		}

		private const int GarageAmount = 2;

		[SerializeField]
		private Button ButtonClose = default;

		[SerializeField]
		private SlotsViewColumn CurrentScrollViewColumn = default;

		[SerializeField]
		private SlotsViewColumn OtherScrollViewColumn = default;

		[SerializeField]
		private SlotManagerUIButtonBase[] Buttons = default;

		[SerializeField]
		private GameObject BlockInputGO = default;

		private IGarageSelectorService _garageSelectorService;

		private IGarageBuyService _garageBuyService;

		private ISlotService _slotService;

		private SlotManagerSelectController _slotManagerSelectController;

		private IDatabaseCarsService _carsService;

		private IGarageEntity _currentGarage;

		private IFreeDriverService _freeDriverService;

		private PlayerDatas.CarLoadingUtility _carLoadingUtility;

		private DataBase _dataBase;

		private AnalyticEventFactory _analyticFactory;

		private bool _isDestroyed;

		public SlotsViewColumn ScrollViewColumnCurrent => CurrentScrollViewColumn;

		public SlotsViewColumn ScrollViewColumnOther => OtherScrollViewColumn;

		private int CurrentGarageIndex => _garageSelectorService.CurrentGarageIndex;

		private int OtherGarageIndex => (CurrentGarageIndex + 1) % 2;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		private bool IsBlockInput
		{
			get
			{
				if ((bool)BlockInputGO)
				{
					return BlockInputGO.activeSelf;
				}
				return false;
			}
			set
			{
				if ((bool)BlockInputGO)
				{
					BlockInputGO.SetActive(value);
				}
			}
		}

		[Inject]
		public void Construct(IGarageSelectorService garageSelectorService, IGarageBuyService garageBuyService, ISlotService slotService, IDatabaseCarsService carsService, IGarageEntity garageEntity, IFreeDriverService freeDriverService, PlayerDatas.CarLoadingUtility loadingUtility, DataBase dataBase, AnalyticEventFactory analyticFactory)
		{
			_garageSelectorService = garageSelectorService;
			_garageBuyService = garageBuyService;
			_slotService = slotService;
			_carsService = carsService;
			_currentGarage = garageEntity;
			_freeDriverService = freeDriverService;
			_carLoadingUtility = loadingUtility;
			_dataBase = dataBase;
			_analyticFactory = analyticFactory;
		}

		protected override void Start()
		{
			IsBlockInput = false;
			CurrentScrollViewColumn.Init(CurrentGarageIndex, OpenGarageBuyWindow);
			OtherScrollViewColumn.Init(OtherGarageIndex, OpenGarageBuyWindow);
			_slotManagerSelectController = new SlotManagerSelectController(new ScrollViewBase[2] { CurrentScrollViewColumn.ScrollViewSlots, OtherScrollViewColumn.ScrollViewSlots });
			for (int i = 0; i < Buttons.Length; i++)
			{
				if ((bool)Buttons[i])
				{
					Buttons[i].Init(_slotManagerSelectController);
				}
			}
			InitBackButton();
			base.Start();
		}

		public void Back()
		{
			ButtonCloseClickCallback();
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		protected override void OnDestroy()
		{
			_slotManagerSelectController?.Dispose();
			_isDestroyed = true;
			base.OnDestroy();
		}

		protected override void OnEnable()
		{
			ButtonClose.onClick.AddListener(ButtonCloseClickCallback);
			ScrollViewSlots scrollViewSlots = CurrentScrollViewColumn.ScrollViewSlots;
			scrollViewSlots.OnStateChangedEvent = (Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>)Delegate.Combine(scrollViewSlots.OnStateChangedEvent, new Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>(ScrollViewStateChangedCallback));
			ScrollViewSlots scrollViewSlots2 = OtherScrollViewColumn.ScrollViewSlots;
			scrollViewSlots2.OnStateChangedEvent = (Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>)Delegate.Combine(scrollViewSlots2.OnStateChangedEvent, new Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>(ScrollViewStateChangedCallback));
			base.OnEnable();
		}

		private void OnDisable()
		{
			ButtonClose.onClick.RemoveListener(ButtonCloseClickCallback);
			ScrollViewSlots scrollViewSlots = CurrentScrollViewColumn.ScrollViewSlots;
			scrollViewSlots.OnStateChangedEvent = (Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>)Delegate.Remove(scrollViewSlots.OnStateChangedEvent, new Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>(ScrollViewStateChangedCallback));
			ScrollViewSlots scrollViewSlots2 = OtherScrollViewColumn.ScrollViewSlots;
			scrollViewSlots2.OnStateChangedEvent = (Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>)Delegate.Remove(scrollViewSlots2.OnStateChangedEvent, new Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>(ScrollViewStateChangedCallback));
		}

		private void ButtonCloseClickCallback()
		{
			CloseAsync(() =>
			{
				GameOverlayWindow.AdditionalData value = new GameOverlayWindow.AdditionalData
				{
					OpenMode = GameOverlayWindow.OpenMode.OnlyBalance
				};
				_managerUI.ShowUserBalanceWindow(null, JsonConvert.SerializeObject(value));
				_managerUI.ShowGarageWindow();
			}).Forget();
		}

		private void ScrollViewStateChangedCallback(ScrollViewBase scrollView, ScrollViewBase.ScrollViewStateChangeData changeData)
		{
			if (changeData.OldState != null && changeData.OldState.State == ScrollViewState.StateType.DragItem && changeData.NewState != null && changeData.NewState.State == ScrollViewState.StateType.Idle)
			{
				UpdateOccupiedFromUI(CurrentScrollViewColumn);
				UpdateOccupiedFromUI(OtherScrollViewColumn);
			}
			if (changeData.NewState != null && changeData.NewState.State != ScrollViewState.StateType.None && changeData.NewState.State != ScrollViewState.StateType.Idle && changeData.NewState.State != ScrollViewState.StateType.Scroll)
			{
				ScrollViewSlots otherScrollViewSlots = GetOtherScrollViewSlots(scrollView as ScrollViewSlots);
				if ((bool)otherScrollViewSlots)
				{
					otherScrollViewSlots.SetState(ScrollViewState.StateType.Idle);
				}
			}
		}

		private void UpdateOccupiedFromUI(SlotsViewColumn column)
		{
			column.UpdateOccupiedFromUI();
		}

		private ScrollViewSlots GetOtherScrollViewSlots(ScrollViewSlots scrollViewSlots)
		{
			if (CurrentScrollViewColumn.ScrollViewSlots == scrollViewSlots)
			{
				return OtherScrollViewColumn.ScrollViewSlots;
			}
			return CurrentScrollViewColumn.ScrollViewSlots;
		}

		private async UniTask CloseAsync(Action finallyCallback = null)
		{
			_ = 2;
			try
			{
				IsBlockInput = true;
				if (HaveChanges(CurrentGarageIndex, CurrentScrollViewColumn) || HaveRemove())
				{
					_managerUI.ShowWaitingWindow(set: true).Forget();
				}
				CurrentScrollViewColumn.ScrollViewSlots.SetState(ScrollViewState.StateType.Idle);
				OtherScrollViewColumn.ScrollViewSlots.SetState(ScrollViewState.StateType.Idle);
				await UniTask.DelayFrame(3);
				bool hadRemovals = HaveRemove();
				await RemoveCars();
				CurrentCarsData currentCarsData = new CurrentCarsData(_slotService.GetSlotCollection(CurrentGarageIndex), _currentGarage);
				ResetCarPositionsInChangedSlots(currentCarsData);
				bool num = hadRemovals || HaveChanges(CurrentGarageIndex, CurrentScrollViewColumn) || HaveChanges(OtherGarageIndex, OtherScrollViewColumn);
				SaveChanges(CurrentGarageIndex, CurrentScrollViewColumn);
				SaveChanges(OtherGarageIndex, OtherScrollViewColumn);
				if (num)
				{
					_dataBase.SendSlotsCollection().Forget();
				}
				await UpdateCarsSlots(currentCarsData);
			}
			catch (Exception)
			{
			}
			finally
			{
				finallyCallback?.Invoke();
				_managerUI.CloseWaitingWindow();
			}
		}

		private void OpenGarageBuyWindow(int garageIndex)
		{
			OpenGarageBuyWindowAsync(garageIndex).Forget();
		}

		private async UniTask OpenGarageBuyWindowAsync(int garageIndex)
		{
			await CloseAsync();
			if (!_isDestroyed)
			{
				_managerUI.ShowConfirmGarageBuyWindow(new ConfirmGarageBuyWindow.AdditionalData
				{
					GarageIndex = garageIndex,
					PreviousWindow = WindowScriptType.SlotManagerWindow.ToString()
				});
			}
		}

		private void ResetCarPositionsInChangedSlots(CurrentCarsData currentCarsData)
		{
			List<CarSlotChangedData> list = new List<CarSlotChangedData>();
			list.AddRange(GetCarSlotChangedData(CurrentGarageIndex, CurrentScrollViewColumn));
			list.AddRange(GetCarSlotChangedData(OtherGarageIndex, OtherScrollViewColumn));
			foreach (CarSlotChangedData item in list)
			{
				VehicleData car = currentCarsData.GetCar(item.CarSlot.CarFilePath);
				if ((bool)car)
				{
					_currentGarage.ResetSavedCarPosition(car);
				}
			}
		}

		private void SaveChanges(int garageIndex, SlotsViewColumn slotsViewColumn)
		{
			SlotCollection slotCollection = _slotService.GetSlotCollection(garageIndex);
			List<CarSlotChangedData> carSlotChangedData = GetCarSlotChangedData(garageIndex, slotsViewColumn);
			for (int i = 0; i < carSlotChangedData.Count; i++)
			{
				carSlotChangedData[i].CarSlot.CarFilePath = carSlotChangedData[i].NewCarFilePath;
			}
			if (carSlotChangedData.Count > 0)
			{
				slotCollection.SaveSlots();
				for (int j = 0; j < carSlotChangedData.Count; j++)
				{
					_slotService.NotifySlotChanged(carSlotChangedData[j].CarSlot.ID);
				}
			}
		}

		private bool HaveChanges(int garageIndex, SlotsViewColumn slotsViewColumn)
		{
			return GetCarSlotChangedData(garageIndex, slotsViewColumn).Count > 0;
		}

		private List<CarSlotChangedData> GetCarSlotChangedData(int garageIndex, SlotsViewColumn slotsViewColumn)
		{
			List<CarSlotChangedData> list = new List<CarSlotChangedData>();
			if (!_garageBuyService.GetGarageStatus(garageIndex))
			{
				return list;
			}
			SlotCollection slotCollection = _slotService.GetSlotCollection(garageIndex);
			int num = 0;
			SlotItemContent[] componentsInChildren = slotsViewColumn.GetComponentsInChildren<SlotItemContent>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].IsActive)
				{
					if (slotCollection[num] != null && slotCollection[num].CarFilePath != componentsInChildren[i].CarInstanceId)
					{
						list.Add(new CarSlotChangedData(slotCollection[num], componentsInChildren[i].CarInstanceId));
					}
					num++;
				}
			}
			return list;
		}

		private bool HaveRemove()
		{
			return GetRemovedSlots().Count > 0;
		}

		private async UniTask RemoveCars()
		{
			List<CarSlot> removeSlots = GetRemovedSlots();
			for (int i = 0; i < removeSlots.Count; i++)
			{
				await _carsService.DeleteCarFull(removeSlots[i]);
			}
		}

		private List<CarSlot> GetRemovedSlots()
		{
			List<CarSlot> list = new List<CarSlot>();
			List<SlotItemContent> list2 = new List<SlotItemContent>();
			list2.AddRange(CurrentScrollViewColumn.GetComponentsInChildren<SlotItemContent>());
			list2.AddRange(OtherScrollViewColumn.GetComponentsInChildren<SlotItemContent>());
			for (int i = 0; i < 2; i++)
			{
				int garageIndex = ((i == 0) ? CurrentGarageIndex : OtherGarageIndex);
				if (!_garageBuyService.GetGarageStatus(garageIndex))
				{
					continue;
				}
				SlotCollection slotCollection = _slotService.GetSlotCollection(garageIndex);
				for (int j = 0; j < (int)slotCollection.Count; j++)
				{
					CarSlot carSlot = slotCollection[j];
					if (carSlot == null || carSlot.IsEmpty)
					{
						continue;
					}
					bool flag = false;
					foreach (SlotItemContent item in list2)
					{
						if (item.CarInstanceId == carSlot.CarFilePath)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(carSlot);
					}
				}
			}
			return list;
		}

		private async UniTask UpdateCarsSlots(CurrentCarsData currentCars)
		{
			SlotCollection slotCollection = _slotService.GetSlotCollection(CurrentGarageIndex);
			if (slotCollection.IsAllSlotsEmpty())
			{
				V.CoreGameManager.CarOutInControl.OutCar(V.DriverPerson, V.Car);
				_ = _freeDriverService.SetupFreeDriver();
			}
			_currentGarage.Clear();
			List<string> allCarInstanceIds = currentCars.GetAllCarInstanceIds();
			for (int i = 0; i < allCarInstanceIds.Count; i++)
			{
				string text = allCarInstanceIds[i];
				VehicleData car = currentCars.GetCar(text);
				if (!car)
				{
					continue;
				}
				CarSlot slot = slotCollection.GetSlot(text);
				if (slot == null)
				{
					if (V.Car == car.gameObject)
					{
						V.Car = null;
					}
					_carLoadingUtility.UnloadCar(car.gameObject);
					currentCars.RemoveCar(text);
				}
				else
				{
					car.slotId = slot.ID;
				}
			}
			allCarInstanceIds = currentCars.GetAllCarInstanceIds();
			for (int j = 0; j < allCarInstanceIds.Count; j++)
			{
				string carInstanceId = allCarInstanceIds[j];
				VehicleData car2 = currentCars.GetCar(carInstanceId);
				_currentGarage.PlaceCarOnSpot(car2.slotId, car2, turnOff: true, setIsOccupied: true);
			}
			for (int k = 0; k < (int)slotCollection.Count; k++)
			{
				CarSlot carSlot = slotCollection[k];
				if (carSlot != null && !carSlot.IsEmpty && !currentCars.GetCar(carSlot.CarFilePath))
				{
					await _currentGarage.LoadCarBySpotId(carSlot.ID, focusOnCar: false);
				}
			}
			if (!V.Car)
			{
				for (int l = 0; l < (int)slotCollection.Count; l++)
				{
					CarSlot carSlot2 = slotCollection[l];
					if (carSlot2 != null && !carSlot2.IsEmpty)
					{
						VehicleData carBySpot = _currentGarage.GetCarBySpot(carSlot2.ID);
						if ((bool)carBySpot)
						{
							await V.CoreGameManager.CarOutInControl.InCar(V.DriverPerson, carBySpot.gameObject);
							break;
						}
					}
				}
			}
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component))
			{
				slotCollection.LastSlotId = component.slotId;
				_currentGarage.FocusOnCarOnSpot(component.slotId, withCameraReset: false);
			}
		}
	}
}
