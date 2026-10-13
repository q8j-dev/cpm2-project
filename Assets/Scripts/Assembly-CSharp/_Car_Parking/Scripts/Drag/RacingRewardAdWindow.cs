using System;
using System.Threading;
using CPM.Ads.Policy;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Drag
{
	public class RacingRewardAdWindow : WindowScript
	{
		private const int AdRewardMultiplier = 2;

		[SerializeField]
		private CircuitRewardUiElement[] CircuitRewardUiElements;

		[SerializeField]
		private Button AdRewardButton;

		[SerializeField]
		private Button ContinueButton;

		[SerializeField]
		private Button BackButton;

		[SerializeField]
		private GameObject MoneyPanel;

		[SerializeField]
		private TextMeshProUGUI MoneyText;

		[SerializeField]
		private GameObject CoinPanel;

		[SerializeField]
		private TextMeshProUGUI CoinText;

		private IRacingRewardAdService _racingRewardAdService;

		private CurrencyManagerController _currencyManagerController;

		private AdsAnalytic _adsAnalytic;

		private AdOfferTracker _adTracker;

		private RacingRewardMode _mode;

		private string _raceId;

		private bool _busy;

		public event Action<int, int> RewardDoubled;

		[Inject]
		public void Construct(IRacingRewardAdService racingRewardAdService, CurrencyManagerController currencyManagerController, AdsAnalytic adsAnalytic)
		{
			_racingRewardAdService = racingRewardAdService;
			_currencyManagerController = currencyManagerController;
			_adsAnalytic = adsAnalytic;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_adTracker?.Dispose();
		}

		protected override void Start()
		{
			base.Start();
			BackButton.onClick.AddListener(Close);
			ContinueButton.onClick.AddListener(Close);
			AdRewardButton.onClick.AddListener(() =>
			{
				DoubleByAd().Forget();
			});
			Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				if (!string.IsNullOrEmpty(_raceId) && !_busy)
				{
					RefreshAdButton();
				}
			}).AddTo(this);
		}

		public void InitReward(RacingRewardMode mode, int[] rewards, int wonPlaces)
		{
			_mode = mode;
			_raceId = Guid.NewGuid().ToString("N");
			if (_adTracker == null)
			{
				_adTracker = _adsAnalytic.Track(AdPlacement.Competition, "RacingRewardAdWindow");
			}
			_adTracker.SetRaceMode(mode.ToString());
			if (rewards != null)
			{
				int num = Mathf.Min(rewards.Length, CircuitRewardUiElements.Length);
				for (int i = 0; i < num; i++)
				{
					if ((bool)CircuitRewardUiElements[i])
					{
						CircuitRewardUiElements[i].Init(rewards[i]);
					}
				}
			}
			ShowAdRewardCount(rewards, wonPlaces);
			AnimateRewardPlaces(wonPlaces).Forget();
			RefreshAdButton();
		}

		private void ShowAdRewardCount(int[] rewards, int wonPlaces)
		{
			if ((bool)MoneyPanel)
			{
				MoneyPanel.SetActive(value: false);
			}
			if ((bool)CoinPanel)
			{
				CoinPanel.SetActive(value: false);
			}
			RacingRewardAmounts racingRewardAmounts = _racingRewardAdService.SplitWonRewards(rewards, wonPlaces);
			if (racingRewardAmounts.Coins > 0 && (bool)CoinPanel && (bool)CoinText)
			{
				CoinPanel.SetActive(value: true);
				CoinText.text = $"{racingRewardAmounts.Coins * 2}";
			}
			if (racingRewardAmounts.Money > 0 && (bool)MoneyPanel && (bool)MoneyText)
			{
				MoneyPanel.SetActive(value: true);
				MoneyText.text = $"{racingRewardAmounts.Money * 2}";
			}
		}

		private async UniTaskVoid AnimateRewardPlaces(int wonPlaces)
		{
			CancellationToken ct = this.GetCancellationTokenOnDestroy();
			for (int i = 0; i < CircuitRewardUiElements.Length; i++)
			{
				CircuitRewardUiElement circuitRewardUiElement = CircuitRewardUiElements[i];
				if ((bool)circuitRewardUiElement)
				{
					if (i >= wonPlaces)
					{
						await circuitRewardUiElement.LoseAnimation(ct);
					}
					else
					{
						await circuitRewardUiElement.WinAnimation(ct);
					}
					if (!this)
					{
						break;
					}
				}
			}
		}

		private void RefreshAdButton()
		{
			OfferState state = _racingRewardAdService.CanOffer(_raceId);
			_adTracker?.Present(state);
			AdRewardButton.gameObject.SetActive(state.IsOfferable);
			AdRewardButton.interactable = state.CanShowNow;
		}

		private void ReportRewardOutcome(RacingRewardDoubleResult result)
		{
			if (_adTracker != null && result.Offer.IsGranted)
			{
				if (result.RewardGranted)
				{
					AdOfferTracker adTracker = _adTracker;
					int? rewardSoft = ((result.Money > 0) ? new int?(result.Money) : ((int?)null));
					int? rewardHard = ((result.Coins > 0) ? new int?(result.Coins) : ((int?)null));
					string serverAdId = result.ServerAdId;
					adTracker.RewardGranted(null, null, rewardSoft, rewardHard, serverAdId);
				}
				else
				{
					_adTracker.RewardFailed(AdRewardErrorCode.FromClaimStatus(result.ClaimStatus), result.ClaimStatus, result.ServerAdId);
				}
			}
		}

		private void GrantReward(int coins, int money)
		{
			if (coins > 0)
			{
				CurrencyManagerController.SetCoins(new AdvancedInt((int)CurrencyManagerController.Coins + coins));
				CurrencyManagerController.UpdateCoinsVisuals();
			}
			if (money > 0)
			{
				_currencyManagerController.AddMoney(new AdvancedInt(money));
			}
		}

		private async UniTaskVoid DoubleByAd()
		{
			if (_busy)
			{
				return;
			}
			_busy = true;
			_managerUI.ShowWaitingWindow(set: true).Forget();
			_adTracker?.Accepted();
			PendingAdRewardPrefs.SaveAdId(AdPlacement.Competition, _adTracker?.AdId);
			try
			{
				RacingRewardDoubleResult result = await _racingRewardAdService.TryDoubleRewardAsync(_mode, _raceId);
				_adTracker?.ShowFinished(result.Offer);
				ReportRewardOutcome(result);
				if (!this)
				{
					return;
				}
				if (result.Offer.IsGranted)
				{
					AdRewardButton.gameObject.SetActive(value: false);
					if (result.RewardGranted)
					{
						GrantReward(result.Coins, result.Money);
						RewardDoubled?.Invoke(result.Money, result.Coins);
						_managerUI.ShowNotificationWindow(new LocalizedString("EventService.RewardRecieved"), 3f).Forget();
						Close();
					}
					else
					{
						_managerUI.ShowNotificationWindow(new LocalizedString("TradeError.OfferError"), 3f, NotificationType.Error).Forget();
					}
				}
				else
				{
					RefreshAdButton();
				}
			}
			finally
			{
				_busy = false;
				if ((bool)_managerUI)
				{
					_managerUI.CloseWaitingWindow();
				}
			}
		}
	}
}
