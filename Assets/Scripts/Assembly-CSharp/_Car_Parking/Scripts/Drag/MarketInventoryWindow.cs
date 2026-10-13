using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Database;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Inventory;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Drag
{
	public class MarketInventoryWindow : WindowScript, IClickable, IBackableWindow
	{
		public enum OpenPanelType
		{
			Market = 1,
			Inventory = 2
		}

		private CarTuningData _carTuningData;

		[SerializeField]
		private GameObject RightPanel = default;

		[Header("References")]
		[SerializeField]
		private ShowcaseUIItem currentElmnt = default;

		[Header("Buttons")]
		[SerializeField]
		private Toggle marketButton = default;

		[SerializeField]
		private Toggle inventoryButton = default;

		[SerializeField]
		private Button BuyButton = default;

		[SerializeField]
		private Button SetButton = default;

		[SerializeField]
		private Button SellButton = default;

		[SerializeField]
		[Header("Engine model texts")]
		private TMP_Text PartTypeText = default;

		[SerializeField]
		private TMP_Text EngineNameText = default;

		[Header("Spawn")]
		public Transform ParentForButton;

		public Transform ParentForInventoryButtons;

		public UIElement PrefabForMarketButton;

		public InventoryPartUIElement PrefabForInventoryButton;

		[Header("Current state")]
		public OpenPanelType openPanel = OpenPanelType.Market;

		public VisualButtonHandler OnPanelOpened;

		private AllParts _allParts;

		private PartData _currentPart;

		private IMenuWindowsMediator _menuWindowsMediator;

		private int _engineModel;

		private ObscuredInt _carId;

		private ICarTuningFactory _tuningFactory;

		private ICarInfoService _carInfoService;

		private ITimerTuningService _timerTuningService;

		private IEngineTuningTimerService _engineTuningTimerService;

		private bool _isInCarEngine;

		private CurrencyManagerController _currencyManagerController;

		private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

		public Sprite CoinsSprite;

		public Sprite MoneySprite;

		public Color CompatibleColor;

		public Color NotCompatiblColor;

		public Color InstalledColor;

		[SerializeField]
		private MarketInventoryItem MarketInventoryItemPrefab = default;

		[SerializeField]
		private MarketInventoryPopup MarketInventoryPopup = default;

		[SerializeField]
		private MarketInventoryShowCasePanel MarketInventoryShowCasePanel = default;

		[SerializeField]
		private Button FilterButton = default;

		[SerializeField]
		private TextMeshProUGUI FilterText = default;

		[SerializeField]
		private TextMeshProUGUI FilterDetailCountText = default;

		[SerializeField]
		private ScrollRect InventoryScrollRectPrefab = default;

		[SerializeField]
		private Transform ScrollRectPlace = default;

		[SerializeField]
		private int CellCountInOneRow = 1;

		private float _lastGridContentWidth = -1f;

		private IPrefabFactory _prefabFactory;

		private UiScrollRectOcclusiion _uiScrollRectOcclusiion;

		private IInventoryPartsService _inventoryPartsService;

		private CarInfo _carInfo;

		private ScrollRect _mainScrollRect;

		private Vector2 _lastMarketScrollPos;

		private Vector2 _lastInventoryScrollPos;

		private Vector2 _rightPanelPosition;

		private Vector2 _rightPanelSize;

		private bool _rightPanelGeometrySaved;

		[Inject]
		public void Construct(AllParts allParts, IMenuWindowsMediator menuWindowsMediator, ICarTuningFactory tuningFactory, ManagerUI managerUI, ICarInfoService carInfoService, ITimerTuningService timerTuningService, IEngineKitsInventoryService engineKitsInventoryService, IPrefabFactory prefabFactory, IInventoryPartsService inventoryPartsService, CurrencyManagerController currencyManagerController, ICarTuningFactory carTuningFactory, IEngineTuningTimerService engineTuningTimerService, VirtualPurchaseAnalytic virtualPurchaseAnalytic)
		{
			_timerTuningService = timerTuningService;
			_carInfoService = carInfoService;
			_tuningFactory = tuningFactory;
			_menuWindowsMediator = menuWindowsMediator;
			_allParts = allParts;
			_managerUI = managerUI;
			_prefabFactory = prefabFactory;
			_inventoryPartsService = inventoryPartsService;
			_currencyManagerController = currencyManagerController;
			_engineTuningTimerService = engineTuningTimerService;
			_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
		}

		private async UniTaskVoid ReloadLayout()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(_mainScrollRect.content);
			_mainScrollRect.content.anchoredPosition = ((openPanel == OpenPanelType.Inventory) ? _lastInventoryScrollPos : _lastMarketScrollPos);
			await UniTask.Yield(this.GetCancellationTokenOnDestroy());
			ApplyGridCellSize();
			_uiScrollRectOcclusiion.Init();
		}

		private void OnMarketToggleClick(bool state)
		{
			if (state && openPanel != OpenPanelType.Market)
			{
				openPanel = OpenPanelType.Market;
				Init();
			}
		}

		private void OnInventoryMarketToggleClick(bool state)
		{
			if (state && openPanel != OpenPanelType.Inventory)
			{
				openPanel = OpenPanelType.Inventory;
				Init();
			}
		}

		private void ShowFilterMarketInventoryWindow()
		{
			ShowFilterSuspendNavigation().Forget();
		}

		private async UniTask ShowFilterSuspendNavigation()
		{
			if (_managerUI.GetPartToMarket.PartType == PartType.GearBox)
			{
				await _managerUI.ShowGearboxFilterWindow();
			}
			else
			{
				await _managerUI.ShowMarketInventoryFilter();
			}
		}

		protected override void Start()
		{
			base.Start();
			InitBackButton();
			MarketInventoryShowCasePanel.InitMarketWindowButtons(this);
			FilterText.text = FillFilterText();
			bool flag = _managerUI.GetPartToMarket.PartType == PartType.GearBox;
			if (flag)
			{
				FilterButton.onClick.AddListener(ShowFilterMarketInventoryWindow);
			}
			ApplyFilterButtonVisibility(flag);
			RightPanel.SetActive(value: false);
			marketButton.onValueChanged.AddListener(OnMarketToggleClick);
			inventoryButton.onValueChanged.AddListener(OnInventoryMarketToggleClick);
			_carInfo = _carInfoService.GetFromSlot(V.SlotID);
			_carTuningData = _carInfo.TuningData;
			_carId = _carInfo.CarID;
			Init();
			PartTypeText.text = _managerUI.EditingEngineKitData.EngineId.PartType.ToString();
			EngineNameText.text = _managerUI.EditingEngineKitData.EngineId.GetName();
			_engineTuningTimerService.OnTimerCompleted += OnTimerCompleted;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_engineTuningTimerService.OnTimerCompleted -= OnTimerCompleted;
		}

		private void ApplyFilterButtonVisibility(bool visible)
		{
			if (!FilterButton)
			{
				return;
			}
			RectTransform rectTransform = (RightPanel ? (RightPanel.transform as RectTransform) : null);
			RectTransform rectTransform2 = FilterButton.transform as RectTransform;
			if (rectTransform != null && !_rightPanelGeometrySaved)
			{
				_rightPanelPosition = rectTransform.anchoredPosition;
				_rightPanelSize = rectTransform.sizeDelta;
				_rightPanelGeometrySaved = true;
			}
			FilterButton.gameObject.SetActive(visible);
			if (!(rectTransform == null) && !(rectTransform2 == null))
			{
				if (visible)
				{
					rectTransform.anchoredPosition = _rightPanelPosition;
					rectTransform.sizeDelta = _rightPanelSize;
				}
				else
				{
					float num = rectTransform2.anchoredPosition.y + rectTransform2.sizeDelta.y * rectTransform2.pivot.y - _rightPanelPosition.y;
					rectTransform.anchoredPosition = new Vector2(_rightPanelPosition.x, _rightPanelPosition.y + num);
					rectTransform.sizeDelta = new Vector2(_rightPanelSize.x, _rightPanelSize.y + num);
				}
			}
		}

		private void OnTimerCompleted(int engineId)
		{
			ReloadWindow();
		}

		public override void ReloadWindow()
		{
			CloseWindows();
			if (_isInCarEngine)
			{
				_managerUI.EditingEngineKitData = null;
			}
			Init();
		}

		private void CloseWindows()
		{
			MarketInventoryPopup.gameObject.SetActive(value: false);
			MarketInventoryShowCasePanel.gameObject.SetActive(value: true);
		}

		public override void RebuildWindow()
		{
			Init();
		}

		private void Init()
		{
			if (_managerUI.EditingEngineKitData == null || _managerUI.EditingEngineKitData.EngineId.InstanceID == -1)
			{
				_managerUI.EditingEngineKitData = _carInfo.TuningData.EngineKitData;
			}
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_engineModel = _managerUI.EditingEngineKitData.EngineId.ModelId;
			if (_managerUI.EditingEngineKitData.EngineId.PartType == PartType.None)
			{
				_managerUI.EditingEngineKitData = _carTuningData.EngineKitData;
				_engineModel = _managerUI.EditingEngineKitData.EngineId.ModelId;
			}
			_isInCarEngine = _carTuningData.EngineKitData.EngineId.InstanceID == _managerUI.EditingEngineKitData.EngineId.InstanceID;
			ChangeSelectedPart(_managerUI.EditingEngineKitData);
			DisableCachedItems().Forget();
			_mainScrollRect = _prefabFactory.Instantiate(InventoryScrollRectPrefab, ScrollRectPlace);
			_mainScrollRect.onValueChanged.AddListener((Vector2 v) =>
			{
				if (openPanel == OpenPanelType.Inventory)
				{
					_lastInventoryScrollPos = _mainScrollRect.content.anchoredPosition;
				}
				else
				{
					_lastMarketScrollPos = _mainScrollRect.content.anchoredPosition;
				}
			});
			_ = CalculateGrid();
			_uiScrollRectOcclusiion = _mainScrollRect.GetComponent<UiScrollRectOcclusiion>();
			FilterText.text = FillFilterText();
			RightPanel.SetActive(value: false);
			if (openPanel == OpenPanelType.Inventory)
			{
				OpenInventory();
			}
			else
			{
				OpenMarket();
			}
		}

		private void ChangeSelectedPart(EngineKitData engineKitData)
		{
			PartType partType = _managerUI.GetPartToMarket.PartType;
			PartData installedPartDataByPartType = engineKitData.GetInstalledPartDataByPartType(partType);
			if (installedPartDataByPartType != null)
			{
				_managerUI.SelectPartToMarket(installedPartDataByPartType);
			}
		}

		private async UniTaskVoid CalculateGrid()
		{
			await UniTask.Yield(this.GetCancellationTokenOnDestroy());
			ApplyGridCellSize();
		}

		private void ApplyGridCellSize()
		{
			if ((bool)_mainScrollRect && (bool)_mainScrollRect.content)
			{
				GridLayoutGroup component = _mainScrollRect.content.GetComponent<GridLayoutGroup>();
				if ((bool)component)
				{
					component.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
					component.constraintCount = Mathf.Max(1, CellCountInOneRow);
					component.cellSize = UIHelper.CalculateGridElementWidth(_mainScrollRect.content, component, CellCountInOneRow, 1);
					_lastGridContentWidth = _mainScrollRect.content.rect.width;
				}
			}
		}

		private void LateUpdate()
		{
			if ((bool)_mainScrollRect && (bool)_mainScrollRect.content && !(Mathf.Abs(_mainScrollRect.content.rect.width - _lastGridContentWidth) < 0.5f))
			{
				ApplyGridCellSize();
			}
		}

		public void InitMarketButtons(PartData partDataOnCar)
		{
			openPanel = OpenPanelType.Market;
			List<PartItem> list = ((partDataOnCar.PartType != PartType.TurboCharger && partDataOnCar.PartType != PartType.SuperCharger) ? _allParts.GetParts(partDataOnCar.PartType) : _allParts.GetParts(PartType.TurboCharger).Concat(_allParts.GetParts(PartType.SuperCharger)).ToList());
			Debug.Log($"All {partDataOnCar.PartType} parts: " + list.Count);
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == null)
				{
					Debug.LogError($"Part {partDataOnCar.PartType} index: {i} is null. Skip it.");
					continue;
				}
				if (list[i].IsTamperedConfig())
				{
					Debug.LogWarning($"[AntiCheat] Hiding tampered part from market: {list[i].TypeId} ModelId={list[i].ModelId}");
					continue;
				}
				PartData partData = list[i].CreateNewPartDataFromPart();
				if (list[i].IsNotForSale || list[i].IsCarExcluded(_carInfo.CarID))
				{
					continue;
				}
				bool result = (_isInCarEngine ? CheckIsNotCompatibleFull(partData, list[i], _managerUI.EditingEngineKitData.EngineId.ModelId) : CheckIsNotCompatible(partData, list[i], _managerUI.EditingEngineKitData.EngineId.ModelId));
				if (!result && partData.PartType == PartType.GearBox)
				{
					CheckGearboxNotCompatible(partData, out result);
					if (result)
					{
						continue;
					}
				}
				else if (((_managerUI.CurrentInventoryElement == MarketInventoryFilters.Compatible) & result) || (_managerUI.CurrentInventoryElement == MarketInventoryFilters.InCompatible && !result))
				{
					continue;
				}
				if (!string.IsNullOrEmpty(partData.GetName()))
				{
					num++;
					_prefabFactory.Instantiate(MarketInventoryItemPrefab, _mainScrollRect.content).Init(partData, this, result);
				}
			}
			FilterDetailCountText.text = num.ToString();
			ReloadLayout().Forget();
		}

		private async UniTaskVoid DisableCachedItems()
		{
			if ((bool)_mainScrollRect)
			{
				UnityEngine.Object.Destroy(_mainScrollRect.gameObject);
			}
		}

		private bool CheckIsNotCompatible(PartData partData, PartItem part, int engineModelId)
		{
			return part.CheckNotCompatibleEngine(engineModelId);
		}

		private bool CheckIsNotCompatibleFull(PartData partData, PartItem part, int engineModelId)
		{
			return part.CheckNotCompatibleFull(_carId, engineModelId);
		}

		public void InitInventoryButtons(PartData part, bool isPartPlaceEmpty)
		{
			if (part == null)
			{
				return;
			}
			openPanel = OpenPanelType.Inventory;
			List<PartData> list = new List<PartData>();
			if (part.ModelId != 0 && !isPartPlaceEmpty)
			{
				list.Add(part);
			}
			List<PartData> list2 = new List<PartData>();
			PartType partType = part.PartType;
			list2 = ((partType != PartType.TurboCharger && partType != PartType.SuperCharger) ? _inventoryPartsService.GetAllParts(part.PartType).ToList() : _inventoryPartsService.GetAllParts(PartType.TurboCharger).Concat(_inventoryPartsService.GetAllParts(PartType.SuperCharger)).ToList());
			list = list.Concat(list2).ToList();
			Debug.Log("All inventory: " + list.Count);
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].InstanceID == -1 || list[i].ModelId == 0)
				{
					continue;
				}
				PartItem part2 = _allParts.GetPart(list[i]);
				if (!part2)
				{
					_inventoryPartsService.Remove(list[i]);
					continue;
				}
				if (part2.IsTamperedConfig())
				{
					Debug.LogWarning($"[AntiCheat] Hiding tampered part from inventory: {part2.TypeId} ModelId={part2.ModelId}");
					continue;
				}
				bool result = (_isInCarEngine ? CheckIsNotCompatibleFull(list[i], part2, _managerUI.EditingEngineKitData.EngineId.ModelId) : CheckIsNotCompatible(list[i], part2, _managerUI.EditingEngineKitData.EngineId.ModelId));
				if (part2.CheckNotCompatibleCar(_carInfo.CarID))
				{
					result = true;
				}
				if (!result && list[i].PartType == PartType.GearBox)
				{
					CheckGearboxNotCompatible(list[i], out result);
				}
				if (!((_managerUI.CurrentInventoryElement == MarketInventoryFilters.Compatible) & result) && (_managerUI.CurrentInventoryElement != MarketInventoryFilters.InCompatible || result))
				{
					num++;
					_prefabFactory.Instantiate(MarketInventoryItemPrefab, _mainScrollRect.content).Init(list[i], this, result, part.InstanceID);
				}
			}
			FilterDetailCountText.text = num.ToString();
			_ = ReloadLayout();
		}

		private void CheckGearboxNotCompatible(PartData partData, out bool result)
		{
			GearboxType getGearboxType = _carInfo.GetDefaultTuningConfig().ChassisData.GetGearboxType;
			GearBoxPart part = _allParts.GetPart<GearBoxPart>(partData.PartType, partData.ModelId);
			bool flag = !getGearboxType.HasFlag((GearboxType)part.GetParams.Type);
			if (flag)
			{
				result = true;
				return;
			}
			if (_managerUI.CurrentGearBoxTypeFilters.Contains(GearBoxTypeFilters.All))
			{
				flag = false;
			}
			else
			{
				GearBoxTypeFilters item = GearBoxTypeFilterUtility.FromKind(part.GetParams.Kind);
				if (!_managerUI.CurrentGearBoxTypeFilters.Contains(item))
				{
					flag = true;
				}
			}
			if (_managerUI.CurrentGearBoxTransMissionTypes.Contains(GearBoxTransmissionTypeFilter.All) && !flag)
			{
				flag = false;
			}
			else
			{
				GearBoxTransmissionTypeFilter type = (GearBoxTransmissionTypeFilter)part.GetParams.Type;
				flag = ((!_managerUI.CurrentGearBoxTransMissionTypes.Contains(type) || flag) ? true : false);
			}
			result = flag;
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			_managerUI.ShowPrevWindow(fade: false);
			Close();
		}

		private void OpenMarket()
		{
			FilterText.text = FillFilterText();
			InitMarketButtons(_managerUI.GetPartToMarket);
		}

		private void OpenInventory()
		{
			FilterText.text = FillFilterText();
			PartData getPartToMarket = _managerUI.GetPartToMarket;
			switch (_managerUI.GetPartToMarket.PartType)
			{
			case PartType.Clutch:
				getPartToMarket = _carTuningData.EngineKitData.GetInstalledPartDataByPartType(_managerUI.GetPartToMarket.PartType);
				break;
			case PartType.GearBox:
				getPartToMarket = _carTuningData.GetInstalledPartDataByPartType(_managerUI.GetPartToMarket.PartType);
				break;
			case PartType.ECU:
				getPartToMarket = _carTuningData.GetInstalledPartDataByPartType(_managerUI.GetPartToMarket.PartType);
				break;
			case PartType.FrontLSD:
			case PartType.RearLSD:
				getPartToMarket = _carTuningData.GetInstalledPartDataByPartType(_managerUI.GetPartToMarket.PartType);
				break;
			default:
				getPartToMarket = _managerUI.EditingEngineKitData.GetInstalledPartDataByPartType(_managerUI.GetPartToMarket.PartType);
				break;
			}
			bool isPartPlaceEmpty = false;
			if (getPartToMarket == null)
			{
				getPartToMarket = _managerUI.GetPartToMarket;
				isPartPlaceEmpty = true;
			}
			InitInventoryButtons(getPartToMarket, isPartPlaceEmpty);
		}

		private void HideLeftPanel()
		{
		}

		public void OnItemClick(PartData parData, int index, UIElement uiElement = null)
		{
			PartItem part = _allParts.GetPart(parData);
			_currentPart = parData;
			currentElmnt.Init(parData, this);
			currentElmnt.EnableAlphaCanvasGroup(enable: true);
			switch (openPanel)
			{
			case OpenPanelType.Market:
				BuyButton.gameObject.SetActive(value: true);
				SetButton.gameObject.SetActive(value: false);
				SellButton.gameObject.SetActive(value: false);
				break;
			case OpenPanelType.Inventory:
			{
				BuyButton.gameObject.SetActive(value: false);
				bool flag = parData.InstanceID == _managerUI.GetPartToMarket.InstanceID;
				SetButton.gameObject.SetActive(!flag);
				SellButton.gameObject.SetActive(!flag);
				if (part.CheckNotCompatibleFull(_carId, _engineModel))
				{
					SetButton.gameObject.SetActive(value: false);
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public void CloseClickable()
		{
			Close();
		}

		public void OnBuyAndSetClick(PartData partData, PartItem part)
		{
			PartData partData2 = _managerUI.GetPartToMarket;
			bool flag = partData2.InstanceID != -1 && partData2.ModelId != 0 && partData != null;
			if (!flag || (flag && partData.PartType == PartType.GearBox))
			{
				partData2 = CheckOtherParts(partData);
				flag = partData2 != null;
			}
			if (flag)
			{
				PartItem part2 = partData2.GetPart(_allParts);
				MarketInventoryPopup.ShowPopup(partData2, part2, partData, part, OnBuyAndSetConfirm);
			}
			else
			{
				MarketInventoryPopup.ShowPopup(partData, part, OnBuyAndSetConfirm, new LocalizedString("Marketinventorywindow.buy&set"));
			}
		}

		public void OnBuyClick(PartData partData, PartItem part)
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_InventoryItem(part.GetName, part.GetPrice, part.TypeId.ToString(), PurchaseStatus.PURCHASE_CLICKED);
			MarketInventoryPopup.ShowPopup(partData, part, OnBuyConfirm, new LocalizedString("Marketinventorywindow.BUY"));
		}

		private void OnBuyAndSetConfirm()
		{
			PartItem part = _allParts.GetPart<PartItem>(_currentPart);
			AdvancedInt price = new AdvancedInt(part.GetPrice);
			_currencyManagerController.TryMoneySpend(out var result, price);
			if (result)
			{
				_ = _managerUI.GetPartToMarketIndex;
				_tuningFactory.BuyPart(_currentPart);
				string text = new LocalizedString("MarketInventoryWindow.YouBoughtPartGetname");
				_ = _managerUI.ShowNotificationWindow(text.Replace("{part.GetName}", part.GetName), 3f);
				OnSetConfirm();
				_virtualPurchaseAnalytic.LogPurchaseVirtual_InventoryItem(part.GetName, part.GetPrice, part.TypeId.ToString(), PurchaseStatus.PURCHASE_COMPLETED);
			}
			else
			{
				_ = _managerUI.ShowNotificationWindow(new LocalizedString("MarketInventoryWindow.NoMoney!"), 3f, NotificationType.Error);
				_virtualPurchaseAnalytic.LogPurchaseVirtual_InventoryItem(part.GetName, part.GetPrice, part.TypeId.ToString(), PurchaseStatus.INSUFFICIENT_FUNDS);
			}
			MarketInventoryPopup.SetActive(active: false);
			MarketInventoryShowCasePanel.gameObject.SetActive(value: true);
		}

		private void OnBuyConfirm()
		{
			PartItem part = _allParts.GetPart<PartItem>(_currentPart);
			AdvancedInt price = new AdvancedInt(part.GetPrice);
			_currencyManagerController.TryMoneySpend(out var result, price);
			if (result)
			{
				_ = _managerUI.GetPartToMarketIndex;
				_tuningFactory.BuyPart(_currentPart);
				string text = new LocalizedString("MarketInventoryWindow.YouBoughtPartGetname");
				_ = _managerUI.ShowNotificationWindow(text.Replace("{part.GetName}", part.GetName), 3f);
				ReloadWindow();
				_virtualPurchaseAnalytic.LogPurchaseVirtual_InventoryItem(part.GetName, part.GetPrice, part.TypeId.ToString(), PurchaseStatus.PURCHASE_COMPLETED);
			}
			else
			{
				_ = _managerUI.ShowNotificationWindow(new LocalizedString("MarketInventoryWindow.NoMoney!"), 3f, NotificationType.Error);
				_virtualPurchaseAnalytic.LogPurchaseVirtual_InventoryItem(part.GetName, part.GetPrice, part.TypeId.ToString(), PurchaseStatus.INSUFFICIENT_FUNDS);
			}
			MarketInventoryShowCasePanel.gameObject.SetActive(value: true);
			MarketInventoryPopup.SetActive(active: false);
		}

		public void HideMainPanel()
		{
			MarketInventoryShowCasePanel.gameObject.SetActive(value: false);
		}

		public void OnCancelPopup()
		{
			MarketInventoryShowCasePanel.gameObject.SetActive(value: true);
		}

		private void OnSellConfirm()
		{
			Debug.Log("Try sell ");
			PartItem part = _allParts.GetPart<PartItem>(_currentPart);
			MarketInventoryShowCasePanel.gameObject.SetActive(value: true);
			MarketInventoryPopup.SetActive(active: false);
			ObscuredInt obscuredInt = Mathf.RoundToInt(_currentPart.GetCondition() * (float)(int)part.GetPrice * 0.2f);
			obscuredInt = Mathf.Clamp(obscuredInt, 1, 2000);
			_menuWindowsMediator.RemovePart(_currentPart);
			_currencyManagerController.AddMoney(new AdvancedInt(obscuredInt));
			ReloadWindow();
		}

		public void OnSellClick(PartData partData, PartItem part)
		{
			MarketInventoryPopup.ShowPopup(partData, part, OnSellConfirm, new LocalizedString("Marketinventorywindow.Sell"));
		}

		private void ResetDynamicTuningValues(PartType partType)
		{
			switch (partType)
			{
			case PartType.GearBox:
				_carInfo.TuningData.DynamicTuningSettings.FinalDrive = -1f;
				_carInfo.TuningData.DynamicTuningSettings.Gears = Enumerable.Repeat(-1f, 14).ToArray();
				_carInfo.TuningData.DynamicTuningSettings.LaunchRpm = -1f;
				_carInfo.TuningData.DynamicTuningSettings.LaunchTime = -1f;
				_carInfo.TuningData.DynamicTuningSettings.TorqueSplit = -1f;
				break;
			case PartType.ECU:
				_carInfo.TuningData.DynamicTuningSettings.IdleRpm = -1f;
				_carInfo.TuningData.DynamicTuningSettings.RedlineRpm = -1f;
				break;
			case PartType.FuelRail:
				_carInfo.TuningData.DynamicTuningSettings.MixtureRatio = -1f;
				break;
			case PartType.TurboCharger:
				_carInfo.TuningData.DynamicTuningSettings.Boost = -1f;
				break;
			case PartType.FrontLSD:
				_carInfo.TuningData.DynamicTuningSettings.FrontAccelBias = -1f;
				_carInfo.TuningData.DynamicTuningSettings.FrontBrakeBias = -1f;
				_carInfo.TuningData.DynamicTuningSettings.FrontMaxBias = -1f;
				_carInfo.TuningData.DynamicTuningSettings.FrontTorqueBiasRatio = -1f;
				_carInfo.TuningData.DynamicTuningSettings.FrontLsdLock = -1f;
				break;
			case PartType.RearLSD:
				_carInfo.TuningData.DynamicTuningSettings.RearAccelBias = -1f;
				_carInfo.TuningData.DynamicTuningSettings.RearBrakeBias = -1f;
				_carInfo.TuningData.DynamicTuningSettings.RearMaxBias = -1f;
				_carInfo.TuningData.DynamicTuningSettings.RearTorqueBiasRatio = -1f;
				_carInfo.TuningData.DynamicTuningSettings.RearLsdLock = -1f;
				break;
			}
			_carInfo.Save();
		}

		private void OnSetConfirm()
		{
			MarketInventoryPopup.gameObject.SetActive(value: false);
			MarketInventoryShowCasePanel.gameObject.SetActive(value: true);
			ResetDynamicTuningValues(_currentPart.PartType);
			if (_isInCarEngine)
			{
				switch (_timerTuningService.TryInstallPart(V.SlotID, _currentPart))
				{
				case TimerInstallResult.DuplicateType:
					_ = _managerUI.ShowNotificationWindow(new LocalizedString("MarketInventoryWindow.PartTypeAlreadyInService"), 3f);
					break;
				default:
					ShowBusyMessage();
					break;
				case TimerInstallResult.Started:
				case TimerInstallResult.Extended:
					break;
				}
				ReloadWindow();
				return;
			}
			PartType partType = _currentPart.PartType;
			if (partType == PartType.GearBox || partType == PartType.ECU)
			{
				switch (_timerTuningService.TryInstallPart(V.SlotID, _currentPart))
				{
				case TimerInstallResult.DuplicateType:
					_ = _managerUI.ShowNotificationWindow(new LocalizedString("MarketInventoryWindow.PartTypeAlreadyInService"), 3f);
					break;
				default:
					_ = _managerUI.ShowNotificationWindow(new LocalizedString("MarketInventoryWindow.Thisengineisintheservicenow.Pleasewait."), 3f);
					break;
				case TimerInstallResult.Started:
				case TimerInstallResult.Extended:
					break;
				}
				ReloadWindow();
			}
			else if (_engineTuningTimerService.CheckEngineBlocked(_managerUI.EditingEngineKitData.EngineId.InstanceID))
			{
				ShowBusyMessage();
			}
			else
			{
				_engineTuningTimerService.CreateTimer(_managerUI.EditingEngineKitData.EngineId.InstanceID, _currentPart);
				ReloadWindow();
			}
		}

		private void ShowBusyMessage()
		{
			string text = ((openPanel == OpenPanelType.Market) ? new LocalizedString("MarketInventoryWindow.Thisengineisintheservicenow.Detail") : new LocalizedString("MarketInventoryWindow.Thisengineisintheservicenow.Pleasewait."));
			_ = _managerUI.ShowNotificationWindow(text, 3f);
		}

		private PartData CheckOtherParts(PartData partData)
		{
			return partData.PartType switch
			{
				PartType.ECU => _carTuningData.ECU, 
				PartType.GearBox => _carTuningData.GearBox, 
				_ => null, 
			};
		}

		public void OnSetClick(PartData partData, PartItem part)
		{
			PartData partData2 = _managerUI.GetPartToMarket;
			bool flag = partData2.InstanceID != -1 && partData != null;
			if (!flag || partData.PartType == PartType.GearBox)
			{
				partData2 = CheckOtherParts(partData);
				flag = partData2 != null;
			}
			if (flag)
			{
				PartItem part2 = partData2.GetPart(_allParts);
				MarketInventoryPopup.ShowPopup(partData2, part2, partData, part, OnSetConfirm, isFromInventory: true);
			}
			else
			{
				MarketInventoryPopup.ShowPopup(partData, part, OnSetConfirm, new LocalizedString("Marketinventorywindow.Set"));
			}
		}

		private string FillFilterText()
		{
			string text = "";
			if (_managerUI.GetPartToMarket.PartType == PartType.GearBox)
			{
				foreach (GearBoxTypeFilters currentGearBoxTypeFilter in _managerUI.CurrentGearBoxTypeFilters)
				{
					text = string.Concat(text, new LocalizedString(GearBoxTypeFilterUtility.LocalizationKey(currentGearBoxTypeFilter)), ", ");
				}
				foreach (GearBoxTransmissionTypeFilter currentGearBoxTransMissionType in _managerUI.CurrentGearBoxTransMissionTypes)
				{
					text = ((currentGearBoxTransMissionType != GearBoxTransmissionTypeFilter.All) ? (text + currentGearBoxTransMissionType.ToString() + ", ") : string.Concat(text, new LocalizedString("Gearboxfilterwindow.All"), ", "));
				}
				return text.Remove(text.Length - 2);
			}
			return Utils.GetLocalizedMarketFilter(_managerUI.CurrentInventoryElement);
		}

		public void OnDetailClick(PartData partData, bool isInstalled, PartItem part, Vendor vendor, bool isNotCompatible)
		{
			RightPanel.SetActive(value: true);
			_currentPart = partData;
			MarketInventoryShowCasePanel.Init(partData, part, vendor, _isInCarEngine, isInstalled, openPanel, isNotCompatible);
		}
	}
}
