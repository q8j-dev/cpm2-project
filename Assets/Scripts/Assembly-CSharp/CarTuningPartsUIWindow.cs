using System.Collections.Generic;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.SOAP_Registry;

public class CarTuningPartsUIWindow : WindowScript, IClickable, IBackableWindow
{
	private AllParts _allParts;

	[HideInInspector]
	public int EngineModel;

	[HideInInspector]
	public int CarId;

	private IPrefabFactory _prefabFactory;

	private ICarInfoService _carInfoService;

	[SerializeField]
	private PartType[] PartDatas = default;

	[SerializeField]
	private UIPartsController UIPartsController = default;

	[SerializeField]
	private PartType DefaultPartTypeInNullElements = default;

	[SerializeField]
	private CategoryType CategoryType = default;

	private HashSet<int> _installedModelIds = new HashSet<int>();

	private CarTuningData _carTuningData;

	private bool _isInCar;

	[SerializeField]
	private EngineImageRotator DetailImageRotator = default;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.MenuWithRotation;

	[Inject]
	public void Construct(AllParts allParts, IPrefabFactory prefabFactory, ICarInfoService carInfoService, ManagerUI managerUI)
	{
		_allParts = allParts;
		_prefabFactory = prefabFactory;
		_carInfoService = carInfoService;
		_managerUI = managerUI;
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		_managerUI.ShowEngineUIWindow();
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
		InitBackButton();
		Init();
		if ((bool)DetailImageRotator && CategoryType != CategoryType.None && DetailImageRotator.gameObject.activeSelf)
		{
			_ = DetailImageRotator.Init(GetRotateImagePath());
		}
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
			CategoryType.None => null, 
			_ => null, 
		};
	}

	private void Init()
	{
		DestroyItems();
		EngineModel = V.Car.GetComponent<VehicleEntityProvider>().CarEntity.Engine.Part.ModelId;
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
				_isInCar = _managerUI.EditingEngineKitData.EngineId.InstanceID == _carTuningData.EngineKitData.EngineId.InstanceID;
			}
			_ = ParseDetails(_managerUI.EditingEngineKitData);
		}
	}

	private async UniTaskVoid ParseDetails(EngineKitData engineKitData)
	{
		List<PartData> list = new List<PartData>();
		for (int i = 0; i < PartDatas.Length; i++)
		{
			PartData partData = PartDatas[i] switch
			{
				PartType.Clutch => _carTuningData.EngineKitData.GetInstalledPartDataByPartType(PartDatas[i]), 
				PartType.GearBox => _carTuningData.GetInstalledPartDataByPartType(PartDatas[i]), 
				PartType.ECU => _carTuningData.GetInstalledPartDataByPartType(PartDatas[i]), 
				_ => engineKitData.GetInstalledPartDataByPartType(PartDatas[i]), 
			};
			if (partData != null && !_installedModelIds.Contains(partData.ModelId))
			{
				_installedModelIds.Add(partData.ModelId);
				list.Add(partData);
			}
		}
		UIPartsController.CreateElements(list, this, DefaultPartTypeInNullElements, _isInCar);
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
		_managerUI.SetPreviousWindow(base.GUID);
		Close();
	}
}
