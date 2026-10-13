using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

public class DefaultEngineInventoryUIElement : UIElement
{
	[SerializeField]
	private TextMeshProUGUI TitleText;

	[SerializeField]
	private TextMeshProUGUI MilliageText;

	[SerializeField]
	private TextMeshProUGUI MilliageTitleText;

	[SerializeField]
	private TextMeshProUGUI MassText;

	[SerializeField]
	private TextMeshProUGUI PriceText;

	[SerializeField]
	private TextMeshProUGUI MaxTorqueText;

	[SerializeField]
	private TextMeshProUGUI CompatibleText;

	[SerializeField]
	private CanvasGroup CanvasGroup;

	[SerializeField]
	private Toggle SelectButton;

	[SerializeField]
	private Image StatusEngine;

	[SerializeField]
	private Image StatusPlate;

	[SerializeField]
	private Image ItemImagePanel;

	[SerializeField]
	private Image CompanyImage;

	private DefaultEngineMarketWindow _defaultEngineMarketWindow;

	private EngineItem _engine;

	private EngineKitOffer _offer;

	private EngineKitData _engineKitData;

	private EngineInstance _engineInstance;

	private bool _isCompatible;

	private bool _isInstalled;

	private ToggleGroup _toggleGroup;

	private AllParts _allParts;

	private CarTuningDetailRaritySpritesConfig _carTuningDetailRaritySpritesConfig;

	private DetailVendorsConfigs _detailVendorsConfigs;

	[Inject]
	public void Construct(AllParts allParts, CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig, DetailVendorsConfigs detailVendorsConfigs)
	{
		_allParts = allParts;
		_carTuningDetailRaritySpritesConfig = carTuningDetailRaritySpritesConfig;
		_detailVendorsConfigs = detailVendorsConfigs;
	}

	protected override void OnEnable()
	{
		SelectButton.group = _toggleGroup;
		base.OnEnable();
	}

	public void SetVisible(int state)
	{
		CanvasGroup.alpha = state;
	}

	public void InitForMarket(EngineKitOffer offer, DefaultEngineMarketWindow defaultEngineMarketWindow, bool isCompatible, ToggleGroup toggleGroup, int carId)
	{
		_toggleGroup = toggleGroup;
		SetVisible(1);
		StatusEngine.gameObject.SetActive(value: false);
		_offer = offer;
		_engine = offer.Base;
		_defaultEngineMarketWindow = defaultEngineMarketWindow;
		TitleText.text = offer.GetDisplayName();
		offer.GetFullPrice(_allParts, carId, out var total);
		total.Get(out int value);
		PriceText.text = $"{value}";
		MassText.text = Utils.GetMass(offer.GetMass());
		MilliageText.text = $"{Utils.GetConvertedMileage(offer.GetTotalMileage()):n0} {Utils.CurrentMetric}";
		MaxTorqueText.text = offer.Base.GetMaxReliability ?? "";
		_isCompatible = isCompatible;
		SetCompatible();
		SelectButton.group = toggleGroup;
		SelectButton.onValueChanged.RemoveAllListeners();
		SelectButton.onValueChanged.AddListener(OnShopEngineClicked);
		LayoutRebuilder.ForceRebuildLayoutImmediate(PriceText.transform.parent.GetComponent<RectTransform>());
		InitRarityAndVendor(offer.Base);
	}

	public void InitForInventory(EngineKitData engineKitData, DefaultEngineMarketWindow defaultEngineMarketWindow, ToggleGroup toggleGroup, bool isCompatible, bool isInstalled = false)
	{
		_toggleGroup = toggleGroup;
		_isInstalled = isInstalled;
		_engine = engineKitData.GetEngine();
		MilliageTitleText.text = new LocalizedString("Cartuningwindow.Mileage:");
		_defaultEngineMarketWindow = defaultEngineMarketWindow;
		SetVisible(1);
		_engineKitData = engineKitData;
		TitleText.text = engineKitData.EngineId.GetName();
		_isCompatible = isCompatible;
		engineKitData.GetPrice(_allParts, out var total);
		PriceText.text = $"{total:d0}";
		MassText.text = Utils.GetMass(engineKitData.GetMass(_allParts));
		MilliageText.text = $"{Mathf.RoundToInt(Utils.GetConvertedMileage(engineKitData.EngineId.GetMileage()))} {Utils.CurrentMetric}";
		MaxTorqueText.text = $"{engineKitData.EngineId.GetDurability():p0}";
		if (isInstalled)
		{
			CompatibleText.text = new LocalizedString("Allinventoryfilterwindow.Installed");
			StatusPlate.color = _defaultEngineMarketWindow.InstalledColor;
			CompatibleText.color = Color.black;
			StatusPlate.gameObject.SetActive(value: true);
			SelectButton.group = toggleGroup;
			SelectButton.SetIsOnWithoutNotify(value: true);
			CheckStatus();
		}
		else
		{
			SetCompatible();
			SelectButton.group = toggleGroup;
			CheckStatus();
			LayoutRebuilder.ForceRebuildLayoutImmediate(PriceText.transform.parent.GetComponent<RectTransform>());
		}
		SelectButton.onValueChanged.RemoveAllListeners();
		SelectButton.onValueChanged.AddListener(OnInventoryEngineClicked);
		EngineItem engine = engineKitData.GetEngine();
		InitRarityAndVendor(engine);
	}

	private void InitRarityAndVendor(EngineItem engine)
	{
		ItemImagePanel.sprite = _carTuningDetailRaritySpritesConfig.CarTuningDetailRarityDatas[engine.Rarity].SquareRaritySpriteForInventory;
		CompanyImage.sprite = _detailVendorsConfigs.Vendors[engine.VendorId].CarTuningWindowSprite;
	}

	private void SetCompatible()
	{
		CompatibleText.text = (_isCompatible ? new LocalizedString("Enginefilterwindow.Compatible") : new LocalizedString("Enginefilterwindow.Incompatible"));
		StatusPlate.color = (_isCompatible ? _defaultEngineMarketWindow.CompatibleColor : _defaultEngineMarketWindow.NotCompatibleColor);
		StatusPlate.gameObject.SetActive(value: true);
	}

	private void OnInventoryEngineClicked(bool state)
	{
		if (state)
		{
			_defaultEngineMarketWindow.OnInventoryButtonPressed(_engineKitData, _engine, _isCompatible, _isInstalled);
		}
	}

	private void OnShopEngineClicked(bool state)
	{
		if (state)
		{
			_defaultEngineMarketWindow.OnItemClickMarket(_offer, _isCompatible);
		}
	}

	private void OnDisable()
	{
		SelectButton.group = null;
		SelectButton.SetIsOnWithoutNotify(value: false);
	}

	private void CheckStatus()
	{
		StatusEngine.gameObject.SetActive(value: true);
		bool checkEngineStatus = _managerUI.EditingEngineKitData.GetCheckEngineStatus();
		if (_managerUI.EditingEngineKitData.GetBrokenEngineStatus())
		{
			StatusEngine.color = Color.red;
		}
		else if (checkEngineStatus)
		{
			StatusEngine.color = Color.yellow;
		}
		else
		{
			StatusEngine.gameObject.SetActive(value: false);
		}
	}
}
