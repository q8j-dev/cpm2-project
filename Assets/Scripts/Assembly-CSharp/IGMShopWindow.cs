using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

public class IGMShopWindow : MarketPurchaseWindow
{
	[SerializeField]
	private Image hintImage;

	[SerializeField]
	private Button PriceFor200k;

	[SerializeField]
	private Button PriceFor500k;

	[SerializeField]
	private Button PriceFor1000k;

	[SerializeField]
	private Button PriceFor5000k;

	[SerializeField]
	private Button closeButton;

	[SerializeField]
	private Button hiddenCloseButton;

	[SerializeField]
	private Text Text200k;

	[SerializeField]
	private Text Text500k;

	[SerializeField]
	private Text Text1000k;

	[SerializeField]
	private Text Text5000k;

	private IPurchaseSystem _purchaseSystem;

	private CurrencyManagerController _currencyManagerController;

	[Inject]
	public void Construct(IPurchaseSystem purchaseSystem, CurrencyManagerController currencyManagerController)
	{
		_purchaseSystem = purchaseSystem;
		_currencyManagerController = currencyManagerController;
	}

	private void Awake()
	{
		if ((bool)PriceFor200k)
		{
			PriceFor200k.onClick.AddListener(OnMoney200K);
		}
		if ((bool)PriceFor500k)
		{
			PriceFor500k.onClick.AddListener(OnMoney500K);
		}
		if ((bool)PriceFor1000k)
		{
			PriceFor1000k.onClick.AddListener(OnMoney1000K);
		}
		if ((bool)PriceFor5000k)
		{
			PriceFor5000k.onClick.AddListener(OnMoney5000K);
		}
	}

	private void Start()
	{
		if (FlagsStorage.StoreInitialized)
		{
			Text200k.text = GetPrice(PurchaseItemType.Money200K, "");
			Text500k.text = GetPrice(PurchaseItemType.Money500K, "");
			Text1000k.text = GetPrice(PurchaseItemType.Money1000K, "");
			Text5000k.text = GetPrice(PurchaseItemType.Money5000K, "");
		}
	}

	private void OnMoney200K()
	{
		TryBuyMoney(PurchaseItemType.Money200K);
	}

	private void OnMoney500K()
	{
		TryBuyMoney(PurchaseItemType.Money500K);
	}

	private void OnMoney1000K()
	{
		TryBuyMoney(PurchaseItemType.Money1000K);
	}

	private void OnMoney5000K()
	{
		TryBuyMoney(PurchaseItemType.Money5000K);
	}

	private void TryBuyMoney(PurchaseItemType purchaseItemType)
	{
		_purchaseSystem.BuyProductID(purchaseItemType);
	}

	private string GetPrice(PurchaseItemType purchaseItemType, string priceDefault)
	{
		return _purchaseSystem.GetPurchaseProduct(purchaseItemType)?.LocalizedPriceText ?? priceDefault;
	}
}
