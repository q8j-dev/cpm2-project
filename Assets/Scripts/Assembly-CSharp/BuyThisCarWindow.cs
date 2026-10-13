using System;
using System.Globalization;
using CPM.Account;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.RequestResponseService;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.Wallet;

public class BuyThisCarWindow : WindowScriptAsyncResult<RequestAnswer>
{
	public static ObscuredInt CarDetailsPrice = 10;

	private const int RotorId = 6;

	[SerializeField]
	private Image ImageCar;

	[SerializeField]
	private TextMeshProUGUI TitleText;

	[SerializeField]
	private TextMeshProUGUI CarTitleText;

	[SerializeField]
	private TextMeshProUGUI TextPrice;

	[SerializeField]
	private Button ButtonBuy;

	[SerializeField]
	private Button ButtonIgnore;

	[SerializeField]
	private Button ButtonClose;

	[SerializeField]
	private Button ButtonDetails;

	[SerializeField]
	private TextMeshProUGUI BuyButtonText;

	[SerializeField]
	private GameObject ShortPanel;

	[SerializeField]
	private GameObject LongPanel;

	[SerializeField]
	private CarTuningExchangeUIElement[] CarTuningExchangeUIElements;

	[SerializeField]
	private CarTuningExchangeUIElement[] DisableIfRotor;

	[SerializeField]
	private CarExchangeLongInfoPanel CarExchangeLongInfoPanel;

	[SerializeField]
	private RectTransform LayoutParent;

	[SerializeField]
	private CanvasGroup CanvasGroup;

	[SerializeField]
	private GameObject PricePanel;

	private CurrencyManagerController _currencyManagerController;

	private ParkingMenuUI _parkingMenuUI;

	private AllCarsInfo _allCarsInfo;

	private IAccountController _accountController;

	private LevelsProvider _levelsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private RpcRrService _rpcRrService;

	private Player _carOwnerPlayer;

	private string _tradeOfferID = "";

	private CarTradeMoreInfoAnalytic _carTradeMoreInfoAnalytic;

	private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

	private bool _logFromOfferCreator;

	private ISteamAchievementService _steamAchievementService;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[Inject]
	private void Construct(CurrencyManagerController currencyManagerController, ParkingMenuUI parkingMenuUI, AllCarsInfo allCarsInfo, IAccountController accountController, LevelsProvider levelsProvider, FreeDriveProvider freeDriveProvider, RpcRrService rpcRrService, CarTradeMoreInfoAnalytic carTradeMoreInfoAnalytic, VirtualPurchaseAnalytic virtualPurchaseAnalytic, ISteamAchievementService steamAchievementService)
	{
		_steamAchievementService = steamAchievementService;
		_freeDriveProvider = freeDriveProvider;
		_levelsProvider = levelsProvider;
		_currencyManagerController = currencyManagerController;
		_parkingMenuUI = parkingMenuUI;
		_allCarsInfo = allCarsInfo;
		_accountController = accountController;
		_rpcRrService = rpcRrService;
		_carTradeMoreInfoAnalytic = carTradeMoreInfoAnalytic;
		_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		GlobalEventSystem.OnBuyThisCarWindowClosed?.Invoke();
	}

	private void CloseAndActivateOther()
	{
		_levelsProvider.LevelScript.SetActiveFalseEachMenu(1);
		_levelsProvider.LevelScript.UnCleanScreen();
		Close();
	}

	public override void Close()
	{
		base.Close();
		if (!base.IsCloseCanceled)
		{
			_parkingMenuUI.Show();
		}
	}

	private void OnCarDetailsReceived(CarTuningData carTuningData)
	{
		ShowUI(state: true);
		LongPanel.SetActive(value: true);
		ShortPanel.SetActive(value: false);
		CarExchangeLongInfoPanel.Init(carTuningData);
		RebuildLayout().Forget();
		GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQEA", "car_details_view");
		_steamAchievementService?.Unlock("ACH_MARKET_SCOUT");
	}

	private void SendCancel(Action<RequestAnswer> callback)
	{
		if ((bool)_freeDriveProvider.PlayerListControl)
		{
			_freeDriveProvider.PlayerListControl.CloseWindow(needUnclean: false);
		}
		else if ((bool)_managerUI.CurrentPlayerProfile)
		{
			_managerUI.CurrentPlayerProfile.CloseWithUnclean();
		}
		if (!FlagsStorage.InGarageFree)
		{
			callback?.Invoke(RequestAnswer.Decline);
		}
		TrySetResult(new ResultData(CodeType.Success, RequestAnswer.Decline));
		CloseAndActivateOther();
	}

