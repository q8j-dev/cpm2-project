using System;
using System.Collections.Generic;
using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.Drag
{
	public class EngineUIWindow : WindowScript, IClickable, IBackableWindow
	{
		[Header("Button prefab")]
		public InventoryUIElement ButtonPrefab;

		private CarTuningData _car;

		public TextMeshProUGUI allWeightText;

		[SerializeField]
		private Button backButton = default;

		[SerializeField]
		private TextMeshProUGUI EngineHeaderText = default;

		public List<ButtonsSlots> ButtonsSlotsList = new List<ButtonsSlots>();

		private List<UIElement> _spawnedButtons = new List<UIElement>();

		private AllParts _allParts;

		private ICarInfoService _carInfoService;

		[SerializeField]
		private UIPartsController UIPartsController = default;

		[SerializeField]
		private UIPartCategoryChecker BlockItemButton = default;

		[SerializeField]
		private UIPartCategoryChecker HeadItemButton = default;

		[SerializeField]
		private UIPartCategoryChecker RotorItemButton = default;

		[SerializeField]
		private UIPartCategoryChecker IntakeExhoustItemButton = default;

		[SerializeField]
		private UIPartCategoryChecker FuelSystemItemButton = default;

		[SerializeField]
		private UIPartCategoryChecker CoolingItemButton = default;

		[SerializeField]
		private Button DefaultEngineButton = default;

		[SerializeField]
		private Button allInventoryDataButton = default;

		[SerializeField]
		private PartType DefaultPartTypeInNullElements = default;

		[SerializeField]
		private EngineImageRotator EngineImageRotator = default;

		private bool _isRotor;

		private bool _isInCar;

		private IEngineTuningTimerService _engineTuningTimerService;

		private OptionalDependency<IEngineTuningTimerService> _engineTuningTimerServiceDep;

		private const string CurrentWindowKey = "EngineUIWindow";

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.MenuWithRotation;

		[Inject]
		public void Construct(AllParts allParts, ICarInfoService carInfoService, OptionalDependency<IEngineTuningTimerService> engineTuningTimerService)
		{
			_engineTuningTimerServiceDep = engineTuningTimerService;
			_carInfoService = carInfoService;
			_allParts = allParts;
		}

		protected override void Awake()
		{
			base.Awake();
			_engineTuningTimerService = _engineTuningTimerServiceDep?.Value;
		}

		protected override void Start()
		{
			base.Start();
			ManagerUI managerUI = _managerUI;
			managerUI.ShowUI = (Action<bool>)Delegate.Combine(managerUI.ShowUI, new Action<bool>(ShowUI));
			InitBackButton();
			CheckEngine();
			backButton.onClick.AddListener(Back);
			BlockItemButton.UIPartCategoryButton.onClick.AddListener(OpenBlockUIWindow);
			RotorItemButton.UIPartCategoryButton.onClick.AddListener(OpenBlockUIWindow);
			FuelSystemItemButton.UIPartCategoryButton.onClick.AddListener(OpenAirFuelTuningMenu);
			HeadItemButton.UIPartCategoryButton.onClick.AddListener(OpenHeadTuningMenu);
			IntakeExhoustItemButton.UIPartCategoryButton.onClick.AddListener(OpenIntakeExhoustTuningMenu);
			DefaultEngineButton.onClick.AddListener(OpenDefaultEnginesMenu);
			allInventoryDataButton.onClick.AddListener(OpenAllInventoryData);
			CoolingItemButton.UIPartCategoryButton.onClick.AddListener(OpenCoolingTuningMenu);
			_ = EngineImageRotator.Init("engine_0");
		}

		private async UniTaskVoid CheckEngineCategories()
		{
			EngineKitData editingEngineKitData = _managerUI.EditingEngineKitData;
			_isRotor = editingEngineKitData.RotorHousing != null;
			List<PartData> partDatas;
			if (_isRotor)
			{
				HeadItemButton.Hide();
				BlockItemButton.Hide();
				partDatas = new List<PartData> { editingEngineKitData.RotorHousing, editingEngineKitData.Rotors, editingEngineKitData.EccentricShaft, editingEngineKitData.RotorTrackingGear, editingEngineKitData.Flywheel };
				RotorItemButton.Check(partDatas);
			}
			else
			{
				RotorItemButton.Hide();
				partDatas = new List<PartData> { editingEngineKitData.Block, editingEngineKitData.Pistons, editingEngineKitData.ConnectionRod, editingEngineKitData.Flywheel, editingEngineKitData.Crankshaft };
				BlockItemButton.Check(partDatas);
				partDatas = new List<PartData> { editingEngineKitData.CylinderHead, editingEngineKitData.Camshaft1, editingEngineKitData.Camshaft2 };
				HeadItemButton.Check(partDatas);
			}
			partDatas = new List<PartData> { editingEngineKitData.IntakeManifold, editingEngineKitData.AirFilter, editingEngineKitData.ExhaustHeader, editingEngineKitData.ExhaustPipe };
			IntakeExhoustItemButton.Check(partDatas);
			partDatas = new List<PartData> { editingEngineKitData.FuelRail, editingEngineKitData.FuelPump, editingEngineKitData.ThrottleValve, editingEngineKitData.FuelInjectors };
			FuelSystemItemButton.Check(partDatas);
			partDatas = new List<PartData> { editingEngineKitData.Radiator, editingEngineKitData.Intercooler };
			CoolingItemButton.Check(partDatas);
		}

		private void CheckEngine()
		{
			UIPartsController.DestroyElements();
			CarInfo fromSlot = _carInfoService.GetFromSlot(V.SlotID);
			if (fromSlot != null)
			{
				CarTuningData tuningData = fromSlot.TuningData;
				if (_managerUI.EditingEngineKitData == null || _managerUI.EditingEngineKitData.EngineId.InstanceID == -1)
				{
					_managerUI.EditingEngineKitData = tuningData.EngineKitData;
				}
				_isInCar = tuningData.EngineKitData.EngineId.InstanceID == _managerUI.EditingEngineKitData.EngineId.InstanceID;
				allWeightText.transform.parent.gameObject.SetActive(_isInCar);
				allInventoryDataButton.gameObject.SetActive(_isInCar);
				DefaultEngineButton.gameObject.SetActive(_isInCar);
				if (_isInCar)
				{
					Init(tuningData);
					_engineTuningTimerService.ObservingEngineId = -1;
				}
				else
				{
					Init(_managerUI.EditingEngineKitData);
					if (_engineTuningTimerService != null)
					{
						_engineTuningTimerService.ObservingEngineId = _managerUI.EditingEngineKitData.EngineId.InstanceID;
					}
				}
			}
			CheckEngineCategories().Forget();
		}

		private void OpenAllInventoryData()
		{
			_managerUI.ShowAllEngineInventoryWindow();
			Close();
		}

		private void OpenCoolingTuningMenu()
		{
			_managerUI.ShowCoolingTuningMenu();
			Close();
		}

		public override void ReloadWindow()
		{
			_managerUI.EditingEngineKitData = null;
			CheckEngine();
		}

		private void OpenHeadTuningMenu()
		{
			_managerUI.ShowHeadTunningWindow();
			Close();
		}

		private void OpenAirFuelTuningMenu()
		{
			_managerUI.ShowAirFuelWindow();
			Close();
		}

		private void OpenBlockUIWindow()
		{
			if (_isRotor)
			{
				_managerUI.ShowRotorBlockUIWindow();
			}
			else
			{
				_managerUI.ShowBlockUIWindow();
			}
			Close();
		}

		private void OpenIntakeExhoustTuningMenu()
		{
			_managerUI.ShowIntakeExhoustTuningWindow();
			Close();
		}

		private void OpenDefaultEnginesMenu()
		{
			_managerUI.ShowDefaultEnginesMenu();
			Close();
		}

		public void OnItemClick(PartData parData, int index, UIElement uiElement = null)
		{
			_managerUI.ShowMarketInventoryWindow(parData, index);
			Close();
		}

		public void CloseClickable()
		{
			Close();
		}

		private void Init(CarTuningData carTuningData)
		{
			_car = carTuningData;
			if (V.Car.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				allWeightText.text = Utils.GetMass(component.CarEntity.Engine.GetWeight(), round: true);
				if ((bool)EngineHeaderText)
				{
					EngineHeaderText.text = $"{carTuningData.EngineKitData.EngineId.GetName()} ({component.CarEntity.Engine.Displacement} cc)";
				}
			}
			ParseDetails(_car.EngineKitData).Forget();
			InitECU();
		}

		private void Init(EngineKitData engineKitData)
		{
			_car = null;
			if (V.Car.TryGetComponent<VehicleEntityProvider>(out var _))
			{
				allWeightText.text = "";
				if ((bool)EngineHeaderText)
				{
					EngineHeaderText.text = engineKitData.EngineId.GetName();
				}
			}
			ParseDetails(engineKitData).Forget();
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			_managerUI.EditingEngineKitData = null;
			if (_isInCar)
			{
				_managerUI.ShowCarTuningWindow();
			}
			else
			{
				_managerUI.ShowDefaultEnginesMenu();
			}
			if (!_isInCar && _engineTuningTimerService != null)
			{
				_engineTuningTimerService.ObservingEngineId = -1;
			}
			Close();
		}

		public override void Close()
		{
			if (!base.IsCloseCanceled)
			{
				base.Close();
				_managerUI.SetPreviousWindow("EngineUIWindow");
				ManagerUI managerUI = _managerUI;
				managerUI.ShowUI = (Action<bool>)Delegate.Remove(managerUI.ShowUI, new Action<bool>(ShowUI));
			}
		}

		private async UniTaskVoid ParseDetails(EngineKitData EngineKitData)
		{
			PartData item = EngineKitData.Charger ?? new PartData
			{
				ModelId = 0,
				PartType = PartType.TurboCharger,
				InstanceID = -1
			};
			List<PartData> partData = new List<PartData> { item };
			UIPartsController.CreateElements(partData, this, DefaultPartTypeInNullElements, _isInCar);
		}

		private void InitECU()
		{
			if (_isInCar && V.Car.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				List<PartData> list = new List<PartData>();
				if (component.CarEntity.Engine.ECU != null)
				{
					PartData partData = component.CarEntity.Engine.ECU.GetPartData();
					_car.ECU = partData;
					list.Add(partData);
				}
				else
				{
					PartData item = _allParts.GetCompatibleEcuFromEngine(_car.EngineKitData.EngineId.ModelId, -1).CreateNewPartDataFromPart();
					list.Add(item);
				}
				UIPartsController.CreateElements(list, this, PartType.None, _isInCar);
			}
		}
	}
}
