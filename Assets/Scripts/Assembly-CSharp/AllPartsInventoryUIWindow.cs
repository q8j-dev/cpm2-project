using System.Collections.Generic;
using System.Linq;
using CPM.Database;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Inventory;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

public class AllPartsInventoryUIWindow : WindowScript, IBackableWindow
{
	private readonly HashSet<AllInventoryFilters> _allInventoryFilters = new HashSet<AllInventoryFilters>
	{
		AllInventoryFilters.AirSystem,
		AllInventoryFilters.Block,
		AllInventoryFilters.Cooling,
		AllInventoryFilters.Head,
		AllInventoryFilters.Turbo,
		AllInventoryFilters.FuelSystem,
		AllInventoryFilters.ECU
	};

	private List<PartData> _selectedItems = new List<PartData>();

	private Dictionary<PartType, PartData> _installedPartDictionary;

	private Dictionary<AllInventoryFilters, PartType[]> _categoryElements;

	private IMenuWindowsMediator _menuWindowsMediator;

	[SerializeField]
	private AllInventoryFilterLineElement AllInventoryFilterLineElementPrefab;

	[SerializeField]
	private AllDataInventoryElement AllDataInventoryElementPrefab;

	[SerializeField]
	private AllInventoryFilterGridElement FilterGridElementPrefab;

	[SerializeField]
	private TextMeshProUGUI FilterText;

	[SerializeField]
	private TextMeshProUGUI FilterDetailsCount;

	[SerializeField]
	private Button FilterButton;

	[SerializeField]
	private MarketInventoryShowCasePanel MarketInventoryShowCasePanel;

	[SerializeField]
	private GameObject ShowcasePanel;

	[SerializeField]
	private CanvasGroup InfoPanelCanvasGroup;

	[SerializeField]
	private ScrollRect InventoryScrollRectPrefab;

	[SerializeField]
	private ScrollRect InventoryFilteredScrollRectPrefab;

	[SerializeField]
	private Transform ScrollRectPlace;

	[SerializeField]
	private AllInventoryPopup AllInventoryPopup;

	[SerializeField]
	private GameObject MainPanel;

	[SerializeField]
	private Button SetButton;

	[SerializeField]
	private Button SellButton;

	private UiScrollRectOcclusiion _uiScrollRectOcclusion;

	private Transform _parentTransform;

	private AllParts _allParts;

	private ICarInfoService _carInfoService;

	private CarInfo _carInfo;

	private IInventoryPartsService _inventoryPartsService;

	private IPrefabFactory _prefabFactory;

	private ITimerTuningService _timerTuningService;

	private int _count;

	private ScrollRect _scrollRect;

	public Color CompatibleColor;

	public Color NotCompatiblColor;

	public Color InstalledColor;

	public Color[] RarityColots;

	public Color WarningColor;

	public Color ErrorColor;

	private CurrencyManagerController _currencyManagerController;

	[Inject]
	public void Construct(ManagerUI managerUI, AllParts allParts, ICarInfoService carInfoService, IInventoryPartsService inventoryPartsService, IPrefabFactory prefabFactory, ITimerTuningService timerTuningService, IMenuWindowsMediator menuWindowsMediator, CurrencyManagerController currencyManagerController)
	{
		_currencyManagerController = currencyManagerController;
		_menuWindowsMediator = menuWindowsMediator;
		_inventoryPartsService = inventoryPartsService;
		_managerUI = managerUI;
		_allParts = allParts;
		_carInfoService = carInfoService;
		_prefabFactory = prefabFactory;
		_timerTuningService = timerTuningService;
	}

	protected override void Start()
	{
		base.Start();
		InitBackButton();
		SetButton.onClick.AddListener(OnSetClick);
		SellButton.onClick.AddListener(OnSellClick);
		FilterButton.onClick.AddListener(ShowFilter);
		_carInfo = _carInfoService.GetFromSlot(V.SlotID);
		ParseCollections();
		Init();
	}

	private void ShowFilter()
	{
		ShowFilterSuspendNavigation().Forget();
	}

