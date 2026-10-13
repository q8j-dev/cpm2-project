using System.Collections.Generic;
using CPM.Database;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Inventory;
using _Car_Parking.Scripts.Localization;

public class UIPartsController : MonoBehaviour
{
	private const string TitleRemovePartKey = "Engineuiwindow.RemovePart";

	private const string ContentRemovePartKey = "Engineuiwindow.RemovePartAssert";

	[SerializeField]
	private UIPartsPlace[] UIPartsPlaces;

	[SerializeField]
	private UIPartElement UIPartElementPrefab;

	private List<UIPartElement> _childs = new List<UIPartElement>();

	private HashSet<int> _createdModelIds = new HashSet<int>();

	private IPrefabFactory _prefabFactory;

	private ManagerUI _managerUI;

	private IEngineKitsInventoryService _engineKitsInventoryService;

	private ICarTuningFactory _carTuningFactory;

	private bool _isEngineInCar;

	private ICarInfoService _carInfoService;

	[Inject]
	public void Construct(IPrefabFactory prefabFactory, ManagerUI managerUI, IEngineKitsInventoryService engineKitsInventoryService, ICarTuningFactory carTuningFactory, ICarInfoService carInfoService)
	{
		_prefabFactory = prefabFactory;
		_managerUI = managerUI;
		_engineKitsInventoryService = engineKitsInventoryService;
		_carTuningFactory = carTuningFactory;
		_carInfoService = carInfoService;
	}

	[ContextMenu("FillList")]
	public void FillList()
	{
		UIPartsPlaces = GetComponentsInChildren<UIPartsPlace>();
	}

	public void CreateElements(List<PartData> partData, IClickable clickable, PartType DefaultPartTypeInNullElements = PartType.None, bool isInCar = true)
	{
		_isEngineInCar = isInCar;
		_createdModelIds = new HashSet<int>();
		for (int i = 0; i < partData.Count; i++)
		{
			if (partData[i].PartType == PartType.None)
			{
				partData[i].PartType = DefaultPartTypeInNullElements;
			}
			for (int j = 0; j < UIPartsPlaces.Length; j++)
			{
				bool flag = false;
				for (int k = 0; k < UIPartsPlaces[j].PartType.Length; k++)
				{
					if (UIPartsPlaces[j].PartType[k] == partData[i].PartType)
					{
						flag = true;
						break;
					}
				}
				if (flag && !_createdModelIds.Contains(partData[i].ModelId))
				{
					_createdModelIds.Add(partData[i].ModelId);
					UIPartElement uIPartElement = _prefabFactory.Instantiate(UIPartElementPrefab, UIPartsPlaces[j].transform);
					uIPartElement.Init(partData[i], clickable, this);
					_childs.Add(uIPartElement);
				}
			}
		}
	}

	public void RemoveDetail(PartType partType)
	{
		_managerUI.ShowUI?.Invoke(obj: false);
		_managerUI.ShowConfirmWindow(() =>
		{
			if (_isEngineInCar)
			{
				SetPartInCar(partType);
			}
			else
			{
				SetPartNotInCar(partType);
			}
			_managerUI.ReloadActiveWindows();
			_managerUI.ShowUI?.Invoke(obj: true);
		}, new LocalizedString("Engineuiwindow.RemovePart"), new LocalizedString("Engineuiwindow.RemovePartAssert"), () =>
		{
			_managerUI.ShowUI?.Invoke(obj: true);
		});
	}

	private void SetPartInCar(PartType partType)
	{
		CarInfo fromSlot = _carInfoService.GetFromSlot(V.SlotID);
		PartData[] partsData = new PartData[1]
		{
			new PartData
			{
				PartType = partType,
				InstanceID = -1
			}
		};
		_carTuningFactory.SetPartsInCar(fromSlot, partsData);
	}

	private void SetPartNotInCar(PartType partType)
	{
		PartData currentPart = new PartData
		{
			PartType = partType
		};
		_carTuningFactory.SetOnePart(currentPart, _managerUI.EditingEngineKitData);
		_engineKitsInventoryService.Save();
	}

	public void DestroyElements()
	{
		_createdModelIds.Clear();
		for (int i = 0; i < _childs.Count; i++)
		{
			_childs[i].gameObject.SetActive(value: false);
			Object.Destroy(_childs[i].gameObject);
		}
		_childs.Clear();
	}
}
