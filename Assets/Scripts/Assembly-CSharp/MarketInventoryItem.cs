using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

public class MarketInventoryItem : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI PriceText;

	[SerializeField]
	private TextMeshProUGUI WeightText;

	[SerializeField]
	private TextMeshProUGUI MaxMiliageText;

	[SerializeField]
	private TextMeshProUGUI MiliageTitleText;

	[SerializeField]
	private TextMeshProUGUI DurabilityText;

	[SerializeField]
	private TextMeshProUGUI PartItemName;

	[SerializeField]
	private TextMeshProUGUI CompatibleInstalledText;

	[SerializeField]
	private Image MoneyImage;

	[SerializeField]
	private Image ItemImage;

	[SerializeField]
	private Image ItemImagePanel;

	[SerializeField]
	private Image CompanyImage;

	[SerializeField]
	private Image CompatiblePanel;

	[SerializeField]
	private Toggle ItemToggle;

	[SerializeField]
	private Image StatusImage;

	[SerializeField]
	private Color WarningColor;

	[SerializeField]
	private Color ErrorColor;

	private PartData _partData;

	private DetailVendorsConfigs _detailVendorsConfigs;

	private AllParts _allParts;

	private PartItem _partItem;

	private Vendor _vendor;

	private MarketInventoryWindow _marketInventoryWindow;

	private bool _isNotCompatible;

	private ManagerUI _managerUI;

	private bool isInstalled;

	private CarTuningDetailRaritySpritesConfig _carTuningDetailRaritySpritesConfig;

	[Inject]
	public void Construct(DetailVendorsConfigs detailVendorsConfigs, AllParts allParts, ManagerUI managerUI, CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig)
	{
		_managerUI = managerUI;
		_detailVendorsConfigs = detailVendorsConfigs;
		_carTuningDetailRaritySpritesConfig = carTuningDetailRaritySpritesConfig;
		_allParts = allParts;
	}

	public void Init(PartData partData, MarketInventoryWindow marketInventoryWindow, bool notCompatibleEngine, int instanceId = -1)
	{
		ItemToggle.onValueChanged.AddListener(OnValueChange);
		_isNotCompatible = notCompatibleEngine;
		_marketInventoryWindow = marketInventoryWindow;
		_partData = partData;
		_partItem = _partData.GetPart(_allParts);
		isInstalled = _partData.InstanceID == instanceId;
		ParseData();
		CheckCompatible(notCompatibleEngine);
		InitForInventory();
	}

	private void ParseData()
	{
		ItemToggle.SetIsOnWithoutNotify(value: false);
		MoneyImage.sprite = ((_partItem.GetPriceType == CostType.Coins) ? _marketInventoryWindow.CoinsSprite : _marketInventoryWindow.MoneySprite);
		if ((bool)_partItem && _partItem.VendorId != -1)
		{
			_vendor = _detailVendorsConfigs.GetVendorById(_partItem.VendorId);
			CompanyImage.sprite = _vendor.InventorySprite;
		}
		ItemImage.sprite = _managerUI.GetEngineIcon((int)_partItem.TypeId);
		ItemImagePanel.sprite = _carTuningDetailRaritySpritesConfig.CarTuningDetailRarityDatas[_partItem.Rarity].SquareRaritySpriteForInventory;
		CompanyImage.sprite = _detailVendorsConfigs.Vendors[_partItem.VendorId].CarTuningWindowSprite;
		ItemToggle.group = base.transform.parent.GetComponent<ToggleGroup>();
		WeightText.text = Utils.GetMass(_partItem.Mass) ?? "";
		MaxMiliageText.text = $"{Utils.GetConvertedMileage(_partItem.MaxMileage):n0} {Utils.CurrentMetric.ToUpper()}";
		DurabilityText.text = _partItem.GetMaxReliability;
		PriceText.text = $"{_partItem.GetPrice:d0}";
		PartItemName.text = _partItem.GetName;
	}

	private void CheckCompatible(bool notCompatibleEngine)
	{
		CompatiblePanel.color = ((!notCompatibleEngine) ? _marketInventoryWindow.CompatibleColor : _marketInventoryWindow.NotCompatiblColor);
		CompatibleInstalledText.text = ((!notCompatibleEngine) ? new LocalizedString("Enginefilterwindow.Compatible") : new LocalizedString("Enginefilterwindow.Incompatible"));
		if (isInstalled && _marketInventoryWindow.openPanel == MarketInventoryWindow.OpenPanelType.Inventory)
		{
			CompatiblePanel.color = _marketInventoryWindow.InstalledColor;
			CompatibleInstalledText.text = new LocalizedString("Allinventoryfilterwindow.Installed");
			CompatibleInstalledText.color = Color.black;
		}
	}

	private void InitForInventory()
	{
		if (_marketInventoryWindow.openPanel == MarketInventoryWindow.OpenPanelType.Inventory)
		{
			if (isInstalled)
			{
				ItemToggle.isOn = true;
			}
			ObscuredInt obscuredInt = Mathf.RoundToInt(_partData.GetCondition() * (float)(int)_partItem.GetPrice * 0.2f);
			obscuredInt = Mathf.Clamp(obscuredInt, 1, 2000);
			PriceText.text = $"{obscuredInt:n0}";
			MiliageTitleText.text = new LocalizedString("Cartuningwindow.Mileage:");
			MaxMiliageText.text = $"{Utils.GetConvertedMileage(_partData.GetMileage()):n0} {Utils.CurrentMetric.ToUpper()}";
			float durability = _partData.GetDurability();
			DurabilityText.text = $"{durability:p0}";
			if (durability < 0.5f)
			{
				StatusImage.gameObject.SetActive(value: true);
				StatusImage.color = ((durability < 0.2f) ? ErrorColor : WarningColor);
			}
			else
			{
				StatusImage.gameObject.SetActive(value: false);
			}
		}
	}

	private void OnValueChange(bool state)
	{
		if (state)
		{
			_marketInventoryWindow.OnDetailClick(_partData, isInstalled, _partItem, _vendor, _isNotCompatible);
		}
	}
}
