using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;

public class CurrencyShopWindow : WindowScript, IBackableWindow
{
	[SerializeField]
	private Button CoinsMarketButton = default;

	[SerializeField]
	private Button MoneyMarketButton = default;

	[SerializeField]
	private MarketPurchaseWindow MoneyMarketWindow = default;

	[SerializeField]
	private MarketPurchaseWindow CoinMarketWindow = default;

	[SerializeField]
	private CurrencyItemView[] CurrencyItemViews = default;

	[SerializeField]
	private RectTransform LayoutParent = default;

	[SerializeField]
	private TextMeshProUGUI titleText = default;

	private bool _isDestroyed;

	private IOfflineModeSystem _offlineModeSystem;

	[field: SerializeField]
	public Button BackButton { get; private set; }

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[Inject]
	public void Construct(IOfflineModeSystem offlineModeSystem)
	{
		_offlineModeSystem = offlineModeSystem;
	}

	protected override void Start()
	{
		base.Start();
		ShowOrHideTopButtons(state: false);
		InitBackButton();
		CurrencyManagerController.OnMoneyChanged += UpdateM;
		CoinPurchaseWindow.OnCoinsChanged += UpdateCR;
		Init(_managerUI.CurrentMarketType);
		MoneyMarketButton.onClick.AddListener(() =>
		{
			Init(MarketType.MoneysMarket);
		});
		CoinsMarketButton.onClick.AddListener(() =>
		{
			Init(MarketType.CoinsMarket);
		});
		UpdateBalance(CurrencyItemType.coins, (int)CurrencyManagerController.Coins, ignoreAnimation: true);
		UpdateBalance(CurrencyItemType.money, (int)CurrencyManagerController.Money, ignoreAnimation: true);
		_managerUI.IncreaseTimerButtonShow?.Invoke(obj: false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_isDestroyed = true;
		CurrencyManagerController.OnMoneyChanged -= UpdateM;
		CoinPurchaseWindow.OnCoinsChanged -= UpdateCR;
	}

	public void InitBackButton()
	{
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		CurrencyManagerController.OnMoneyChanged -= UpdateM;
		CoinPurchaseWindow.OnCoinsChanged -= UpdateCR;
		ShowOrHideTopButtons(state: true);
		if (ScenesHelper.IsFreeDriveSceneActive())
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: false);
			_managerUI.ReloadActiveWindows();
		}
		else if ((!V.CoreGameManager.HaveCarInAnyGarage() && _offlineModeSystem.Disabled) || (bool)V.CoreGameManager.ManagerUI.GetWindow("ConfirmGarageBuyWindow"))
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: false);
		}
		Close();
	}

	protected override void OnEnable()
	{
		BackButton.gameObject.SetActive(value: false);
		base.OnEnable();
	}

	private void UpdateCR(object sender)
	{
		UpdateBalance(CurrencyItemType.coins, (int)CurrencyManagerController.Coins);
	}

	private void UpdateM(object sender)
	{
		UpdateBalance(CurrencyItemType.money, (int)CurrencyManagerController.Money);
	}

	private void UpdateBalance(CurrencyItemType currencyItemType, float balance, bool ignoreAnimation = false)
	{
		CurrencyItemView[] currencyItemViews = CurrencyItemViews;
		foreach (CurrencyItemView currencyItemView in currencyItemViews)
		{
			if ((bool)currencyItemView && currencyItemType == currencyItemView.currencyItemType)
			{
				currencyItemView.SetBalance(balance, ignoreAnimation);
			}
		}
		_managerUI.OnCurrencyRestored?.Invoke();
		ReloadLayout().Forget();
	}

	private void Init(MarketType marketType)
	{
		titleText.text = new LocalizedString((marketType == MarketType.CoinsMarket) ? "PurchaseWindow.CoinsTitle" : "PurchaseWindow.MoneyTitle");
		MoneyMarketWindow.SetActive(marketType == MarketType.MoneysMarket);
		CoinMarketWindow.SetActive(marketType == MarketType.CoinsMarket);
	}

	public void ShowOrHideTopButtons(bool state)
	{
		_managerUI.OnActiveAllBalanceWindow?.Invoke(state);
		bool flag = _managerUI.IsMainMenuActive();
		bool flag2 = ScenesHelper.IsLapRacingSceneActive();
		if (!state || (!flag && !flag2))
		{
			_ = !_managerUI.PersonWindowUnpurchasedWindowActive;
		}
		else
			_ = 0;
	}

	private async UniTaskVoid ReloadLayout()
	{
		await UniTask.Yield();
		if (!_isDestroyed)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutParent);
		}
	}
}
