using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

public class MarketInventoryShowCasePanel : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI MaxMilliageTitleText;

	[SerializeField]
	private TextMeshProUGUI DetailName;

	[SerializeField]
	private TextMeshProUGUI WeightValueText;

	[SerializeField]
	private TextMeshProUGUI MaxMilliageValueText;

	[SerializeField]
	private TextMeshProUGUI DurabilityValueText;

	[SerializeField]
	private TextMeshProUGUI DetailDescriptionText;

	[SerializeField]
	private TextMeshProUGUI VendorTitleText;

	[SerializeField]
	private TextMeshProUGUI PriceText;

	[SerializeField]
	private Image VendorImage;

	[SerializeField]
	private Image MaxMileageImage;

	[SerializeField]
	private Image MoneyImage;

	[SerializeField]
	private Image DetailImage;

	[SerializeField]
	private Image DetailPanelImage;

	[SerializeField]
	private Image InstalledButton;

	[SerializeField]
	private Button SetButton;

	[SerializeField]
	private Button SellButton;

	[SerializeField]
	private Button BuyButton;

	[SerializeField]
	private Button BuyAndSetButton;

	[SerializeField]
	private float GearsMaxValue;

	[SerializeField]
	private float WeightMaxValue;

	[SerializeField]
	private float TorqueMaxValue;

	[SerializeField]
	private float HpMaxValue;

	[SerializeField]
	private Sprite CoinsSprite;

	[SerializeField]
	private Sprite MoneySprite;

	[SerializeField]
	private Sprite MaxMilliageSprite;

	[SerializeField]
	private Sprite DurabilitySprite;

	private PartData _partData;

	private PartItem _partItem;

	private Vendor _vendor;

	private ManagerUI _managerUI;

	private CarTuningDetailRaritySpritesConfig _carTuningDetailRaritySpritesConfig;

	private UnityAction OnBuy;

	private UnityAction OnBuyAndSet;

	private UnityAction OnSell;

	private UnityAction OnSet;

	[Inject]
	public void Construct(ManagerUI managerUI, CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig)
	{
		_managerUI = managerUI;
		_carTuningDetailRaritySpritesConfig = carTuningDetailRaritySpritesConfig;
	}

	private void Start()
	{
		if ((bool)BuyButton)
		{
			BuyButton.onClick.AddListener(OnBuyClick);
		}
		if ((bool)BuyAndSetButton)
		{
			BuyAndSetButton.onClick.AddListener(OnBuyAndSetClick);
		}
		SetButton.onClick.AddListener(OnSetClick);
		SellButton.onClick.AddListener(OnSellClick);
	}

	private void OnSellClick()
	{
		OnSell?.Invoke();
	}

	private void OnSetClick()
	{
		OnSet?.Invoke();
	}

	private void OnBuyAndSetClick()
	{
		OnBuyAndSet?.Invoke();
	}

	private void OnBuyClick()
	{
		OnBuy?.Invoke();
	}

	private void DisableAllButtons()
	{
		InstalledButton.gameObject.SetActive(value: false);
		SetButton.gameObject.SetActive(value: false);
		SellButton.gameObject.SetActive(value: false);
		if ((bool)BuyButton)
		{
			BuyButton.gameObject.SetActive(value: false);
		}
		if ((bool)BuyAndSetButton)
		{
			BuyAndSetButton.gameObject.SetActive(value: false);
		}
	}

	private void ChangeButtonsStateForMarket(bool isNotCompatible)
	{
		if (!isNotCompatible && (bool)BuyAndSetButton)
		{
			BuyAndSetButton.gameObject.SetActive(value: true);
		}
		SetButton.gameObject.SetActive(value: false);
		SellButton.gameObject.SetActive(value: false);
		if ((bool)BuyButton)
		{
			BuyButton.gameObject.SetActive(value: true);
		}
	}

	private void ChangeButtonsStateForInventory(bool isNotCompatible)
	{
		if (!isNotCompatible)
		{
			SetButton.gameObject.SetActive(value: true);
		}
		SellButton.gameObject.SetActive(value: true);
		if ((bool)BuyButton)
		{
			BuyButton.gameObject.SetActive(value: false);
		}
		if ((bool)BuyAndSetButton)
		{
			BuyAndSetButton.gameObject.SetActive(value: false);
		}
	}

	public void InitMarketWindowButtons(MarketInventoryWindow marketInventoryWindow)
	{
		OnBuy = () =>
		{
			marketInventoryWindow.OnBuyClick(_partData, _partItem);
		};
		OnBuyAndSet = () =>
		{
			marketInventoryWindow.OnBuyAndSetClick(_partData, _partItem);
		};
		OnSell = () =>
		{
			marketInventoryWindow.OnSellClick(_partData, _partItem);
		};
		OnSet = () =>
		{
			marketInventoryWindow.OnSetClick(_partData, _partItem);
		};
	}

	public void Init(PartData partData, PartItem part, Vendor vendor, bool isInCar, bool isInstalled, MarketInventoryWindow.OpenPanelType openPanelType, bool isNotCompatible)
	{
		MaxMilliageTitleText.text = new LocalizedString((openPanelType == MarketInventoryWindow.OpenPanelType.Market) ? "Marketinventorywindow.maxMileage" : "Cartuningwindow.Mileage:");
		MaxMileageImage.sprite = ((openPanelType == MarketInventoryWindow.OpenPanelType.Market) ? MaxMilliageSprite : DurabilitySprite);
		_partData = partData;
		_partItem = part;
		_vendor = vendor;
		base.gameObject.SetActive(value: true);
		DisableAllButtons();
		if (!isInstalled)
		{
			if (openPanelType == MarketInventoryWindow.OpenPanelType.Inventory)
			{
				ChangeButtonsStateForInventory(isNotCompatible);
			}
			else
			{
				ChangeButtonsStateForMarket(isNotCompatible);
			}
		}
		else
		{
			InstalledButton.gameObject.SetActive(value: true);
		}
		DetailName.text = part.GetName;
		WeightValueText.text = Utils.GetMass(part.Mass);
		MaxMilliageValueText.text = ((openPanelType == MarketInventoryWindow.OpenPanelType.Market) ? $"{Utils.GetConvertedMileage(part.MaxMileage):n0} {Utils.CurrentMetric}" : $"{Utils.GetConvertedMileage(_partData.GetMileage(part)):n0} {Utils.CurrentMetric}");
		DurabilityValueText.text = ((openPanelType == MarketInventoryWindow.OpenPanelType.Market) ? part.GetMaxReliability : $"{_partData.GetDurability():p0}");
		DetailDescriptionText.text = new LocalizedString(part.GetDescription);
		VendorTitleText.text = vendor.Name;
		ObscuredInt obscuredInt = Mathf.RoundToInt(_partData.GetCondition() * (float)(int)_partItem.GetPrice * 0.2f);
		obscuredInt = Mathf.Clamp(obscuredInt, 1, 2000);
		PriceText.text = ((openPanelType == MarketInventoryWindow.OpenPanelType.Inventory) ? $"{obscuredInt:n0}" : $"{part.GetPrice:n0}");
		DetailPanelImage.sprite = _carTuningDetailRaritySpritesConfig.CarTuningDetailRarityDatas[part.Rarity].SquareRaritySpriteForInventory;
		MoneyImage.sprite = ((part.GetPriceType == CostType.Coins) ? CoinsSprite : MoneySprite);
		DetailImage.sprite = _managerUI.GetEngineIcon((int)part.TypeId);
		VendorImage.sprite = _vendor.FullSizeSprite;
		LayoutRebuilder.ForceRebuildLayoutImmediate(PriceText.transform.parent.GetComponent<RectTransform>());
		LayoutRebuilder.ForceRebuildLayoutImmediate(MaxMilliageTitleText.transform.parent.GetComponent<RectTransform>());
	}
}
