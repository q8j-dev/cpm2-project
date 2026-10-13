using System;
using CPM.Connection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Drag
{
	public class GameOverlayWindow : WindowScript, IRankReceiver
	{
		public enum OpenMode
		{
			Default = 0,
			OnlyBalance = 1
		}

		public enum BackButtonMode
		{
			Default = 0,
			Show = 1,
			Hide = 2
		}

		public class AdditionalData
		{
			public OpenMode OpenMode;

			public BackButtonMode BackButtonMode;
		}

		public static GameOverlayWindow instance;

		public TextMeshProUGUI m_PlayerName;

		public CurrencyItemView[] m_CurrencyItemViews;

		public TextMeshProUGUI offerCounter;

		public GameObject offerSale;

		public TextMeshProUGUI rankText;

		public RectTransform BalancePanel;

		public Button creditsBtn;

		public Button moneyBtn;

		public GameObject TunningProgress;

		public Button settingsBtn;

		public Button achievmentsBtn;

		public Button shopBtn;

		public CanvasGroup LevelsMenuWindowCanvasGroup;

		[SerializeField]
		private RectTransform RankTextParent;

		[SerializeField]
		private RawImage IconRank;

		[SerializeField]
		private TextMeshProUGUI RankName;

		[SerializeField]
		private GameObject LoadingRankSign;

		[SerializeField]
		private GameObject Social;

		[SerializeField]
		private GameObject MainHidablePanel;

		private IOfflineModeSystem _offlineModeSystem;

		private IConnectionDataHandler _connectionDataHandler;

		private AdditionalData _additionalData;

		private bool _suspendedFromPaint;

		[field: SerializeField]
		public Button BackButton { get; private set; }

		[field: SerializeField]
		public GameObject Background { get; private set; }

		[Inject]
		public void Construct(IConnectionDataHandler connectionDataHandler, IOfflineModeSystem offlineModeSystem)
		{
			_offlineModeSystem = offlineModeSystem;
			_connectionDataHandler = connectionDataHandler;
		}

		protected override void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			m_CurrencyItemViews = GetComponentsInChildren<CurrencyItemView>();
			achievmentsBtn.onClick.AddListener(() =>
			{
				_managerUI.ShowAchievmentsWindow();
			});
			creditsBtn.onClick.AddListener(() =>
			{
				_managerUI.OnMarketTypeClick?.Invoke(MarketType.CoinsMarket);
			});
			moneyBtn.onClick.AddListener(() =>
			{
				_managerUI.OnMarketTypeClick?.Invoke(MarketType.MoneysMarket);
			});
			settingsBtn.onClick.AddListener(OpenSettings);
			BackButton.onClick.AddListener(BackButtonHandler());
			base.Awake();
		}

		private UnityAction BackButtonHandler()
		{
			return _managerUI.Back;
		}

		private void OpenSettings()
		{
			_managerUI.CloseWindow("MainMenuWindow");
			_managerUI.ShowSettingsWindow();
		}

		protected override void OnEnable()
		{
			RestoreBalance();
			CurrencyManagerController.OnMoneyChanged += UpdateM;
			CoinPurchaseWindow.OnCoinsChanged += UpdateCR;
			GetterCoinsManager.CoinsChanged = (Action<int, GetterCoinsManager.Source>)Delegate.Combine(GetterCoinsManager.CoinsChanged, new Action<int, GetterCoinsManager.Source>(OnCoinsPrefsChanged));
			ManagerUI managerUI = _managerUI;
			managerUI.OnBackButtonActivated = (Action<bool>)Delegate.Combine(managerUI.OnBackButtonActivated, new Action<bool>(ActivateBackButton));
			ManagerUI managerUI2 = _managerUI;
			managerUI2.OnSettingsButtonActivated = (Action<bool>)Delegate.Combine(managerUI2.OnSettingsButtonActivated, new Action<bool>(ActivateSettingsButton));
			ManagerUI managerUI3 = _managerUI;
			managerUI3.EnableAllBalanceWindow = (Action<bool>)Delegate.Combine(managerUI3.EnableAllBalanceWindow, new Action<bool>(EnableAllBalanceWindow));
			ManagerUI managerUI4 = _managerUI;
			managerUI4.OnAchievmentsButtonActivated = (Action<bool>)Delegate.Combine(managerUI4.OnAchievmentsButtonActivated, new Action<bool>(ActivateAchievmentsButton));
			ManagerUI managerUI5 = _managerUI;
			managerUI5.OnActiveAllBalanceWindow = (Action<bool>)Delegate.Combine(managerUI5.OnActiveAllBalanceWindow, new Action<bool>(ActiveAllBalanceWindow));
			ManagerUI managerUI6 = _managerUI;
			managerUI6.OnActivateBalancePanel = (Action<bool>)Delegate.Combine(managerUI6.OnActivateBalancePanel, new Action<bool>(SetActiveBalanceAttachments));
			ManagerUI managerUI7 = _managerUI;
			managerUI7.ChangeBalancePanelActiveState = (Action<bool>)Delegate.Combine(managerUI7.ChangeBalancePanelActiveState, new Action<bool>(HideAllIcons));
			ManagerUI managerUI8 = _managerUI;
			managerUI8.SetBackground = (Action<bool>)Delegate.Combine(managerUI8.SetBackground, new Action<bool>(SetBackground));
			base.OnEnable();
			BackButton.gameObject.SetActive(value: false);
			InputMapSetEvent.Subscribe(CheckForPaintMap);
		}

		private void CheckForPaintMap(InputGroupsEnum group)
		{
			if (InputMapSetEvent.GetNewMaps().Contains(InputMapEnum.PaintBase))
			{
				SuspendUiNavigation();
				_suspendedFromPaint = true;
			}
			else if (_suspendedFromPaint)
			{
				RestoreUiNavigation();
			}
		}

		protected override void Start()
		{
			base.Start();
			TryInitWindow();
			_connectionDataHandler.SubscribeForRankChange(this);
			ManagerUI managerUI = _managerUI;
			managerUI.OnCurrencyRestored = (Action)Delegate.Combine(managerUI.OnCurrencyRestored, new Action(RestoreBalance));
			if (_offlineModeSystem.Enabled)
			{
				UpdateRankUI();
			}
			if (_additionalData != null && _additionalData.OpenMode == OpenMode.OnlyBalance)
			{
				ActivateSettingsButton(enable: false);
				ActivateAchievmentsButton(enable: false);
			}
		}

		private void OnDisable()
		{
			CurrencyManagerController.OnMoneyChanged -= UpdateM;
			CoinPurchaseWindow.OnCoinsChanged -= UpdateCR;
			GetterCoinsManager.CoinsChanged = (Action<int, GetterCoinsManager.Source>)Delegate.Remove(GetterCoinsManager.CoinsChanged, new Action<int, GetterCoinsManager.Source>(OnCoinsPrefsChanged));
			ManagerUI managerUI = _managerUI;
			managerUI.OnBackButtonActivated = (Action<bool>)Delegate.Remove(managerUI.OnBackButtonActivated, new Action<bool>(ActivateBackButton));
			ManagerUI managerUI2 = _managerUI;
			managerUI2.OnSettingsButtonActivated = (Action<bool>)Delegate.Remove(managerUI2.OnSettingsButtonActivated, new Action<bool>(ActivateSettingsButton));
			ManagerUI managerUI3 = _managerUI;
			managerUI3.EnableAllBalanceWindow = (Action<bool>)Delegate.Remove(managerUI3.EnableAllBalanceWindow, new Action<bool>(EnableAllBalanceWindow));
			ManagerUI managerUI4 = _managerUI;
			managerUI4.OnAchievmentsButtonActivated = (Action<bool>)Delegate.Remove(managerUI4.OnAchievmentsButtonActivated, new Action<bool>(ActivateAchievmentsButton));
			ManagerUI managerUI5 = _managerUI;
			managerUI5.OnActiveAllBalanceWindow = (Action<bool>)Delegate.Remove(managerUI5.OnActiveAllBalanceWindow, new Action<bool>(ActiveAllBalanceWindow));
			ManagerUI managerUI6 = _managerUI;
			managerUI6.OnActivateBalancePanel = (Action<bool>)Delegate.Remove(managerUI6.OnActivateBalancePanel, new Action<bool>(SetActiveBalanceAttachments));
			ManagerUI managerUI7 = _managerUI;
			managerUI7.ChangeBalancePanelActiveState = (Action<bool>)Delegate.Remove(managerUI7.ChangeBalancePanelActiveState, new Action<bool>(HideAllIcons));
			ManagerUI managerUI8 = _managerUI;
			managerUI8.SetBackground = (Action<bool>)Delegate.Remove(managerUI8.SetBackground, new Action<bool>(SetBackground));
			InputMapSetEvent?.Unsubscribe(CheckForPaintMap);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			achievmentsBtn.onClick.RemoveAllListeners();
			creditsBtn.onClick.RemoveAllListeners();
			moneyBtn.onClick.RemoveAllListeners();
			settingsBtn.onClick.RemoveAllListeners();
			BackButton.onClick.RemoveAllListeners();
			ManagerUI managerUI = _managerUI;
			managerUI.OnCurrencyRestored = (Action)Delegate.Remove(managerUI.OnCurrencyRestored, new Action(RestoreBalance));
			_connectionDataHandler?.UnsubscribeFromRankChange(this);
			instance = null;
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			if (!string.IsNullOrEmpty(data))
			{
				_additionalData = JsonConvert.DeserializeObject<AdditionalData>(data);
			}
		}

		private void TryInitWindow()
		{
			if (_managerUI.GameOverlayWindowInitializer != null)
			{
				_managerUI.GameOverlayWindowInitializer.Init(this);
				_managerUI.GameOverlayWindowInitializer = null;
			}
		}

		private void HideAllIcons(bool hide)
		{
			MainHidablePanel.SetActive(hide);
		}

		private void SetBackground(bool hide)
		{
			Background.SetActive(hide);
		}

		private void EnableAllBalanceWindow(bool enable)
		{
			creditsBtn.interactable = enable;
			moneyBtn.interactable = enable;
			settingsBtn.interactable = enable;
			shopBtn.interactable = enable;
		}

		private void ActiveAllBalanceWindow(bool enable)
		{
			CurrencyManagerController.OnMoneyChanged -= UpdateM;
			CoinPurchaseWindow.OnCoinsChanged -= UpdateCR;
			GetterCoinsManager.CoinsChanged = (Action<int, GetterCoinsManager.Source>)Delegate.Remove(GetterCoinsManager.CoinsChanged, new Action<int, GetterCoinsManager.Source>(OnCoinsPrefsChanged));
			creditsBtn.SetActive(enable);
			moneyBtn.SetActive(enable);
			TunningProgress.SetActive(enable);
			if (enable)
			{
				CurrencyManagerController.OnMoneyChanged += UpdateM;
				CoinPurchaseWindow.OnCoinsChanged += UpdateCR;
				GetterCoinsManager.CoinsChanged = (Action<int, GetterCoinsManager.Source>)Delegate.Combine(GetterCoinsManager.CoinsChanged, new Action<int, GetterCoinsManager.Source>(OnCoinsPrefsChanged));
				SetBalance(CurrencyItemType.coins, (int)CurrencyManagerController.Coins);
				SetBalance(CurrencyItemType.money, (int)CurrencyManagerController.Money);
			}
			_managerUI.OnAchievmentsButtonActivated?.Invoke(enable);
			_managerUI.OnSettingsButtonActivated?.Invoke(enable);
			shopBtn.SetActive(enable);
		}

		private void ActivateBackButton(bool enable)
		{
		}

		public void ActivateSettingsButton(bool enable)
		{
			if (!enable || _managerUI.IsMainMenuActive())
			{
				settingsBtn.SetActive(enable);
			}
		}

		private void ActivateAchievmentsButton(bool enable)
		{
			if (!enable || _managerUI.IsMainMenuActive())
			{
				achievmentsBtn.SetActive(enable);
			}
		}

		public void SetActiveBalanceAttachments(bool enable)
		{
			BalancePanel.gameObject.SetActive(enable);
		}

		public static void SetRank(Texture2D tex, string rankText)
		{
			instance.rankText.text = rankText;
		}

		private void RestoreBalance()
		{
			SetBalance(CurrencyItemType.coins, (int)CurrencyManagerController.Coins);
			SetBalance(CurrencyItemType.money, (int)CurrencyManagerController.Money);
		}

		private void UpdateCR(object sender)
		{
			SetBalance(CurrencyItemType.coins, (int)CurrencyManagerController.Coins);
		}

		private void OnCoinsPrefsChanged(int coins, GetterCoinsManager.Source source)
		{
			SetBalance(CurrencyItemType.coins, (int)CurrencyManagerController.Coins);
		}

		private void UpdateM(object sender)
		{
			SetBalance(CurrencyItemType.money, (int)CurrencyManagerController.Money);
		}

		public static void OfferSale(bool state)
		{
			instance.offerSale.SetActive(state);
		}

		public static void SetTextOfferSale(string text)
		{
			instance.offerSale.transform.GetComponentInChildren<TextMeshProUGUI>().text = text;
		}

		public static void IncrementCountOffers(bool offer)
		{
			instance.offerCounter.gameObject.SetActive(value: true);
			if (offer)
			{
				int num = int.Parse(instance.offerCounter.text);
				instance.offerCounter.text = (num + 1).ToString();
			}
		}

		public void SetBalance(CurrencyItemType currencyId, float balance)
		{
			CurrencyItemView[] currencyItemViews = instance.m_CurrencyItemViews;
			foreach (CurrencyItemView currencyItemView in currencyItemViews)
			{
				if (currencyId == currencyItemView.currencyItemType)
				{
					currencyItemView.SetBalance(balance);
				}
			}
		}

		public void ClearBalances()
		{
			CurrencyItemView[] currencyItemViews = m_CurrencyItemViews;
			for (int i = 0; i < currencyItemViews.Length; i++)
			{
				currencyItemViews[i].SetBalance(0f);
			}
		}

		public CurrencyItemView GetCurrencyItemView(CurrencyItemType currencyDefinitionId)
		{
			CurrencyItemView[] currencyItemViews = m_CurrencyItemViews;
			foreach (CurrencyItemView currencyItemView in currencyItemViews)
			{
				if (currencyItemView.currencyItemType == currencyDefinitionId)
				{
					return currencyItemView;
				}
			}
			return null;
		}

		private async UniTaskVoid ShowRankUpdate(int rank)
		{
			(await _managerUI.ShowPopupNotificationWindow()).InitRankNotification(midTexture: UtilsRank.LoadRankTextureFromResources((UtilsRank.RankType)rank), newRank: rank, title: new LocalizedString("FreeDriveDB.NewRank"));
		}

		public void OnRankReceived(ref int rank)
		{
			if (FlagsStorage.IsMainMenu && !_offlineModeSystem.Enabled)
			{
				if (rank > 10)
				{
					rank -= 10;
					ShowRankUpdate(rank);
				}
				UtilsRank.SetRank((UtilsRank.RankType)rank);
				UpdateRankUI();
			}
		}

		private void UpdateRankUI()
		{
			Texture texture = UtilsRank.LoadMyRankTextureFromResources();
			if ((bool)LoadingRankSign)
			{
				LoadingRankSign.SetActive(value: false);
			}
			if ((bool)IconRank)
			{
				IconRank.gameObject.SetActive(value: true);
				IconRank.texture = texture;
			}
			RankName.text = UtilsRank.GetMyRankName();
			RebuildLayout().Forget();
		}

		private async UniTaskVoid RebuildLayout()
		{
			await UniTask.Yield(this.GetCancellationTokenOnDestroy());
			LayoutRebuilder.ForceRebuildLayoutImmediate(RankTextParent);
		}
	}
}
