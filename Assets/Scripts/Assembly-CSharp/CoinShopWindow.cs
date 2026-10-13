using System;
using System.Threading;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using NewUI.Scripts;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

public class CoinShopWindow : MarketPurchaseWindow
{
	private const int RoundStep = 5;

	private const int MaxPercent = 95;

	private const int MinPercent = 0;

	[SerializeField]
	private Button PriceFor100 = default;

	[SerializeField]
	private Button PriceFor300 = default;

	[SerializeField]
	private Button PriceFor1000 = default;

	[SerializeField]
	private Button PriceFor5000 = default;

	[SerializeField]
	private Button PriceFor10000 = default;

	[SerializeField]
	private Text Text100 = default;

	[SerializeField]
	private MarketSpecialPriceElement SpecialPrice100Element = default;

	[SerializeField]
	private Text Text300 = default;

	[SerializeField]
	private MarketSpecialPriceElement SpecialPrice300Element = default;

	[SerializeField]
	private Text Text1000 = default;

	[SerializeField]
	private MarketSpecialPriceElement SpecialPrice1000Element = default;

	[SerializeField]
	private Text Text5000 = default;

	[SerializeField]
	private MarketSpecialPriceElement SpecialPrice5000Element = default;

	[SerializeField]
	private Text Text10000 = default;

	[SerializeField]
	private MarketSpecialPriceElement SpecialPrice10000Element = default;

	[SerializeField]
	private GameObject CoinsPanel = default;

	[SerializeField]
	private GameObject DiscountPanel = default;

	[SerializeField]
	private Button[] CoinsButtons = default;

	[SerializeField]
	private Button[] DiscountButtons = default;

	private FreeDriveProvider _freeDriveProvider;

	private PurchaseCoinsBuyDiscount _purchaseCoinsBuyDiscount = new PurchaseCoinsBuyDiscount();

	private CurrencyManagerController _currencyManagerController;

	private IRemoteConfigManager _remoteConfigManager;

	private ConfigValuesStorage _configValuesStorage;

	private IGuestAccountCreator _guestAccountCreator;

	private IPurchaseSystem _purchaseSystem;

	[Inject]
	public void Construct(FreeDriveProvider freeDriveProvider, CurrencyManagerController currencyManagerController, IRemoteConfigManager remoteConfigManager, IGuestAccountCreator guestAccountCreator, IPurchaseSystem purchaseSystem)
	{
		_remoteConfigManager = remoteConfigManager;
		_freeDriveProvider = freeDriveProvider;
		_guestAccountCreator = guestAccountCreator;
		_currencyManagerController = currencyManagerController;
		_purchaseSystem = purchaseSystem;
	}

	private void Awake()
	{
		Init();
		if ((bool)PriceFor100)
		{
			PriceFor100.onClick.AddListener(() =>
			{
				_purchaseSystem.BuyProductID(PurchaseItemType.Coin1);
			});
		}
		if ((bool)PriceFor300)
		{
			PriceFor300.onClick.AddListener(() =>
			{
				_purchaseSystem.BuyProductID(PurchaseItemType.Coin2);
			});
		}
		if ((bool)PriceFor1000)
		{
			PriceFor1000.onClick.AddListener(() =>
			{
				_purchaseSystem.BuyProductID(PurchaseItemType.Coin3);
			});
		}
		if ((bool)PriceFor5000)
		{
			PriceFor5000.onClick.AddListener(() =>
			{
				_purchaseSystem.BuyProductID(PurchaseItemType.Coin4);
			});
		}
		if ((bool)PriceFor10000)
		{
			PriceFor10000.onClick.AddListener(() =>
			{
				_purchaseSystem.BuyProductID(PurchaseItemType.Coin5);
			});
		}
		if ((bool)SpecialPrice100Element)
		{
			SpecialPrice100Element.AddListener(() =>
			{
				_purchaseSystem.BuyProductID(PurchaseItemType.Coin1Fp);
			});
		}
		if ((bool)SpecialPrice300Element)
		{
			SpecialPrice300Element.AddListener(() =>
			{
				_purchaseSystem.BuyProductID(PurchaseItemType.Coin2Fp);
			});
		}
		if ((bool)SpecialPrice1000Element)
		{
			SpecialPrice1000Element.AddListener(() =>
			{
				_purchaseSystem.BuyProductID(PurchaseItemType.Coin3Fp);
			});
		}
		if ((bool)SpecialPrice5000Element)
		{
			SpecialPrice5000Element.AddListener(() =>
			{
				_purchaseSystem.BuyProductID(PurchaseItemType.Coin4Fp);
			});
		}
		if ((bool)SpecialPrice10000Element)
		{
			SpecialPrice10000Element.AddListener(() =>
			{
				_purchaseSystem.BuyProductID(PurchaseItemType.Coin5Fp);
			});
		}
	}

