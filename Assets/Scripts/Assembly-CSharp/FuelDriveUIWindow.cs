using System;
using System.Threading;
using CPM.Ads;
using CPM.Ads.Policy;
using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using RacingScripts.RaceControllers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Ads.UI;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Wallet;

public class FuelDriveUIWindow : WindowScript, IBackableWindow, IPointerDownHandler, IEventSystemHandler
{
	private const float TimeLoadKof = 0.05f;

	[SerializeField]
	private FuelTankGadget FuelTankGadget = default;

	[SerializeField]
	private GameObject MainPanel = default;

	[SerializeField]
	private GameObject ConfirmationPanel = default;

	[SerializeField]
	private GameObject LoadingPanel = default;

	[SerializeField]
	private GameObject FuelPanel = default;

	[SerializeField]
	private Button BuyFuelButton = default;

	[SerializeField]
	private TextMeshProUGUI FullFuelPriceText = default;

	[SerializeField]
	private Button WatchAdButton = default;

	[SerializeField]
	private GameObject WatchAdPanel = default;

	[SerializeField]
	private CanvasGroup WatchAdCanvasGroup = default;

	[SerializeField]
	private TMP_Text WatchAdCooldownLabel = default;

	[SerializeField]
	private Button CancelButton = default;

	[SerializeField]
	private Button CancelAdButton = default;

	[SerializeField]
	private Button CloseButton = default;

	[SerializeField]
	private Image ProgressImage = default;

	[SerializeField]
	private float Duration = 1.5f;

	[SerializeField]
	private GameObject RewardedLoading = default;

	[SerializeField]
	private GameObject HideWhenLoadingRewarded = default;

	private const int DelayStep = 10;

	private const int FullTankCoinCap = 100;

	private const float DisabledAdTextAlpha = 0.4f;

	private OptionalDependency<LevelScript> _levelScript;

	private CircuitRacingUIControl _circuitRacingUIControl;

	private CircuitRaceControlller _circuitRaceControlller;

	private OptionalDependency<RaceModeController> _raceModeController;

	private IAdOfferService _adOffers;

	private CurrencyManagerController _currencyManagerController;

	private ICarInfoService _carInfoService;

	private IClientAdRewardConsumeService _adRewardConsume;

	private AdsAnalytic _adsAnalytic;

	private AdOfferTracker _adTracker;

	private bool _rewardedInProgress;

	private bool _fullTankPurchaseInProgress;

	private AdCooldownTimerView _watchAdCooldownTimer;

	private readonly AdOfferMessages _adMessages = new AdOfferMessages();

	private string _contextId;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	private AdCooldownTimerView WatchAdCooldownTimer => _watchAdCooldownTimer ?? (_watchAdCooldownTimer = new AdCooldownTimerView(WatchAdCooldownLabel));

	private bool IsLapRacingSceneActive => ScenesHelper.IsLapRacingSceneActive();

	[Inject]
	public void Construct(OptionalDependency<LevelScript> levelScript, OptionalDependency<RaceModeController> raceModeController, IAdOfferService adOffers, CurrencyManagerController currencyManagerController, ICarInfoService carInfoService, IClientAdRewardConsumeService adRewardConsume, AdsAnalytic adsAnalytic)
	{
		_levelScript = levelScript;
		_raceModeController = raceModeController;
		_adOffers = adOffers;
		_currencyManagerController = currencyManagerController;
		_carInfoService = carInfoService;
		_adRewardConsume = adRewardConsume;
		_adsAnalytic = adsAnalytic;
	}

	protected override void OnEnable()
	{
		CloseButton.SetActive(active: false);
		base.OnEnable();
	}