	private async UniTask ShowFilterSuspendNavigation()
	{
		WindowScript windowScript = await _managerUI.ShowAllInventoryFilterWindow();
		if ((bool)windowScript)
		{
			SuspendUiNavigation();
			windowScript.OnDestroyAsObservable().Subscribe((Unit _) =>
			{
				RestoreUiNavigation();
			});
		}
	}

	public override void ReloadWindow()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_selectedItems.Clear();
		ParseCollections();
		Init();
	}

	public override void RebuildWindow()
	{
		Init();
	}

	private void Init()
	{
		CleanElements();
		ShowcasePanel.SetActive(value: false);
		InfoPanelCanvasGroup.alpha = 0f;
		_carInfo = _carInfoService.GetFromSlot(V.SlotID);
		_managerUI.EditingEngineKitData = _carInfo.TuningData.EngineKitData;
		bool flag = _managerUI.CurrentAllInventoryFilters.Contains(AllInventoryFilters.All);
		ParseFilteredDatas(flag ? _allInventoryFilters : _managerUI.CurrentAllInventoryFilters);
		FillFilterText();
	}

	private void FillFilterText()
	{
		bool num = _managerUI.CurrentAllInventoryInstalledElement != AllInventoryInstalledFilter.All;
		string text = "";
		if (num)
		{
			switch (_managerUI.CurrentAllInventoryInstalledElement)
			{
			case AllInventoryInstalledFilter.All:
				text = string.Concat(new LocalizedString("Allinventoryfilterwindow.All"), ", ");
				break;
			case AllInventoryInstalledFilter.Installed:
				text = string.Concat(new LocalizedString("Allinventoryfilterwindow.Installed"), ", ");
				break;
			case AllInventoryInstalledFilter.NotInstalled:
				text = string.Concat(new LocalizedString("Allinventoryfilterwindow.NotInstalled"), ", ");
				break;
			}
		}
		foreach (AllInventoryFilters currentAllInventoryFilter in _managerUI.CurrentAllInventoryFilters)
		{
			switch (currentAllInventoryFilter)
			{
			case AllInventoryFilters.All:
				text = string.Concat(text, new LocalizedString("Allinventoryfilterwindow.All"), ", ");
				break;
			case AllInventoryFilters.Block:
				text = string.Concat(text, new LocalizedString("Allinventoryfilterwindow.Block"), ", ");
				break;
			case AllInventoryFilters.Cooling:
				text = string.Concat(text, new LocalizedString("Allinventoryfilterwindow.Cooling"), ", ");
				break;
			case AllInventoryFilters.Head:
				text = string.Concat(text, new LocalizedString("Allinventoryfilterwindow.Head"), ", ");
				break;
			case AllInventoryFilters.Turbo:
				text = string.Concat(text, new LocalizedString("Allinventoryfilterwindow.Turbo"), ", ");
				break;
			case AllInventoryFilters.AirSystem:
				text = string.Concat(text, new LocalizedString("Allinventoryfilterwindow.AirSystem"), ", ");
				break;
			case AllInventoryFilters.FuelSystem:
				text = string.Concat(text, new LocalizedString("Allinventoryfilterwindow.FuelSystem"), ", ");
				break;
			case AllInventoryFilters.ECU:
				text = string.Concat(text, new LocalizedString("Allinventoryfilterwindow.ECU"), ", ");
				break;
			}
		}
		FilterDetailsCount.text = _count.ToString();
		text = text.Remove(text.Length - 2);
		FilterText.text = text;
	}

	private void RebuildLayouts()
	{
		UIHelper.DelayedRebuildLayout(_scrollRect.content).Forget();
		_uiScrollRectOcclusion.Init();
	}

	private void ParseAll()
	{
		_scrollRect = _prefabFactory.Instantiate(InventoryScrollRectPrefab, ScrollRectPlace);
		_uiScrollRectOcclusion = _scrollRect.GetComponent<UiScrollRectOcclusiion>();
		_count = 0;
		foreach (KeyValuePair<AllInventoryFilters, PartType[]> categoryElement in _categoryElements)
		{
			PartType[] value = categoryElement.Value;
			foreach (PartType partType in value)
			{
				List<PartData> list = new List<PartData>();
				if (_installedPartDictionary[partType] != null)
				{
					list.Add(_installedPartDictionary[partType]);
				}
				list = list.Concat(_inventoryPartsService.GetAllParts(partType)).ToList();
				FillCategory(list, _scrollRect.content, _installedPartDictionary[partType]);
			}
		}
		RebuildLayouts();
	}

	private void ParseFilteredDatas(HashSet<AllInventoryFilters> allInventoryFiltersArray)
	{
		_scrollRect = _prefabFactory.Instantiate(InventoryFilteredScrollRectPrefab, ScrollRectPlace);
		_uiScrollRectOcclusion = _scrollRect.GetComponent<UiScrollRectOcclusiion>();
		_count = 0;
		int num = 0;
		foreach (AllInventoryFilters item in allInventoryFiltersArray)
		{
			CreateLine(num, item);
			CreateGrid(num, item);
			num++;
		}
		RebuildLayouts();
	}

	private void CreateLine(int i, AllInventoryFilters filter)
	{
		_prefabFactory.Instantiate(AllInventoryFilterLineElementPrefab, _scrollRect.content).SetText(filter.ToString());
	}

	private void CreateGrid(int i, AllInventoryFilters filter)
	{
		Transform content = _scrollRect.content;
		AllInventoryFilterGridElement allInventoryFilterGridElement = _prefabFactory.Instantiate(FilterGridElementPrefab, _scrollRect.content);
		content = allInventoryFilterGridElement.transform;
		PartType[] array = _categoryElements[filter];
		foreach (PartType partType in array)
		{
			List<PartData> list = new List<PartData>();
			if (_installedPartDictionary[partType] != null)
			{
				list.Add(_installedPartDictionary[partType]);
			}
			list = list.Concat(_inventoryPartsService.GetAllParts(partType)).ToList();
			FillCategory(list, content, _installedPartDictionary[partType], filter);
		}
		allInventoryFilterGridElement.RebuildGrid();
	}

	private void FillCategory(List<PartData> partDatas, Transform parent, PartData installedPart, AllInventoryFilters filter = AllInventoryFilters.All)
	{
		for (int i = 0; i < partDatas.Count; i++)
		{
			if (partDatas[i].InstanceID == -1)
			{
				continue;
			}
			PartItem part = _allParts.GetPart<PartItem>(partDatas[i]);
			if (part != null && part.IsTamperedConfig())
			{
				Debug.LogWarning($"[AntiCheat] Hiding tampered part from all-parts inventory: {part.TypeId} ModelId={part.ModelId}");
				continue;
			}
			bool flag = CheckIsNotCompatible(partDatas[i], part, _managerUI.EditingEngineKitData.EngineId.ModelId);
			if (!flag)
			{
				bool flag2 = installedPart != null && installedPart.InstanceID == partDatas[i].InstanceID;
				if ((_managerUI.CurrentAllInventoryInstalledElement == AllInventoryInstalledFilter.All || ((_managerUI.CurrentAllInventoryInstalledElement != AllInventoryInstalledFilter.Installed || flag2) && !((_managerUI.CurrentAllInventoryInstalledElement == AllInventoryInstalledFilter.NotInstalled) & flag2))) && (_managerUI.CurrentAllInventoryFilters.Contains(filter) || _managerUI.CurrentAllInventoryFilters.Contains(AllInventoryFilters.All)) && part != null)
				{
					_prefabFactory.Instantiate(AllDataInventoryElementPrefab, parent).Init(partDatas[i], part, !flag, flag2, this);
					_count++;
				}
			}
		}
	}

	private bool CheckIsNotCompatible(PartData partData, PartItem part, int engineModelId)
	{
		if (part == null)
		{
			return false;
		}
		bool flag = false;
		if (partData.PartType != PartType.ECU)
		{
			return part.CheckNotCompatibleEngine(engineModelId);
		}
		return part.CheckNotCompatibleFull(V.CarId, engineModelId);
	}

	public new void Close()
	{
		if (!base.IsCloseCanceled)
		{
			_managerUI.ShowEngineUIWindow();
			base.NavigationModeSet(isEnabled: false);
			base.Close();
		}
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		_managerUI.ShowEngineUIWindow();
		Close();
	}

	private void ParseCollections()
	{
		_installedPartDictionary = new Dictionary<PartType, PartData>
		{
			{
				PartType.Block_Inline,
				(_managerUI.EditingEngineKitData.Block != null && _managerUI.EditingEngineKitData.Block.PartType == PartType.Block_Inline) ? _managerUI.EditingEngineKitData.Block : null
			},
			{
				PartType.Block_Vee,
				(_managerUI.EditingEngineKitData.Block != null && _managerUI.EditingEngineKitData.Block.PartType == PartType.Block_Vee) ? _managerUI.EditingEngineKitData.Block : null
			},
			{
				PartType.Crankshaft,
				_managerUI.EditingEngineKitData.Crankshaft
			},
			{
				PartType.Pistons,
				_managerUI.EditingEngineKitData.Pistons
			},
			{
				PartType.Flywheel,
				_managerUI.EditingEngineKitData.Flywheel
			},
			{
				PartType.ConnectionRod,
				_managerUI.EditingEngineKitData.ConnectionRod
			},
			{
				PartType.SOHC_CylinderHead,
				(_managerUI.EditingEngineKitData.CylinderHead != null && _managerUI.EditingEngineKitData.CylinderHead.PartType == PartType.SOHC_CylinderHead) ? _managerUI.EditingEngineKitData.CylinderHead : null
			},
			{
				PartType.DOHC_CylinderHead,
				(_managerUI.EditingEngineKitData.CylinderHead != null && _managerUI.EditingEngineKitData.CylinderHead.PartType == PartType.DOHC_CylinderHead) ? _managerUI.EditingEngineKitData.CylinderHead : null
			},
			{
				PartType.OHV_CylinderHead,
				(_managerUI.EditingEngineKitData.CylinderHead != null && _managerUI.EditingEngineKitData.CylinderHead.PartType == PartType.OHV_CylinderHead) ? _managerUI.EditingEngineKitData.CylinderHead : null
			},
			{
				PartType.SOHC_Camshaft,
				(_managerUI.EditingEngineKitData.Camshaft1 != null && _managerUI.EditingEngineKitData.Camshaft1.PartType == PartType.SOHC_Camshaft) ? _managerUI.EditingEngineKitData.Camshaft1 : null
			},
			{
				PartType.OHV_Camshaft,
				(_managerUI.EditingEngineKitData.Camshaft1 != null && _managerUI.EditingEngineKitData.Camshaft1.PartType == PartType.OHV_Camshaft) ? _managerUI.EditingEngineKitData.Camshaft1 : null
			},
			{
				PartType.DOHC_Camshaft_Intake,
				(_managerUI.EditingEngineKitData.Camshaft1 != null && _managerUI.EditingEngineKitData.Camshaft1.PartType == PartType.DOHC_Camshaft_Intake) ? _managerUI.EditingEngineKitData.Camshaft1 : null
			},
			{
				PartType.IntakeManifold,
				_managerUI.EditingEngineKitData.IntakeManifold
			},
			{
				PartType.AirFilter,
				_managerUI.EditingEngineKitData.AirFilter
			},
			{
				PartType.ExhaustHeader,
				_managerUI.EditingEngineKitData.ExhaustHeader
			},
			{
				PartType.ExhaustPipe,
				_managerUI.EditingEngineKitData.ExhaustPipe
			},
			{
				PartType.ThrottleValve,
				_managerUI.EditingEngineKitData.ThrottleValve
			},
			{
				PartType.FuelRail,
				_managerUI.EditingEngineKitData.FuelRail
			},
			{
				PartType.FuelInjectors,
				_managerUI.EditingEngineKitData.FuelInjectors
			},
			{
				PartType.FuelPump,
				_managerUI.EditingEngineKitData.FuelPump
			},
			{
				PartType.Radiator,
				_managerUI.EditingEngineKitData.Radiator
			},
			{
				PartType.ECU,
				_carInfo.TuningData.ECU
			},
			{
				PartType.RotorHousing,
				(_managerUI.EditingEngineKitData.RotorHousing != null && _managerUI.EditingEngineKitData.RotorHousing.PartType == PartType.RotorHousing) ? _managerUI.EditingEngineKitData.RotorHousing : null
			},
			{
				PartType.Rotor,
				(_managerUI.EditingEngineKitData.Rotors != null && _managerUI.EditingEngineKitData.Rotors.PartType == PartType.Rotor) ? _managerUI.EditingEngineKitData.Rotors : null
			},
			{
				PartType.RotorTrackingGear,
				(_managerUI.EditingEngineKitData.RotorTrackingGear != null && _managerUI.EditingEngineKitData.RotorTrackingGear.PartType == PartType.RotorTrackingGear) ? _managerUI.EditingEngineKitData.RotorTrackingGear : null
			},
			{
				PartType.EccentricShaft,
				(_managerUI.EditingEngineKitData.EccentricShaft != null && _managerUI.EditingEngineKitData.EccentricShaft.PartType == PartType.EccentricShaft) ? _managerUI.EditingEngineKitData.EccentricShaft : null
			}
		};
		EngineKitData editingEngineKitData = _managerUI.EditingEngineKitData;
		if (editingEngineKitData.Charger != null)
		{
			_installedPartDictionary.Add(PartType.SuperCharger, (editingEngineKitData.Charger.PartType == PartType.SuperCharger) ? editingEngineKitData.Charger : null);
			_installedPartDictionary.Add(PartType.TurboCharger, (editingEngineKitData.Charger.PartType == PartType.TurboCharger) ? editingEngineKitData.Charger : null);
		}
		else
		{
			_installedPartDictionary.Add(PartType.SuperCharger, null);
			_installedPartDictionary.Add(PartType.TurboCharger, null);
		}
		PartType partType = ((editingEngineKitData.CylinderHead != null) ? editingEngineKitData.CylinderHead.PartType : PartType.None);
		if (partType == PartType.DOHC_CylinderHead)
		{
			if (editingEngineKitData.Camshaft2 != null)
			{
				_installedPartDictionary.Add(PartType.DOHC_Camshaft_Exhaust, (editingEngineKitData.Camshaft2.PartType == PartType.DOHC_Camshaft_Exhaust) ? editingEngineKitData.Camshaft2 : null);
			}
			else
			{
				_installedPartDictionary.Add(PartType.DOHC_Camshaft_Exhaust, null);
			}
		}
		if (editingEngineKitData.Intercooler != null)
		{
			_installedPartDictionary.Add(PartType.Intercooler, (editingEngineKitData.Intercooler.PartType == PartType.Intercooler) ? editingEngineKitData.Intercooler : null);
		}
		else
		{
			_installedPartDictionary.Add(PartType.Intercooler, null);
		}
		_categoryElements = new Dictionary<AllInventoryFilters, PartType[]>();
		_categoryElements.Add(AllInventoryFilters.Block, new PartType[6]
		{
			PartType.Block_Inline,
			PartType.Block_Vee,
			PartType.ConnectionRod,
			PartType.Pistons,
			PartType.Flywheel,
			PartType.Crankshaft
		});
		_categoryElements.Add(AllInventoryFilters.Cooling, new PartType[2]
		{
			PartType.Radiator,
			PartType.Intercooler
		});
		_categoryElements.Add(AllInventoryFilters.Head, BuildHeadCategory(partType));
		_categoryElements.Add(AllInventoryFilters.AirSystem, new PartType[4]
		{
			PartType.IntakeManifold,
			PartType.ExhaustHeader,
			PartType.ExhaustPipe,
			PartType.AirFilter
		});
		_categoryElements.Add(AllInventoryFilters.FuelSystem, new PartType[4]
		{
			PartType.FuelRail,
			PartType.ThrottleValve,
			PartType.FuelPump,
			PartType.FuelInjectors
		});
		_categoryElements.Add(AllInventoryFilters.ECU, new PartType[1] { PartType.ECU });
		_categoryElements.Add(AllInventoryFilters.Turbo, new PartType[2]
		{
			PartType.SuperCharger,
			PartType.TurboCharger
		});
	}

	private static PartType[] BuildHeadCategory(PartType headType)
	{
		return headType switch
		{
			PartType.OHV_CylinderHead => new PartType[2]
			{
				PartType.OHV_CylinderHead,
				PartType.OHV_Camshaft
			}, 
			PartType.SOHC_CylinderHead => new PartType[2]
			{
				PartType.SOHC_CylinderHead,
				PartType.SOHC_Camshaft
			}, 
			PartType.DOHC_CylinderHead => new PartType[3]
			{
				PartType.DOHC_CylinderHead,
				PartType.DOHC_Camshaft_Intake,
				PartType.DOHC_Camshaft_Exhaust
			}, 
			_ => new PartType[3]
			{
				PartType.OHV_CylinderHead,
				PartType.SOHC_CylinderHead,
				PartType.DOHC_CylinderHead
			}, 
		};
	}

	public void SelectItem(PartData partData, PartItem part, Vendor vendor, AllDataInventoryElement allDataInventoryElement, bool isInstalled)
	{
		if (!isInstalled)
		{
			_selectedItems.Add(partData);
			if (SamePartTypesExist())
			{
				SetButton.interactable = false;
			}
		}
		if (!ShowcasePanel.activeSelf)
		{
			ShowcasePanel.SetActive(value: true);
		}
		InfoPanelCanvasGroup.alpha = 1f;
		MarketInventoryShowCasePanel.Init(partData, part, vendor, isInCar: true, isInstalled: false, MarketInventoryWindow.OpenPanelType.Inventory, isNotCompatible: false);
		SetActiveSetSellButtons(_selectedItems.Count > 0);
	}

	public void RemoveSelectedItem(PartData partData, PartItem part, Vendor vendor, AllDataInventoryElement allDataInventoryElement, bool isInstalled)
	{
		if (_selectedItems.Contains(partData))
		{
			_selectedItems.Remove(partData);
			if (!SamePartTypesExist())
			{
				SetButton.interactable = true;
			}
		}
		SetActiveSetSellButtons(_selectedItems.Count > 0);
		InfoPanelCanvasGroup.alpha = 0f;
	}

	private void SetActiveSetSellButtons(bool isActive)
	{
		SetButton.SetActive(isActive);
		SellButton.SetActive(isActive);
	}

	private bool SamePartTypesExist()
	{
		List<PartType> list = new List<PartType>();
		foreach (PartData selectedItem in _selectedItems)
		{
			if (list.Contains(selectedItem.PartType))
			{
				return true;
			}
			list.Add(selectedItem.PartType);
		}
		return false;
	}

	private void OnSellClick()
	{
		AllInventoryPopup.Init(_selectedItems, isSell: true, OnSellConfirm);
		AllInventoryPopup.gameObject.SetActive(value: true);
		MainPanel.SetActive(value: false);
	}

	private void OnSetClick()
	{
		AllInventoryPopup.Init(_selectedItems, isSell: false, OnSetConfirm);
		AllInventoryPopup.gameObject.SetActive(value: true);
		MainPanel.SetActive(value: false);
	}

	private void OnSellConfirm()
	{
		AllInventoryPopup.gameObject.SetActive(value: false);
		MainPanel.SetActive(value: true);
		PartData[] array = _selectedItems.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				PartItem part = _allParts.GetPart(array[i]);
				ObscuredInt obscuredInt = Mathf.RoundToInt(array[i].GetCondition() * (float)(int)part.GetPrice * 0.2f);
				_menuWindowsMediator.RemovePart(array[i]);
				_currencyManagerController.AddMoney(new AdvancedInt(Mathf.Min(obscuredInt, 2000)));
			}
		}
		ReloadWindow();
	}

	private void OnSetConfirm()
	{
		AllInventoryPopup.SetActive(active: false);
		MainPanel.SetActive(value: true);
		List<PartData> list = new List<PartData>();
		foreach (PartData selectedItem in _selectedItems)
		{
			if (selectedItem != null)
			{
				list.Add(selectedItem);
			}
		}
		_timerTuningService.TryCreateTimer(V.SlotID, list.ToArray()).Get(out var value);
		if (!value)
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("AllPartsInventoryUIWindow.Thisengineisintheservicenow.Pleasewait."), 3f).Forget();
		}
		ReloadWindow();
	}

	private void CleanElements()
	{
		_selectedItems.Clear();
		if ((bool)_scrollRect)
		{
			Object.Destroy(_scrollRect.gameObject);
		}
	}

	public void OnCancelPopup()
	{
		MainPanel.SetActive(value: true);
	}
}