	private void Init()
	{
		CheckDiscountAvailable(base.destroyCancellationToken).Forget();
	}

	public override void Reload()
	{
		Init();
	}

	private async UniTaskVoid CheckDiscountAvailable(CancellationToken cancellationToken)
	{
		_configValuesStorage = await _remoteConfigManager.GetConfigValuesStorage();
		ResetPanels();
		ShowLoadingIcon(show: true);
		if (_configValuesStorage.coinsDiscount == 0)
		{
			int[] availableIndexes = await _purchaseCoinsBuyDiscount.CheckProductToBuyAsync();
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			ActivateButtons(availableIndexes);
		}
		else
		{
			ActivateButtons(null);
		}
		ShowLoadingIcon(show: false);
	}

	private void ResetPanels()
	{
		CoinsPanel.SetActive(value: false);
		DiscountPanel.SetActive(value: false);
		for (int i = 0; i < CoinsButtons.Length; i++)
		{
			CoinsButtons[i].gameObject.SetActive(value: true);
			DiscountButtons[i].gameObject.SetActive(value: false);
		}
	}

	private void ActivateButtons(int[] availableIndexes)
	{
		if (availableIndexes != null)
		{
			for (int i = 0; i < availableIndexes.Length; i++)
			{
				bool flag = availableIndexes[i] > 0;
				CoinsButtons[i].gameObject.SetActive(!flag);
				DiscountButtons[i].gameObject.SetActive(flag);
			}
			DiscountPanel.SetActive(value: true);
		}
		CoinsPanel.SetActive(value: true);
	}

	private void DeactivateDiscountButton(int index)
	{
		CoinsButtons[index].gameObject.SetActive(value: true);
		DiscountButtons[index].gameObject.SetActive(value: false);
	}

	private void Start()
	{
		_currencyManagerController.CoinsChanged += OnPurchaseComplete;
		if (FlagsStorage.StoreInitialized)
		{
			Text100.text = GetPriceText(PurchaseItemType.Coin1);
			Text300.text = GetPriceText(PurchaseItemType.Coin2);
			Text1000.text = GetPriceText(PurchaseItemType.Coin3);
			Text5000.text = GetPriceText(PurchaseItemType.Coin4);
			Text10000.text = GetPriceText(PurchaseItemType.Coin5);
			SpecialPrice100Element.Init(GetPriceText(PurchaseItemType.Coin1), GetPriceText(PurchaseItemType.Coin1Fp), GetDiscountPrice(PurchaseItemType.Coin1, PurchaseItemType.Coin1Fp));
			SpecialPrice300Element.Init(GetPriceText(PurchaseItemType.Coin2), GetPriceText(PurchaseItemType.Coin2Fp), GetDiscountPrice(PurchaseItemType.Coin2, PurchaseItemType.Coin2Fp));
			SpecialPrice1000Element.Init(GetPriceText(PurchaseItemType.Coin3), GetPriceText(PurchaseItemType.Coin3Fp), GetDiscountPrice(PurchaseItemType.Coin3, PurchaseItemType.Coin3Fp));
			SpecialPrice5000Element.Init(GetPriceText(PurchaseItemType.Coin4), GetPriceText(PurchaseItemType.Coin4Fp), GetDiscountPrice(PurchaseItemType.Coin4, PurchaseItemType.Coin4Fp));
			SpecialPrice10000Element.Init(GetPriceText(PurchaseItemType.Coin5), GetPriceText(PurchaseItemType.Coin5Fp), GetDiscountPrice(PurchaseItemType.Coin5, PurchaseItemType.Coin5Fp));
		}
	}

	private string GetDiscountPrice(PurchaseItemType oldItem, PurchaseItemType newItem)
	{
		float price = GetPrice(oldItem);
		float price2 = GetPrice(newItem);
		if (price <= 0f || price2 <= 0f)
		{
			return "";
		}
		int num = Math.Clamp((int)(Math.Round((price - price2) / price * 100f / 5f) * 5.0), 0, 95);
		return $"{num}%";
	}

	private string GetPriceText(PurchaseItemType purchaseItemType)
	{
		return _purchaseSystem.GetPurchaseProduct(purchaseItemType)?.LocalizedPriceText ?? "";
	}

	private float GetPrice(PurchaseItemType purchaseItemType)
	{
		return _purchaseSystem.GetPurchaseProduct(purchaseItemType)?.LocalizedPrice ?? 0f;
	}

	private void OnPurchaseComplete()
	{
		Reload();
	}

	private void OnDestroy()
	{
		_currencyManagerController.CoinsChanged -= OnPurchaseComplete;
	}
}