	protected override void Start()
	{
		InitBackButton();
		base.Start();
		InitRefsIfNeed();
		FuelTankGadget.Bought += OnFuelChangeConfirm;
		BuyFuelButton.onClick.AddListener(BuyFullTank);
		RefreshFullTankPrice();
		if ((bool)WatchAdButton)
		{
			WatchAdButton.onClick.AddListener(WatchAdForFuel);
		}
		else
		{
			Debug.LogWarning("[Ads] FuelDriveUIWindow.WatchAdButton is not wired on the FuelDriveUIWindow prefab - the rewarded-ad refuel option will not be shown until it is assigned.");
		}
		CancelAdButton.onClick.AddListener(OnRewardedClosed);
		CloseButton.onClick.AddListener(Close);
		if (IsLapRacingSceneActive)
		{
			InitFuelPanel(onlyCurrentBenzin: true);
			CancelButton.onClick.AddListener(CloseInCircuitRace);
		}
		else
		{
			CancelButton.onClick.AddListener(Close);
			if (!FlagsStorage.IsFreeFuelWindow)
			{
				InitFuelPanel(onlyCurrentBenzin: false);
			}
		}
		_contextId = Guid.NewGuid().ToString("N");
		_adTracker = _adsAnalytic.Track(AdPlacement.FuelRefill, "FuelDriveUIWindow");
		RefreshAdButton();
		PollAdButtonAsync(this.GetCancellationTokenOnDestroy()).Forget();
	}

