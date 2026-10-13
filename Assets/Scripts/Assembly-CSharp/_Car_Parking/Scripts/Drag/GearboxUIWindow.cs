using System.Collections.Generic;
using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Drag
{
	public class GearboxUIWindow : WindowScript, IClickable, IBackableWindow
	{
		[HideInInspector]
		public int EngineModel;

		[HideInInspector]
		public int CarId;

		private ICarInfoService _carInfoService;

		[SerializeField]
		private PartType[] PartDatas = default;

		[SerializeField]
		private string PreviousWindowKey = default;

		[SerializeField]
		private UIPartsController UIPartsController = default;

		[SerializeField]
		private UIPartElement FrontLSDPartsElement = default;

		[SerializeField]
		private UIPartElement RearLSDPartsElement = default;

		[SerializeField]
		private PartType DefaultPartTypeInNullElements = default;

		[SerializeField]
		private CategoryType CategoryType = default;

		private HashSet<int> _installedModelIds = new HashSet<int>();

		private CarTuningData _carTuningData;

		private bool _isInCar;

		[SerializeField]
		private EngineImageRotator DetailImageRotator = default;

		private VehicleEntityProvider _vehicleEntityProvider;

		[SerializeField]
		private Button GearBoxConfigButton = default;

		[SerializeField]
		private GameObject GearBoxConfigWindow = default;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.MenuWithRotation;

		[Inject]
		public void Construct(ICarInfoService carInfoService)
		{
			_carInfoService = carInfoService;
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			_managerUI.ShowWindow(PreviousWindowKey).Forget();
			if (_isInCar)
			{
				_managerUI.EditingEngineKitData = null;
			}
			Close();
		}

		public override void ReloadWindow()
		{
			_managerUI.EditingEngineKitData = null;
			Init();
		}

		protected override void Start()
		{
			base.Start();
			_vehicleEntityProvider = V.Car.GetComponent<VehicleEntityProvider>();
			InitBackButton();
			Init();
			if ((bool)DetailImageRotator && CategoryType != CategoryType.None)
			{
				_ = DetailImageRotator.Init(GetRotateImagePath());
			}
			GearBoxConfigButton.onClick.AddListener(EnableGearBoxConfigWindow);
			GearBoxConfigWindow.SetActive(value: false);
			GearBoxConfigWindow.OnEnableAsObservable().Subscribe((Unit _) =>
			{
				SuspendUiNavigation();
			}).AddTo(this);
			GearBoxConfigWindow.OnDisableAsObservable().Subscribe((Unit _) =>
			{
				RestoreUiNavigation();
			}).AddTo(this);
		}

		protected override void RefreshCache(GameObject target = null, GameObject exceptions = null)
		{
			base.RefreshCache(target, GearBoxConfigWindow);
		}

		private void EnableGearBoxConfigWindow()
		{
			GearBoxConfigWindow.SetActive(value: true);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			GearBoxConfigButton.onClick.RemoveAllListeners();
		}

		private string GetRotateImagePath()
		{
			return CategoryType switch
			{
				CategoryType.Block => "Block_0", 
				CategoryType.Intercooler => "Intercooler_0", 
				CategoryType.Gearbox => "Gearbox_0", 
				CategoryType.FuelRail => "Fuel_Rail_0", 
				CategoryType.ExhaustPipe => "ExhaustPipe_0", 
				CategoryType.EngineHead => "EngineHead_0", 
				_ => null, 
			};
		}

		private void Init()
		{
			DestroyItems();
			EngineModel = _vehicleEntityProvider.CarEntity.Engine.Part.ModelId;
			CarId = V.CarId;
			CarInfo fromSlot = _carInfoService.GetFromSlot(V.SlotID);
			if (fromSlot != null)
			{
				_carTuningData = fromSlot.TuningData;
				if (_managerUI.EditingEngineKitData == null || _managerUI.EditingEngineKitData.EngineId.InstanceID == -1)
				{
					_isInCar = false;
					_managerUI.EditingEngineKitData = _carTuningData.EngineKitData;
				}
				else
				{
					_isInCar = _managerUI.EditingEngineKitData.EngineId.ModelId == _carTuningData.EngineKitData.EngineId.ModelId;
				}
				_ = ParseDetails(_managerUI.EditingEngineKitData);
			}
		}

		private async UniTaskVoid ParseDetails(EngineKitData engineKitData)
		{
			List<PartData> list = new List<PartData>();
			for (int i = 0; i < PartDatas.Length; i++)
			{
				PartData partData = null;
				switch (PartDatas[i])
				{
				case PartType.Clutch:
					partData = _carTuningData.EngineKitData.GetInstalledPartDataByPartType(PartDatas[i]);
					break;
				case PartType.GearBox:
					partData = _carTuningData.GetInstalledPartDataByPartType(PartDatas[i]);
					break;
				}
				if (partData != null && !_installedModelIds.Contains(partData.ModelId))
				{
					_installedModelIds.Add(partData.ModelId);
					list.Add(partData);
				}
			}
			UIPartsController.CreateElements(list, this, DefaultPartTypeInNullElements, _isInCar);
			ParseLSD();
		}

		private void ParseLSD()
		{
			_ = _carTuningData.FrontLSD;
			TransmissionValues getGearboxValues = _vehicleEntityProvider.CarEntity.GetGearboxValues;
			HideLSDCells();
			switch ((GearboxType)getGearboxValues.Type)
			{
			case GearboxType.GearFwd:
				ActivateLSDCell(_carTuningData.FrontLSD, UIPartsController, FrontLSDPartsElement);
				break;
			case GearboxType.GearAwd:
			case GearboxType.Gear4Wd:
				ActivateLSDCell(_carTuningData.FrontLSD, UIPartsController, FrontLSDPartsElement);
				ActivateLSDCell(_carTuningData.RearLSD, UIPartsController, RearLSDPartsElement);
				break;
			case GearboxType.GearRwd:
				ActivateLSDCell(_carTuningData.RearLSD, UIPartsController, RearLSDPartsElement);
				break;
			default:
				HideLSDCells();
				break;
			}
		}

		private void HideLSDCells()
		{
			FrontLSDPartsElement.SetActive(active: false);
			RearLSDPartsElement.SetActive(active: false);
		}

		private void ActivateLSDCell(PartData partData, UIPartsController uiPartsController, UIPartElement LSDCell)
		{
			LSDCell.Init(partData, this, uiPartsController);
			LSDCell.SetActive(active: true);
		}

		public void OnItemClick(PartData parData)
		{
			_managerUI.ShowMarketInventoryWindow(parData, 0);
			Close();
		}

		private void DestroyItems()
		{
			_installedModelIds.Clear();
			UIPartsController.DestroyElements();
		}

		public void OnItemClick(PartData parData, int index = 0, UIElement uiElement = null)
		{
		}

		public void CloseClickable()
		{
			_managerUI.SetPreviousWindow("GearboxUIWindow");
			Close();
		}
	}
}
