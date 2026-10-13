using CPM.Scripts.Garage;
using CPMEngine;
using DanielLochner.Assets.SimpleScrollSnap;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP_Registry;

public class CircuitCarSelectWindow : WindowScript, IBackableWindow
{
	[SerializeField]
	private Button StartButton = default;

	[SerializeField]
	private Button BackButton = default;

	[SerializeField]
	private Transform ContentPanel = default;

	[SerializeField]
	private TextMeshProUGUI TransmissionText = default;

	[SerializeField]
	private TextMeshProUGUI WeightText = default;

	[SerializeField]
	private TextMeshProUGUI ClassText = default;

	[SerializeField]
	private SimpleScrollSnap SimpleScrollSnap = default;

	[SerializeField]
	private CircuitSlotCollectionUIHandler SlotCollectionUI = default;

	[SerializeField]
	private ScrollSnapSoundComponent ScrollSnapSound = default;

	private GarageController _garageController;

	private IEventService _eventService;

	private IGarageEntity _currentGarage;

	private EventRestrictionService _eventRestrictionService;

	private _Car_Parking.Scripts.EventSystem.EventType _previewEventType;

	private _Car_Parking.Scripts.EventSystem.Data.Event _currentEvent;

	private bool _isEventCarSelect;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Garage;

	[Inject]
	public void Construct(GarageController garageController, IEventService eventService, IGarageEntity garageEntity, EventRestrictionService eventRestrictionService)
	{
		_garageController = garageController;
		_eventService = eventService;
		_currentGarage = garageEntity;
		_eventRestrictionService = eventRestrictionService;
	}

	public override void SetAdditionalData(string data)
	{
		_isEventCarSelect = true;
		_previewEventType = JsonConvert.DeserializeObject<_Car_Parking.Scripts.EventSystem.EventType>(data);
		_currentEvent = _eventService.GetEventByName(_previewEventType);
		SlotCollectionUI.SetEvent(_currentEvent);
		BackButton.SetActive(active: false);
		base.SetAdditionalData(data);
	}

	protected override void Start()
	{
		base.Start();
		InitBackButton();
		BackButton.onClick.AddListener(Back);
		StartButton.onClick.AddListener(SelectCar);
		V.OnCarEntered += OnCarChanged;
		ParseCarInfo(V.Car.GetComponent<VehicleData>());
		SnapTo();
		SimpleScrollSnap.OnPanelSelected.AddListener(FocusOnCar);
		ScrollSnapSound.Init(SimpleScrollSnap.ScrollRect.content.childCount);
		SimpleScrollSnap.ScrollRect.onValueChanged.AddListener(ScrollSnapSound.OnScrollValueChanged);
		_managerUI.SetBackground?.Invoke(obj: false);
		_managerUI.OnAchievmentsButtonActivated?.Invoke(obj: false);
		_managerUI.OnSettingsButtonActivated?.Invoke(obj: false);
		_managerUI.OnActiveAllBalanceWindow?.Invoke(obj: false);
	}

	private void FocusOnCar(int selected)
	{
		int slotIdByElementIndex = SlotCollectionUI.GetSlotIdByElementIndex(SimpleScrollSnap.CenteredPanel);
		VehicleData carBySpot = _currentGarage.GetCarBySpot(slotIdByElementIndex);
		if ((bool)carBySpot && _currentEvent != null)
		{
			VehicleClass classVehicle = Utils.GetClassVehicle(carBySpot.GetVehicleEntityMono.CarEntity.GetValue().Points, carBySpot.CarId);
			if (!_eventRestrictionService.IsVehicleAllowed(_currentEvent.Type, classVehicle))
			{
				return;
			}
		}
		if (!_garageController.IsSwitchingCar)
		{
			if (V.LastUsedCar.TryGetComponent<VehicleData>(out var component))
			{
				_garageController.DisableCarPhysicsDelayed(component, 2f);
				ParseCarInfo(component);
			}
			_garageController.ResetCarToSavedPosition();
			GarageMenuWindow.SelectedSlotId = slotIdByElementIndex;
			_garageController.FocusOnCarOnSlot(slotIdByElementIndex).Forget();
			SlotCollectionUI.SelectElementById(SimpleScrollSnap.CenteredPanel);
		}
	}

	private void SelectCar()
	{
		if (!_isEventCarSelect)
		{
			_managerUI.SetCarSelect(active: true);
			_managerUI.ShowPrevWindow(fade: false);
		}
		else
		{
			_managerUI.ShowEventViewWindow(_previewEventType);
		}
		Close();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		BackButton.SetActive(active: false);
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		_managerUI.SetCarSelect(active: true);
		if (!_isEventCarSelect)
		{
			_managerUI.SetCarSelect(active: true);
			_managerUI.ShowPrevWindow(fade: false);
		}
		else
		{
			_managerUI.ShowEventViewWindow(_previewEventType);
		}
		Close();
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			_isEventCarSelect = false;
			V.OnCarEntered -= OnCarChanged;
			base.Close();
		}
	}

	private void SnapTo()
	{
		_garageController.LastSelecterCarInGarage = GarageMenuWindow.SelectedSlotId;
		if (_garageController.LastSelecterCarInGarage != -1)
		{
			int elementIndexBySlotId = SlotCollectionUI.GetElementIndexBySlotId(GarageMenuWindow.SelectedSlotId);
			if (elementIndexBySlotId <= ContentPanel.childCount)
			{
				SimpleScrollSnap.GoToPanel(elementIndexBySlotId);
			}
		}
	}

	private void OnCarChanged(GameObject car, VehicleData vehicleData)
	{
		SnapTo();
		ParseCarInfo(vehicleData);
	}

	private void ParseCarInfo(VehicleData vehicleData)
	{
		TransmissionText.text = (vehicleData.GetPowertrain.IsAutomaticGearbox ? new LocalizedString("Cartuningwindow.Automatic") : new LocalizedString("Cartuningwindow.Manual"));
		WeightText.text = Utils.GetMass(vehicleData.GetVehicleEntityMono.CarEntity.ChassisData.Weight, round: true);
		VehicleClass classVehicle = Utils.GetClassVehicle(vehicleData.GetVehicleEntityMono.CarEntity.PerformancePoints, vehicleData.CarId);
		ClassText.text = classVehicle.ToString();
	}
}
