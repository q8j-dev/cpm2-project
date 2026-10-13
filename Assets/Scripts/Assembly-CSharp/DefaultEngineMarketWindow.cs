using System.Collections.Generic;
using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Inventory;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;

public class DefaultEngineMarketWindow : WindowScript, IBackableWindow
{
	public const float ECUDiscount = 0.8f;

	private IPrefabFactory _prefabFactory;

	private CarTuningFactory _carTuningFactory;

	private ICarInfoService _carInfoService;

	private CarInfo _carInfo;

	private AllParts _allParts;

	[SerializeField]
	private RectTransform ItemsParent = default;

	[SerializeField]
	private DefaultEngineInventoryUIElement DefaultEngineInventoryUIElementPrefab = default;

	[SerializeField]
	private Button FilterButton = default;

	[SerializeField]
	private Toggle MarketToggle = default;

	[SerializeField]
	private Toggle InventoryToggle = default;

	[SerializeField]
	private EngineMarketInventoryShowcaseWindow EngineMarketInventoryShowcaseWindow = default;

	[SerializeField]
	private EngineMarketInventoryPopupWindow EngineMarketInventoryPopupWindow = default;

	[SerializeField]
	private TextMeshProUGUI SelectedFilterText = default;

	[SerializeField]
	private TextMeshProUGUI SelectedFilterDetailsCount = default;

	[SerializeField]
	private ScrollRect InventoryScrollRectPrefab = default;

	[SerializeField]
	private Transform ScrollRectPlace = default;

	[SerializeField]
	private int CellCountInOneRow = 1;

	private float _lastGridContentWidth = -1f;

	[SerializeField]
	private GameObject MainPanel = default;

	private EngineItem _selectedEngine;

	private EngineKitData _selectedEngineKit;

	private EngineKitOffer _selectedOffer;

	private ITimerTuningService _timerTuningService;

	private bool _isInventoryMenuActive;

	private IEngineKitsInventoryService _engineKitsInventoryService;

	private IInventoryPartsService _inventoryPartsService;

	public Color InstalledColor;

	public Color CompatibleColor;

	public Color NotCompatibleColor;

	private ScrollRect _mainScrollRect;

	private UiScrollRectOcclusiion _uiScrollRectOcclusiion;

	private CurrencyManagerController _currencyManagerController;

	private MarketInventoryWindow.OpenPanelType _openPanel = MarketInventoryWindow.OpenPanelType.Market;

	private Vector2 _lastMarketScrollPos;

	private Vector2 _lastInventoryScrollPos;

	private IEngineTuningTimerService _engineTuningTimerService;

	private string _priceText;

	private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

	private IDatabaseSaveLoadService _databaseSaveLoadService;

	public EngineItem SelectedEngineItem => _selectedEngine;

	public CarInfo CurrentCarInfo => _carInfo;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[Inject]
	public void Construct(IPrefabFactory prefabFactory, ICarTuningFactory carTuningFactory, ICarInfoService carInfoService, AllParts allParts, ManagerUI managerUI, ITimerTuningService timerTuningService, IEngineKitsInventoryService engineKitsInventoryService, CurrencyManagerController currencyManagerController, IEngineTuningTimerService engineTuningTimerService, IInventoryPartsService inventoryPartsService, VirtualPurchaseAnalytic virtualPurchaseAnalytic, IDatabaseSaveLoadService databaseSaveLoadService)
	{
		_databaseSaveLoadService = databaseSaveLoadService;
		_engineTuningTimerService = engineTuningTimerService;
		_timerTuningService = timerTuningService;
		_prefabFactory = prefabFactory;
		_carTuningFactory = carTuningFactory as CarTuningFactory;
		_carInfoService = carInfoService;
		_allParts = allParts;
		_managerUI = managerUI;
		_engineKitsInventoryService = engineKitsInventoryService;
		_currencyManagerController = currencyManagerController;
		_inventoryPartsService = inventoryPartsService;
		_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
	}