	private async UniTaskVoid PollAdButtonAsync(CancellationToken token)
	{
		try
		{
			while (true)
			{
				await UniTask.Delay(1000, ignoreTimeScale: true, PlayerLoopTiming.Update, token);
				if (!_rewardedInProgress)
				{
					RefreshAdButton();
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		Close();
	}

	private void ResetAllWindows()
	{
		ConfirmationPanel.SetActive(value: false);
		FuelPanel.SetActive(value: false);
		MainPanel.SetActive(value: false);
		LoadingPanel.SetActive(value: false);
	}

	private void InitFuelPanel(bool onlyCurrentBenzin)
	{
		ResetAllWindows();
		ConfirmationPanel.gameObject.SetActive(value: true);
		FuelPanel.SetActive(value: true);
		FuelTankGadget.Init("FuelDriveUI", onlyCurrentBenzin);
	}

	private void OnFuelChangeConfirm()
	{
		if (FuelTankGadget.CanBuy)
		{
			ShowConfirmWindow(FuelTankGadget.FuelToLoad);
			if ((bool)_circuitRacingUIControl && (bool)_circuitRacingUIControl.CircuitPlayWindow)
			{
				_circuitRacingUIControl.CircuitPlayWindow.ShowCrashPanel(state: false, 0f);
			}
		}
	}

	private void ShowConfirmWindow(float fuelToLoad)
	{
		ResetAllWindows();
		ConfirmationPanel.SetActive(value: true);
		LoadingPanel.SetActive(value: true);
		StartProgress(fuelToLoad).Forget();
	}

	private async UniTaskVoid StartProgress(float fuelToLoad)
	{
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		if (IsLapRacingSceneActive)
		{
			_circuitRaceControlller.RaceCrewTalkSoundController.PlayFuelLoopSound();
			float timeLoad = Mathf.Abs(fuelToLoad) * 0.05f;
			float timeNow = 0f;
			while (timeNow < timeLoad)
			{
				timeNow += Time.unscaledDeltaTime;
				ProgressImage.fillAmount = timeNow / timeLoad;
				await UniTask.NextFrame(token);
			}
			_circuitRaceControlller.RaceCrewTalkSoundController.StopFuelLoopSound();
			_circuitRaceControlller.RaceCrewTalkSoundController.PlayFuelOutSound();
		}
		else
		{
			for (int i = 0; i < 100; i++)
			{
				await UniTask.Delay(10, ignoreTimeScale: true, PlayerLoopTiming.Update, token);
				ProgressImage.fillAmount = (float)i * 0.1f;
			}
		}
		_managerUI.ShowNotificationWindow(new LocalizedString("LevelLoader.Done"), 2f).Forget();
		if (IsLapRacingSceneActive)
		{
			CloseInCircuitRace();
		}
		else
		{
			Close();
		}
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			base.Close();
			if ((bool)_levelScript.Value)
			{
				_levelScript.Value.Resume();
			}
			ReleaseGasPistol();
			FlagsStorage.IsFreeFuelWindow = false;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_adTracker?.Dispose();
	}

	private void ReleaseGasPistol()
	{
		GameObject gameObject = (V.Car ? V.Car : V.LastUsedCar);
		if ((bool)gameObject && gameObject.TryGetComponent<VehicleData>(out var component))
		{
			FuelTankEntity fuelTank = component.CarChildsReferences.FuelTank;
			if ((bool)fuelTank)
			{
				fuelTank.ReleaseGasPistol();
			}
		}
	}

	private void WatchAdForFuel()
	{
		WatchAdForFuelAsync().Forget();
	}

	private async UniTaskVoid WatchAdForFuelAsync()
	{
		if (_rewardedInProgress)
		{
			return;
		}
		_rewardedInProgress = true;
		SetRewardedLoading(loading: true);
		_managerUI.ShowWaitingWindow(set: true).Forget();
		_adTracker?.Accepted();
		try
		{
			int slotId = V.SlotID;
			if (_carInfoService.TryGetFromSlot(slotId, out var carInfo))
			{
				PendingAdRewardPrefs.Save(AdPlacement.FuelRefill, slotId, carInfo.CarID);
			}
			PendingAdRewardPrefs.SaveAdId(AdPlacement.FuelRefill, _adTracker?.AdId);
			OfferResult result = await _adOffers.TryShowAsync(AdPlacement.FuelRefill, _contextId);
			_adTracker?.ShowFinished(result);
			if (!result.IsGranted)
			{
				PendingAdRewardPrefs.Clear(AdPlacement.FuelRefill);
				Debug.LogWarning($"[Ads] Fuel rewarded ad not granted: {result.Outcome} ({result.Cause})");
				if ((bool)this)
				{
					HandleNoReward(result);
					RefreshAdButton();
				}
				return;
			}
			bool flag = TryFillTank(out var fuelLoaded);
			if (flag)
			{
				_adTracker?.RewardGranted("Fuel", Mathf.RoundToInt(fuelLoaded));
				PendingAdRewardPrefs.Clear(AdPlacement.FuelRefill);
				_adRewardConsume.ConsumeInBackground(AdPlacement.FuelRefill);
			}
			if ((bool)this)
			{
				if (!flag)
				{
					Debug.LogError("[Ads] Fuel rewarded ad was granted but the tank could not be filled: no active car with a fuel tank. The refill is deferred to the recovery on next launch.");
					_adTracker?.RewardFailed("GRANT_ERROR", "no active car with a fuel tank");
					OnRewardedClosed();
					_managerUI.ShowNotificationWindow(new LocalizedString("DebugAccountData.SomethingWentWrong"), 3f, NotificationType.Error).Forget();
				}
				else
				{
					SetRewardedLoading(loading: false);
					RefreshAdButton();
					ShowConfirmWindow(fuelLoaded);
				}
			}
		}
		finally
		{
			_rewardedInProgress = false;
			if ((bool)_managerUI)
			{
				_managerUI.CloseWaitingWindow();
			}
		}
	}

	private void HandleNoReward(OfferResult result)
	{
		OnRewardedClosed();
		if (result.Outcome == OfferOutcome.Blocked)
		{
			ShowBlockedReason(result.StateAfter);
		}
		else if (result.Blame == AdFailureBlame.Player)
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("GarageMenuWindow.NoInternetConnection"), 3f).Forget();
		}
	}

	private void ShowBlockedReason(OfferState state)
	{
		if (_adMessages.TryGetBlockedKey(state, out var key))
		{
			_managerUI.ShowNotificationWindow(new LocalizedString(key), 3f).Forget();
		}
	}

	private bool TryFillTank(out float fuelLoaded)
	{
		fuelLoaded = 0f;
		GameObject gameObject = V.Car;
		if (gameObject == null)
		{
			gameObject = V.LastUsedCar;
		}
		if (!gameObject || !gameObject.TryGetComponent<VehicleData>(out var component))
		{
			return false;
		}
		FuelTankEntity fuelTank = component.CarChildsReferences.FuelTank;
		if (!fuelTank)
		{
			return false;
		}
		fuelLoaded = Mathf.Max(0f, fuelTank.TankCapacity - fuelTank.CurrentFuel);
		fuelTank.FullFuelImmediately();
		return true;
	}

	private bool TryGetTankInfo(out int capacity, out FuelType fuelType)
	{
		capacity = 0;
		fuelType = FuelType.Gas95;
		GameObject gameObject = (V.Car ? V.Car : V.LastUsedCar);
		if (!gameObject || !gameObject.TryGetComponent<VehicleData>(out var component))
		{
			return false;
		}
		FuelTankEntity fuelTank = component.CarChildsReferences.FuelTank;
		if (!fuelTank)
		{
			return false;
		}
		capacity = Mathf.RoundToInt(fuelTank.TankCapacity);
		if (gameObject.TryGetComponent<VehicleEntityProvider>(out var component2))
		{
			fuelType = component2.CarEntity.Engine.GetBenzin;
		}
		return true;
	}

	private int FullTankCoinPrice()
	{
		if (!TryGetTankInfo(out var capacity, out var fuelType))
		{
			return 0;
		}
		if (fuelType != FuelType.GasE85)
		{
			return Mathf.Min(capacity, 100);
		}
		return capacity;
	}

	private void RefreshFullTankPrice()
	{
		if ((bool)FullFuelPriceText)
		{
			FullFuelPriceText.text = FullTankCoinPrice().ToString();
		}
	}

	private void BuyFullTank()
	{
		if (_fullTankPurchaseInProgress)
		{
			return;
		}
		int price = FullTankCoinPrice();
		if (price > 0)
		{
			string format = new LocalizedString("VisualTuningWindow.Buythisitemfor");
			_managerUI.ShowConfirmWindow(() =>
			{
				BuyFullTankAsync(price).Forget();
			}, new LocalizedString("Playerprofilewindow.Message"), string.Format(format, price)).Forget();
		}
	}

	private async UniTaskVoid BuyFullTankAsync(int price)
	{
		if (_fullTankPurchaseInProgress)
		{
			return;
		}
		_fullTankPurchaseInProgress = true;
		BuyFuelButton.interactable = false;
		try
		{
			await _currencyManagerController.TryCoinsSpend(SpendCoinsType.Others, new AdvancedInt(price), () =>
			{
				OnFullTankBought(price);
			});
		}
		finally
		{
			_fullTankPurchaseInProgress = false;
			if ((bool)this && (bool)BuyFuelButton)
			{
				BuyFuelButton.interactable = true;
			}
		}
	}

	private void OnFullTankBought(int price)
	{
		if (TryFillTank(out var _))
		{
			_adTracker?.DeclinedPaid(price);
			_managerUI.ShowNotificationWindow(new LocalizedString("FuelDriveUIWindow.Success"), 3f).Forget();
			Close();
		}
	}

	private void OnRewardedClosed()
	{
		SetRewardedLoading(loading: false);
	}

	private void SetRewardedLoading(bool loading)
	{
		if ((bool)HideWhenLoadingRewarded)
		{
			HideWhenLoadingRewarded.SetActive(!loading);
		}
		if ((bool)RewardedLoading)
		{
			RewardedLoading.SetActive(loading);
		}
	}

	private void SetWatchAdButtonEnabled(bool enabled)
	{
		if ((bool)WatchAdCanvasGroup)
		{
			WatchAdCanvasGroup.alpha = (enabled ? 1f : 0.4f);
		}
	}

	private void RefreshAdButton()
	{
		if ((bool)WatchAdButton)
		{
			OfferState state = _adOffers.CanOffer(AdPlacement.FuelRefill, _contextId);
			_adTracker?.Present(state, FullTankCoinPrice());
			WatchAdCooldownTimer.SetCooldown(_adMessages.CooldownSeconds(state));
			SetWatchAdButtonEnabled(state.CanShowNow);
			if ((bool)WatchAdPanel)
			{
				WatchAdPanel.SetActive(state.IsOfferable);
			}
			else
			{
				WatchAdButton.gameObject.SetActive(state.IsOfferable);
			}
		}
	}

	private void CloseInCircuitRace()
	{
		if (!base.IsCloseCanceled)
		{
			_circuitRacingUIControl.CircuitPlayWindow.ShowUI(state: true);
			_circuitRacingUIControl.CircuitPlayWindow.WindowCircuitPitStop.Init();
			base.Close();
		}
	}

	private void InitRefsIfNeed()
	{
		if (!_circuitRaceControlller && (bool)_raceModeController.Value && (bool)_raceModeController.Value.SpawnedObject)
		{
			_circuitRaceControlller = _raceModeController.Value.SpawnedObject.GetComponent<CircuitRaceControlller>();
			_circuitRacingUIControl = _raceModeController.Value.SpawnedObject.GetComponent<CircuitRacingUIControl>();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Close();
	}
}