	public void Init(string textTitle, string textTitleCar, int carId, int engineType, int price, int shortTuningData, string tradeOfferID, Player carOwnerPlayer, bool logFromOfferCreator = false)
	{
		_logFromOfferCreator = logFromOfferCreator;
		_tradeOfferID = tradeOfferID;
		Init(textTitle, carId, engineType, price, shortTuningData, null, showIgnore: false, isExchange: false, isFromCarSale: false, carOwnerPlayer);
		CarTitleText.text = textTitleCar;
		TitleText.text = textTitle;
		_carTradeMoreInfoAnalytic.LogPreviewOpened(_tradeOfferID, _logFromOfferCreator);
	}

	private void Init(string textTitle, int carId, int engineType, int price, int shortTuningData, Action<RequestAnswer> callback, bool showIgnore, bool isExchange, bool isFromCarSale, Player carOwnerPlayer)
	{
		_carOwnerPlayer = carOwnerPlayer;
		bool flag = engineType == 1024;
		if (!_accountController.HasEmail)
		{
			_managerUI.ShowNotificationWindow(isFromCarSale ? new LocalizedString("BuyThisCarWindow.YouMustHaveAnAccountToBuyACar") : new LocalizedString("BuyThisCarWindow.YouMustHaveAnAccountToExchange"), 3f, NotificationType.Error).Forget();
			SendCancel(callback);
			return;
		}
		_allCarsInfo.ResolveCarMeta(carId, out var result);
		if (math.all(result == Utils.NotValid) || math.all(_allCarsInfo.CheckTradeCarNotAllowed(carId) == Utils.NotValid))
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("BuyThisCarWindow.Youcannotbuyorexchangethiscar!"), 3f, NotificationType.Error).Forget();
			SendCancel(callback);
			return;
		}
		if (CarIdStorage.IsCarLockedOnThisPlatform(carId))
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("CarShop.AvailableOnlyOnSteam"), 3f, NotificationType.Error).Forget();
			SendCancel(callback);
			return;
		}
		bool flag2 = price == 0;
		PricePanel.SetActive(!flag2);
		if (isFromCarSale || !flag2)
		{
			CarTitleText.text = textTitle;
			TitleText.text = new LocalizedString("BuyThisCarWindow.PartsCondition");
		}
		else
		{
			if ((bool)_freeDriveProvider.PlayerListControl)
			{
				_freeDriveProvider.PlayerListControl.CloseWindow(needUnclean: false);
			}
			else if ((bool)_managerUI.CurrentPlayerProfile)
			{
				_managerUI.CurrentPlayerProfile.CloseWithUnclean();
			}
			TitleText.text = textTitle;
		}
		_parkingMenuUI.Hide();
		LongPanel.SetActive(value: false);
		ShortPanel.SetActive(value: true);
		BuyButtonText.text = (flag2 ? new LocalizedString("BuyThisCarWindow.Exchange") : new LocalizedString("BuyThisCarWindow.Buy"));
		int[] array = BitMaskPacker.UnpackTuningData(shortTuningData);
		for (int i = 0; i < array.Length; i++)
		{
			CarTuningExchangeUIElements[i].Init(array[i]);
		}
		NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
		numberFormatInfo.NumberGroupSeparator = " ";
		TextPrice.text = price.ToString("N0", numberFormatInfo);
		LoadCarIconForPurchase(carId);
		ButtonBuy.onClick.AddListener(() =>
		{
			if (!FlagsStorage.InGarageFree)
			{
				TrySetResult(new ResultData(CodeType.Success, RequestAnswer.Confirm));
				callback?.Invoke(RequestAnswer.Confirm);
			}
			else
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("BuyThisCarWindow.Youcan'texchangeacarinthegarage"), 3f).Forget();
			}
			CloseAndActivateOther();
		});
		if (showIgnore)
		{
			ButtonIgnore.gameObject.SetActive(value: true);
			ButtonIgnore.onClick.AddListener(() =>
			{
				if (!FlagsStorage.InGarageFree)
				{
					TrySetResult(new ResultData(CodeType.Success, RequestAnswer.Ignore));
					callback?.Invoke(RequestAnswer.Ignore);
				}
				CloseAndActivateOther();
			});
		}
		else
		{
			ButtonIgnore.gameObject.SetActive(value: false);
		}
		if (flag)
		{
			CarTuningExchangeUIElement[] disableIfRotor = DisableIfRotor;
			for (int num = 0; num < disableIfRotor.Length; num++)
			{
				disableIfRotor[num].SetActive(active: false);
			}
		}
		else
		{
			CarTuningExchangeUIElements[6].SetActive(active: false);
		}
		ButtonDetails.onClick.AddListener(() =>
		{
			if (FlagsStorage.InGarageFree)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("BuyThisCarWindow.Youcan'texchangeacarinthegarage"), 3f).Forget();
				CloseAndActivateOther();
			}
			else
			{
				ButtonDetails.gameObject.SetActive(value: false);
				ButtonIgnore.gameObject.SetActive(value: false);
				ShowUI(state: false);
				_carTradeMoreInfoAnalytic.LogMoreInfoClicked(_tradeOfferID, _logFromOfferCreator);
				_virtualPurchaseAnalytic.LogPurchaseMoreInfo(_tradeOfferID, PurchaseStatus.PURCHASE_CLICKED);
				if (!_currencyManagerController.IsEnoughCoins(CarDetailsPrice))
				{
					_virtualPurchaseAnalytic.LogPurchaseMoreInfo(_tradeOfferID, PurchaseStatus.INSUFFICIENT_FUNDS);
				}
				if (!IsCarOwnerInRoom())
				{
					NotifyCarOwnerLeft();
				}
				else
				{
					_managerUI.ShowConfirmWindow(() =>
					{
						_virtualPurchaseAnalytic.LogPurchaseMoreInfo(_tradeOfferID, PurchaseStatus.PURCHASE_CONFIRMED);
						GetDetails().Forget();
					}, new LocalizedString("BuyThisCarWindow.GetCarDescription"), new LocalizedString("BuyThisCarWindow.DoYouWantToGetDetailedCarDescr"), () =>
					{
						_carTradeMoreInfoAnalytic.LogMoreInfoCanceled(_tradeOfferID, _logFromOfferCreator);
						_virtualPurchaseAnalytic.LogPurchaseMoreInfo(_tradeOfferID, PurchaseStatus.PURCHASE_CANCELED_BY_USER);
						RestorePurchaseButtons();
					}).Forget();
				}
			}
		});
		ButtonClose.onClick.AddListener(() =>
		{
			if (!FlagsStorage.InGarageFree)
			{
				TrySetResult(new ResultData(CodeType.Success, RequestAnswer.Decline));
				callback?.Invoke(RequestAnswer.Decline);
			}
			CloseAndActivateOther();
		});
		async UniTaskVoid GetDetails()
		{
			if (!IsCarOwnerInRoom())
			{
				NotifyCarOwnerLeft();
			}
			else
			{
				AdvancedInt price2 = new AdvancedInt(CarDetailsPrice);
				(await _currencyManagerController.TryCoinsSpend(SpendCoinsType.DetailsSellingCar, price2, () =>
				{
					GetDetailsAsync().Forget();
				}, null, allowZero: false)).Get(out var value);
				if (value)
				{
					_carTradeMoreInfoAnalytic.LogPurchased(_tradeOfferID, _logFromOfferCreator);
					_virtualPurchaseAnalytic.LogPurchaseMoreInfo(_tradeOfferID, PurchaseStatus.PURCHASE_COMPLETED);
				}
				else
				{
					_carTradeMoreInfoAnalytic.LogInsufficientHard(_tradeOfferID, _logFromOfferCreator);
					_virtualPurchaseAnalytic.LogPurchaseMoreInfo(_tradeOfferID, PurchaseStatus.PURCHASE_CANCELED_BY_USER);
				}
				if (!value)
				{
					ButtonDetails.SetActive(active: true);
					if (showIgnore)
					{
						ButtonIgnore.gameObject.SetActive(value: true);
					}
				}
				ShowUI(state: true);
			}
		}
		void NotifyCarOwnerLeft()
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("PlayersListControl.Playeralreadylefttheroom"), 3f, NotificationType.Error).Forget();
			_virtualPurchaseAnalytic.LogPurchaseMoreInfo(_tradeOfferID, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
			RestorePurchaseButtons();
		}
		void RestorePurchaseButtons()
		{
			ShowUI(state: true);
			ButtonDetails.gameObject.SetActive(value: true);
			if (showIgnore)
			{
				ButtonIgnore.gameObject.SetActive(value: true);
			}
		}
	}

	private bool IsCarOwnerInRoom()
	{
		if (_carOwnerPlayer != null && PhotonNetwork.InRoom)
		{
			return PhotonNetwork.CurrentRoom.GetPlayer(_carOwnerPlayer.ActorNumber) != null;
		}
		return false;
	}

	private async UniTask GetDetailsAsync()
	{
		RpcRrResponseCarDetails rpcRrResponseCarDetails = await _rpcRrService.Send<RpcRrResponseCarDetails>(_carOwnerPlayer, RpcRrRequestObjectBase.CodeType.CarDetails, RpcRrRequestCarDetails.GetRequestData(), 20f, base.destroyCancellationToken);
		if (!base.destroyCancellationToken.IsCancellationRequested && rpcRrResponseCarDetails != null && rpcRrResponseCarDetails.Code != RpcRrResponseObjectBase.CodeType.Fail && rpcRrResponseCarDetails.CarTuningData != null)
		{
			OnCarDetailsReceived(rpcRrResponseCarDetails.CarTuningData);
		}
	}

	private async UniTaskVoid RebuildLayout()
	{
		await UniTask.Yield(this.GetCancellationTokenOnDestroy());
		LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutParent);
	}

	private void LoadCarIconForPurchase(int carId)
	{
		ImageCar.sprite = Resources.Load<Sprite>($"UI/CarImages/b{carId}");
	}
}