	protected override void Start()
	{
		base.Start();
		InitBackButton();
		FilterButton.onClick.AddListener(OpenFilter);
		MarketToggle.SetIsOnWithoutNotify(value: true);
		MarketToggle.onValueChanged.AddListener(OnMarketPressed);
		InventoryToggle.onValueChanged.AddListener(OnInventoryPressed);
		InitMarket();
	}

	private void OpenFilter()
	{
		ShowFilterSuspendNavigation().Forget();
	}

	private async UniTask ShowFilterSuspendNavigation()
	{
		await _managerUI.ShowEngineFilterWindow();
	}

	public override void RebuildWindow()
	{
		ReloadWindow();
	}

	public override void ReloadWindow()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		if (_managerUI.EditingEngineKitData == null || _managerUI.EditingEngineKitData.EngineId.InstanceID == -1)
		{
			_managerUI.EditingEngineKitData = _carInfo.TuningData.EngineKitData;
		}
		EngineMarketInventoryShowcaseWindow.Panel.SetActive(value: false);
		if (_isInventoryMenuActive)
		{
			InitInventory();
		}
		else
		{
			InitMarket();
		}
	}

	private void CreateScrollRect()
	{
		_ = ResetCachedEngineItems();
		_mainScrollRect = _prefabFactory.Instantiate(InventoryScrollRectPrefab, ScrollRectPlace);
		_mainScrollRect.onValueChanged.AddListener((Vector2 _) =>
		{
			if (_isInventoryMenuActive)
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
	}

	public void SetPriceText(string price)
	{
		_priceText = price;
	}

	public void HideMainPanel()
	{
		EngineMarketInventoryShowcaseWindow.Panel.SetActive(value: false);
	}

	public void OnCancelPopup()
	{
		ChangePanelState(state: true);
		EngineMarketInventoryShowcaseWindow.Panel.SetActive(value: true);
	}

	public void InitMarket()
	{
		_openPanel = MarketInventoryWindow.OpenPanelType.Market;
		_isInventoryMenuActive = false;
		CreateScrollRect();
		EngineMarketInventoryShowcaseWindow.Panel.gameObject.SetActive(value: false);
		FillFilterText();
		ToggleGroup component = _mainScrollRect.content.gameObject.GetComponent<ToggleGroup>();
		if (!_carInfoService.TryGetFromSlot(V.SlotID, out var carInfo))
		{
			Debug.LogError($"[DefaultEngineMarketWindow] Slot {V.SlotID} is empty/invalid — aborting InitMarket. No engine offers will be shown.", this);
			return;
		}
		_carInfo = carInfo;
		List<EngineKitOffer> engineKitOffers = _allParts.EngineKitOffers;
		int num = 0;
		for (int i = 0; i < engineKitOffers.Count; i++)
		{
			EngineKitOffer engineKitOffer = engineKitOffers[i];
			if (engineKitOffer == null || engineKitOffer.IsNotForSale)
			{
				continue;
			}
			if (engineKitOffer.Base == null)
			{
				Debug.LogError("EngineKitOffer '" + engineKitOffer.name + "' has no Base — skipped.", engineKitOffer);
				continue;
			}
			EngineItem engineItem = engineKitOffer.Base;
			if (engineItem.IsCarExcluded(_carInfo.CarID) || (engineItem.CarLinkCount > 0 && !engineItem.IsCarLinked(_carInfo.CarID)))
			{
				continue;
			}
			EngineType getEngineType = _carInfo.GetDefaultTuningConfig().ChassisData.GetEngineType;
			bool flag = engineItem.HasFlag(getEngineType);
			if ((_managerUI.CurrentInventoryElement != MarketInventoryFilters.Compatible || flag) && !((_managerUI.CurrentInventoryElement == MarketInventoryFilters.InCompatible) & flag))
			{
				int getTypeEngine = (int)engineItem.GetTypeEngine;
				if (_managerUI.CurrentEngineFilterTypeFilters.Contains(EngineFilterTypeFilter.All) || _managerUI.CurrentEngineFilterTypeFilters.Contains((EngineFilterTypeFilter)getTypeEngine))
				{
					num++;
					_prefabFactory.Instantiate(DefaultEngineInventoryUIElementPrefab, _mainScrollRect.content).InitForMarket(engineKitOffer, this, flag, component, _carInfo.CarID);
				}
			}
		}
		SelectedFilterDetailsCount.text = num.ToString();
		RebuildLayout().Forget();
	}

	private async UniTaskVoid CalculateGrid()
	{
		await UniTask.Yield(this.GetCancellationTokenOnDestroy());
		ApplyGridCellSize();
	}

	private async UniTaskVoid RebuildLayout()
	{
		if ((bool)ItemsParent)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(ItemsParent);
		}
		_mainScrollRect.content.anchoredPosition = (_isInventoryMenuActive ? _lastInventoryScrollPos : _lastMarketScrollPos);
		await UniTask.Yield(this.GetCancellationTokenOnDestroy());
		ApplyGridCellSize();
		_uiScrollRectOcclusiion.Init();
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

	private async UniTaskVoid ResetCachedEngineItems()
	{
		if ((bool)_mainScrollRect)
		{
			Object.Destroy(_mainScrollRect.gameObject);
		}
	}

	private void InitInventory()
	{
		EngineMarketInventoryShowcaseWindow.Panel.SetActive(value: false);
		_openPanel = MarketInventoryWindow.OpenPanelType.Inventory;
		_isInventoryMenuActive = true;
		CreateScrollRect();
		ToggleGroup component = _mainScrollRect.content.gameObject.GetComponent<ToggleGroup>();
		FillFilterText();
		_prefabFactory.Instantiate(DefaultEngineInventoryUIElementPrefab, _mainScrollRect.content).InitForInventory(_carInfo.TuningData.EngineKitData, this, component, isCompatible: true, isInstalled: true);
		List<EngineKitData> allEngines = _engineKitsInventoryService.GetAllEngines();
		int num = 0;
		for (int i = 0; i < allEngines.Count; i++)
		{
			EngineItem engine = allEngines[i].GetEngine();
			EngineType getTypeEngine = engine.GetTypeEngine;
			EngineType getEngineType = _carInfo.GetDefaultTuningConfig().ChassisData.GetEngineType;
			bool flag = getEngineType.HasFlag(getTypeEngine);
			if (engine.IsCarExcluded(_carInfo.CarID) || (engine.CarLinkCount > 0 && !engine.IsCarLinked(_carInfo.CarID)))
			{
				continue;
			}
			switch (_managerUI.CurrentInventoryElement)
			{
			case MarketInventoryFilters.Compatible:
				if (!flag)
				{
					continue;
				}
				break;
			case MarketInventoryFilters.InCompatible:
				if (flag)
				{
					continue;
				}
				break;
			}
			int getTypeEngine2 = (int)engine.GetTypeEngine;
			if (_managerUI.CurrentEngineFilterTypeFilters.Contains(EngineFilterTypeFilter.All) || _managerUI.CurrentEngineFilterTypeFilters.Contains((EngineFilterTypeFilter)getTypeEngine2))
			{
				num++;
				_engineKitsInventoryService.IsInInventory(allEngines[i]);
				_prefabFactory.Instantiate(DefaultEngineInventoryUIElementPrefab, _mainScrollRect.content).InitForInventory(allEngines[i], this, component, flag);
			}
		}
		SelectedFilterDetailsCount.text = num.ToString();
		RebuildLayout().Forget();
	}

	public void OnItemClickMarket(EngineKitOffer offer, bool isCompatible)
	{
		_selectedOffer = offer;
		_selectedEngine = ((offer != null) ? offer.Base : null);
		EngineMarketInventoryShowcaseWindow.Init(offer, isCompatible);
		EngineMarketInventoryShowcaseWindow.Panel.SetActive(value: true);
	}

	public void OnInventoryButtonPressed(EngineKitData engineKitData, EngineItem engine, bool isCompatible, bool isInstalled = false)
	{
		_selectedEngineKit = engineKitData;
		EngineMarketInventoryShowcaseWindow.Init(engineKitData, engine, isCompatible, isInstalled);
		EngineMarketInventoryShowcaseWindow.Panel.SetActive(value: true);
	}

	public void OnBuyAndSetClick()
	{
		if (_timerTuningService.IsCarBlocked(V.SlotID))
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("DefaultEngineMarketWindow.Carisinservicenow.Wait."), 3f).Forget();
			return;
		}
		_virtualPurchaseAnalytic.LogPurchaseVirtual_Engine(_selectedEngine.EngineId.GetName, _selectedEngine.GetPrice, PurchaseStatus.PURCHASE_CLICKED);
		ChangePanelState(state: false);
		EngineMarketInventoryPopupWindow.BuyAndSetEngine(_managerUI.EditingEngineKitData, _selectedOffer, OnBuyConfirm, _priceText, _virtualPurchaseAnalytic);
	}

	public void OnBuyClick()
	{
		ChangePanelState(state: false);
		_virtualPurchaseAnalytic.LogPurchaseVirtual_Engine(_selectedEngine.EngineId.GetName, _selectedEngine.GetPrice, PurchaseStatus.PURCHASE_CLICKED);
		EngineMarketInventoryPopupWindow.BuyEngine(_selectedOffer, BuyWithoutInstallConfirm, _priceText, _virtualPurchaseAnalytic);
	}

	private void ChangePanelState(bool state)
	{
		MainPanel.SetActive(state);
	}

	public void OnSetClick()
	{
		if (_timerTuningService.IsCarBlocked(V.SlotID))
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("DefaultEngineMarketWindow.Carisinservicenow.Wait."), 3f).Forget();
			ChangePanelState(state: true);
		}
		else if (_engineTuningTimerService.CheckEngineBlocked(_selectedEngineKit.EngineId.InstanceID))
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("DefaultEngineMarketWindow.Thisengineisintheservicenow.Wait."), 3f).Forget();
			ChangePanelState(state: true);
		}
		else
		{
			ChangePanelState(state: false);
			EngineMarketInventoryPopupWindow.SetEngine(_managerUI.EditingEngineKitData, _selectedEngineKit, OnBuyFromInventoryConfirm);
		}
	}

	public void SellEngine()
	{
		ChangePanelState(state: false);
		EngineMarketInventoryPopupWindow.SellEngine(_selectedEngineKit, OnSellConfirm, _priceText);
	}

	public void OnSellConfirm()
	{
		ChangePanelState(state: true);
		_currencyManagerController.AddMoney(_selectedEngineKit.GetPrize(_allParts));
		_engineKitsInventoryService.Remove(_selectedEngineKit);
		string text = new LocalizedString("DefaultEngineMarketWindow.EngineSelectedenginekitEnginei");
		_ = _managerUI.ShowNotificationWindow(text.Replace("{0}", _selectedEngineKit.EngineId.GetName()), 3f);
		InitInventory();
	}

	private void OnBuyConfirm()
	{
		ChangePanelState(state: true);
		EngineKitData engineKit = _selectedOffer.Base.GetEngineKit();
		engineKit.ApplyStockExtras(_selectedOffer.Extras, _selectedOffer.name);
		List<PartType> enginePartsNotCopmatibleToCar = _carTuningFactory.GetEnginePartsNotCopmatibleToCar(engineKit, _carInfo.CarID);
		EngineType getTypeEngine = (engineKit.GetEngine() ?? _selectedOffer.Base).GetTypeEngine;
		EngineType getEngineType = _carInfo.GetDefaultTuningConfig().ChassisData.GetEngineType;
		if (!getEngineType.HasFlag(getTypeEngine))
		{
			string text = string.Format(new LocalizedString("DefaultEngineMarketWindow.YouCanTInstallSelectedengineGe"), _selectedOffer.Base.GetTypeEngine);
			_managerUI.ShowNotificationWindow(text, 3f, NotificationType.Error).Forget();
			return;
		}
		if (enginePartsNotCopmatibleToCar.Count > 0)
		{
			string text2 = "";
			for (int i = 0; i < enginePartsNotCopmatibleToCar.Count; i++)
			{
				string text3 = ((i == 0) ? "" : ", ");
				text2 = text2 + text3 + enginePartsNotCopmatibleToCar[i];
			}
			string text4 = new LocalizedString("DefaultEngineMarketWindow.EngineSelectedengineEngineidGe");
			_ = _managerUI.ShowNotificationWindow(text4.Replace("{0}", _selectedOffer.Base.EngineId.GetName).Replace("{partsNames}", text2), 8f, NotificationType.Error);
			return;
		}
		ECUPart effectiveECU = _selectedOffer.GetEffectiveECU(_allParts, _carInfo.CarID);
		if (!effectiveECU)
		{
			_ = _managerUI.ShowNotificationWindow(new LocalizedString("DefaultEngineMarketWindow.YouCanTInstallSelectedengineGe"), 3f, NotificationType.Error);
			return;
		}
		_selectedOffer.GetFullPrice(_allParts, _carInfo.CarID, out var total);
		_currencyManagerController.TryMoneySpend(out var result, total);
		if (result)
		{
			PushWalletToBackend();
			_timerTuningService.TryCreateTimer(V.SlotID, engineKit, effectiveECU.CreateNewPartDataFromPart()).Get(out var value);
			_managerUI.EditingEngineKitData = null;
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Engine(_selectedOffer.Base.EngineId.GetName, _selectedOffer.Base.GetPrice, PurchaseStatus.PURCHASE_COMPLETED);
			if (value)
			{
				ShowTuningTimerWindow();
			}
		}
		else
		{
			_ = _managerUI.ShowNotificationWindow(new LocalizedString("DefaultEngineMarketWindow.Youdon'thaveenoughmoney!"), 3f, NotificationType.Error);
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Engine(_selectedOffer.Base.EngineId.GetName, _selectedOffer.Base.GetPrice, PurchaseStatus.INSUFFICIENT_FUNDS);
		}
	}

	private void ShowTuningTimerWindow()
	{
		_managerUI.ShowWindow(WindowScriptType.TuningTimerWindow.ToString(), alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false, null, withAnimate: false, updateBackButton: false).Forget();
	}

	public void BuyWithoutInstallConfirm()
	{
		ChangePanelState(state: true);
		ECUPart effectiveECU = _selectedOffer.GetEffectiveECU(_allParts, _carInfo.CarID);
		if (!effectiveECU)
		{
			_ = _managerUI.ShowNotificationWindow(new LocalizedString("DefaultEngineMarketWindow.YouCanTInstallSelectedengineGe"), 3f, NotificationType.Error);
			return;
		}
		_selectedOffer.GetFullPrice(_allParts, _carInfo.CarID, out var total);
		_currencyManagerController.TryMoneySpend(out var result, total);
		if (result)
		{
			PushWalletToBackend();
			EngineKitData engineKit = _selectedOffer.Base.GetEngineKit();
			engineKit.ApplyStockExtras(_selectedOffer.Extras, _selectedOffer.name);
			_inventoryPartsService.Add(effectiveECU.CreateNewPartDataFromPart());
			_engineKitsInventoryService.Add(engineKit);
			string text = new LocalizedString("DefaultEngineMarketWindow.EngineSelectedengineNameWasBou");
			_ = _managerUI.ShowNotificationWindow(text.Replace("{0}", _selectedOffer.GetDisplayName()), 3f);
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Engine(_selectedOffer.Base.EngineId.GetName, _selectedOffer.Base.GetPrice, PurchaseStatus.PURCHASE_COMPLETED);
		}
		else
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Engine(_selectedOffer.Base.EngineId.GetName, _selectedOffer.Base.GetPrice, PurchaseStatus.INSUFFICIENT_FUNDS);
			_ = _managerUI.ShowNotificationWindow(new LocalizedString("DefaultEngineMarketWindow.Youdon'thaveenoughmoney!"), 3f, NotificationType.Error);
		}
	}

	private void PushWalletToBackend()
	{
		_databaseSaveLoadService.SendWalletData(new WalletData(CurrencyManagerController.Money)).Forget();
	}

	public void OnInventoryPressed(bool state)
	{
		if (state && _openPanel != MarketInventoryWindow.OpenPanelType.Inventory)
		{
			InitInventory();
		}
	}

	public void OnMarketPressed(bool state)
	{
		if (state && _openPanel != MarketInventoryWindow.OpenPanelType.Market)
		{
			InitMarket();
		}
	}

	public void OnBuyFromInventoryConfirm()
	{
		ChangePanelState(state: true);
		List<PartType> enginePartsNotCopmatibleToCar = _carTuningFactory.GetEnginePartsNotCopmatibleToCar(_selectedEngineKit, _carInfo.CarID);
		EngineType getTypeEngine = _selectedEngineKit.GetEngine().GetTypeEngine;
		EngineType getEngineType = _carInfo.GetDefaultTuningConfig().ChassisData.GetEngineType;
		if (!getEngineType.HasFlag(getTypeEngine))
		{
			string text = new LocalizedString("DefaultEngineMarketWindow.YouCanTInstallSelectedengineGe");
			_managerUI.ShowNotificationWindow(text.Replace("{0}", _selectedEngine.GetTypeEngine.ToString()), 3f, NotificationType.Error).Forget();
		}
		else if (enginePartsNotCopmatibleToCar.Count > 0)
		{
			string text2 = "";
			for (int i = 0; i < enginePartsNotCopmatibleToCar.Count; i++)
			{
				string text3 = ((i == 0) ? "" : ", ");
				text2 = text2 + text3 + enginePartsNotCopmatibleToCar[i];
			}
			string text4 = new LocalizedString("DefaultEngineMarketWindow.EngineSelectedenginekitEnginei");
			_ = _managerUI.ShowNotificationWindow(text4.Replace("{0}", _selectedEngine.EngineId.GetName).Replace("{partsNames}", text2), 8f, NotificationType.Error);
		}
		else
		{
			_managerUI.EditingEngineKitData = null;
			_timerTuningService.TryCreateTimer(V.SlotID, _selectedEngineKit).Get(out var value);
			if (!value)
			{
				string text5 = ((_openPanel == MarketInventoryWindow.OpenPanelType.Market) ? new LocalizedString("DefaultEngineMarketWindow.ThisEngineIsInTheServiceNowDet") : new LocalizedString("DefaultEngineMarketWindow.ThisEngineIsInTheServiceNowPle"));
				_ = _managerUI.ShowNotificationWindow(text5, 3f);
			}
			else
			{
				ShowTuningTimerWindow();
			}
			ReloadWindow();
		}
	}

	public void OnConfigurateEngine()
	{
		_managerUI.EditingEngineKitData = _selectedEngineKit;
		_managerUI.ShowEngineUIWindow();
		Close();
	}

	public override void Close()
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

	private void FillFilterText()
	{
		string text = Utils.GetLocalizedMarketFilter(_managerUI.CurrentInventoryElement) + ", ";
		foreach (EngineFilterTypeFilter currentEngineFilterTypeFilter in _managerUI.CurrentEngineFilterTypeFilters)
		{
			text = ((currentEngineFilterTypeFilter != EngineFilterTypeFilter.All) ? (text + currentEngineFilterTypeFilter.ToString() + ", ") : string.Concat(text, new LocalizedString("Enginefilterwindow.All"), ", "));
		}
		text = text.Remove(text.Length - 2);
		SelectedFilterText.text = text;
	}

	public void Back()
	{
		_managerUI.EditingEngineKitData = null;
		Close();
	}
}
