using System;
using System.Collections.Generic;
using CPM.Ads.Policy;
using CPM.Database;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Ads.UI;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.Drag
{
	public class TuningTimerWindow : WindowScript
	{
		[SerializeField]
		private RectTransform ElementsParent = default;

		[SerializeField]
		private CanvasGroup AdButtonCanvasGroup = default;

		[SerializeField]
		private Button AdButton = default;

		[SerializeField]
		private Button CoinButton = default;

		[SerializeField]
		private Button SkipButton = default;

		[SerializeField]
		private Button CloseButton = default;

		[SerializeField]
		private TuningTimerElement TimerElementPrefab = default;

		[SerializeField]
		private TextMeshProUGUI TotalTimeText = default;

		[SerializeField]
		private TextMeshProUGUI AdCooldownText = default;

		private const int SpendCoin = 100;

		private IPrefabFactory _prefabFactory;

		private IEngineTimerAdService _engineTimerAdService;

		private ITimerTuningService _timerTuningService;

		private IEngineTuningTimerService _engineTuningTimerService;

		private CurrencyManagerController _currencyManagerController;

		private AllParts _allParts;

		private ICarInfoService _carInfoService;

		private AdsAnalytic _adsAnalytic;

		private AdOfferTracker _adTracker;

		private bool _busy;

		private int _benchEngineId = -1;

		private AdCooldownTimerView _adCooldownTimer;

		private readonly AdOfferMessages _adMessages = new AdOfferMessages();

		private readonly List<TuningTimerElement> _elements = new List<TuningTimerElement>();

		private bool BenchTarget => _benchEngineId > 0;

		private AdCooldownTimerView AdCooldownTimer => _adCooldownTimer ?? (_adCooldownTimer = new AdCooldownTimerView(AdCooldownText));

		[Inject]
		public void Construct(IPrefabFactory prefabFactory, IEngineTimerAdService engineTimerAdService, ITimerTuningService timerTuningService, IEngineTuningTimerService engineTuningTimerService, CurrencyManagerController currencyManagerController, AllParts allParts, ICarInfoService carInfoService, AdsAnalytic adsAnalytic)
		{
			_prefabFactory = prefabFactory;
			_engineTimerAdService = engineTimerAdService;
			_timerTuningService = timerTuningService;
			_engineTuningTimerService = engineTuningTimerService;
			_currencyManagerController = currencyManagerController;
			_allParts = allParts;
			_carInfoService = carInfoService;
			_adsAnalytic = adsAnalytic;
		}

		private new void Start()
		{
			_managerUI.IncreaseTimerButtonShow?.Invoke(obj: false);
			int observingEngineId = _engineTuningTimerService.ObservingEngineId;
			if (observingEngineId > 0 && _engineTuningTimerService.TryGetTimerInfo(observingEngineId, out var _))
			{
				_benchEngineId = observingEngineId;
			}
			AdButton.onClick.AddListener(() =>
			{
				FinishByAd().Forget();
			});
			CoinButton.onClick.AddListener(() =>
			{
				FinishByCoins().Forget();
			});
			SkipButton.onClick.AddListener(CloseIfIdle);
			CloseButton.onClick.AddListener(CloseIfIdle);
			_adTracker = _adsAnalytic.Track(AdPlacement.EngineTimerReduce, "TuningTimerWindow");
			FillEntries();
			RefreshAdButton();
			Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				RefreshTimes();
			}).AddTo(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_adTracker?.Dispose();
			if ((bool)_managerUI)
			{
				_managerUI.IncreaseTimerButtonShow?.Invoke(obj: true);
			}
		}

		private void FillEntries()
		{
			for (int num = ElementsParent.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(ElementsParent.GetChild(num).gameObject);
			}
			_elements.Clear();
			foreach (TuningTimerEntry entry in GetEntries())
			{
				TuningTimerElement tuningTimerElement = _prefabFactory.Instantiate(TimerElementPrefab, ElementsParent);
				tuningTimerElement.Init(GetTypeText(entry), GetNameText(entry), FormatWait(entry.RemainingSeconds), entry.RemainingSeconds <= 0.0);
				_elements.Add(tuningTimerElement);
			}
			SetTotal();
		}

		private void RefreshTimes()
		{
			RefreshAdButton();
			List<TuningTimerEntry> entries = GetEntries();
			if (entries.Count != _elements.Count)
			{
				FillEntries();
				return;
			}
			for (int i = 0; i < entries.Count; i++)
			{
				TuningTimerEntry entry = entries[i];
				_elements[i].Init(GetTypeText(entry), GetNameText(entry), FormatWait(entry.RemainingSeconds), entry.RemainingSeconds <= 0.0);
			}
			SetTotal();
		}

		private List<TuningTimerEntry> GetEntries()
		{
			if (!BenchTarget)
			{
				return _timerTuningService.GetTimerEntries(V.SlotID);
			}
			return _engineTuningTimerService.GetTimerEntries(_benchEngineId);
		}

		private bool TryGetTimerInfo(out EngineTimerInfo info)
		{
			if (!BenchTarget)
			{
				return _timerTuningService.TryGetEngineTimerInfo(V.SlotID, out info);
			}
			return _engineTuningTimerService.TryGetTimerInfo(_benchEngineId, out info);
		}

		private void SetTotal()
		{
			string text = FormatWait(TryGetTimerInfo(out var info) ? info.RemainingSeconds : 0.0);
			string text2 = new LocalizedString("MainMenu.InstallationTime");
			TotalTimeText.text = text2 + ": " + text;
		}

		private string GetTypeText(TuningTimerEntry entry)
		{
			if (!entry.IsEngine)
			{
				return entry.Part.GetPartTypeName();
			}
			return new LocalizedString("TuningTimerUi.Engine");
		}

		private string GetNameText(TuningTimerEntry entry)
		{
			PartData partData = (entry.IsEngine ? entry.EngineKit.EngineId : entry.Part);
			if (partData == null)
			{
				return string.Empty;
			}
			PartItem part = _allParts.GetPart(partData);
			if (!(part != null))
			{
				return partData.GetPartTypeName();
			}
			return part.GetName;
		}

		private static string FormatWait(double seconds)
		{
			int num = (int)Math.Round((seconds < 0.0) ? 0.0 : seconds);
			int num2 = num / 3600;
			int num3 = num % 3600 / 60;
			int num4 = num % 60;
			if (num2 > 0)
			{
				return $"{num2}h {num3:D2}m {num4:D2}s";
			}
			if (num3 <= 0)
			{
				return $"{num4}s";
			}
			return $"{num3}m {num4:D2}s";
		}

		private static void ShowSuccess()
		{
			V.CoreGameManager.SetMessage(new LocalizedString("FuelDriveUIWindow.Success"), 2f);
		}

		private static void ShowFailure()
		{
			V.CoreGameManager.SetMessage(new LocalizedString("Error"), 3f, NotificationType.Error);
		}

		private void CloseIfIdle()
		{
			if (!_busy)
			{
				Close();
			}
		}

		private bool TryFinishNow(string timerId)
		{
			if (!BenchTarget)
			{
				return _timerTuningService.TryFinishNow(V.SlotID, timerId);
			}
			return _engineTuningTimerService.TryFinishNow(_benchEngineId, timerId);
		}

		private void RefreshAdButton()
		{
			if (!TryGetTimerInfo(out var info) || !info.IsEngineInstall)
			{
				AdButton.gameObject.SetActive(value: false);
				AdCooldownTimer.SetCooldown(0f);
				return;
			}
			OfferState state = (BenchTarget ? _engineTimerAdService.CanOfferForEngine(_benchEngineId) : _engineTimerAdService.CanOffer(V.SlotID));
			if (!state.IsOfferable)
			{
				AdButton.gameObject.SetActive(value: false);
				AdCooldownTimer.SetCooldown(0f);
				_adTracker?.Present(state, 100);
				return;
			}
			AdButton.gameObject.SetActive(value: true);
			_adTracker?.Present(state, 100);
			if ((bool)AdButtonCanvasGroup)
			{
				AdButtonCanvasGroup.alpha = (state.CanShowNow ? 1f : 0.3f);
			}
			AdCooldownTimer.SetCooldown(_adMessages.CooldownSeconds(state));
		}

		private async UniTaskVoid FinishByAd()
		{
			if (_busy)
			{
				return;
			}
			OfferState state = (BenchTarget ? _engineTimerAdService.CanOfferForEngine(_benchEngineId) : _engineTimerAdService.CanOffer(V.SlotID));
			if (!state.CanShowNow)
			{
				if (_adMessages.TryGetBlockedKey(state, out var key))
				{
					V.CoreGameManager.SetMessage(new LocalizedString(key), 3f);
				}
				return;
			}
			_busy = true;
			_managerUI.ShowWaitingWindow(set: true).Forget();
			_adTracker?.Accepted();
			try
			{
				OfferResult offerResult;
				if (BenchTarget)
				{
					PendingAdRewardPrefs.SaveEngine(_benchEngineId);
					PendingAdRewardPrefs.SaveAdId(AdPlacement.EngineTimerReduce, _adTracker?.AdId);
					offerResult = await _engineTimerAdService.TryReduceEngineAsync(_benchEngineId, _adTracker);
					PendingAdRewardPrefs.ClearEngine();
				}
				else
				{
					int slotId = V.SlotID;
					if (_carInfoService.TryGetFromSlot(slotId, out var carInfo))
					{
						PendingAdRewardPrefs.Save(AdPlacement.EngineTimerReduce, slotId, carInfo.CarID);
					}
					PendingAdRewardPrefs.SaveAdId(AdPlacement.EngineTimerReduce, _adTracker?.AdId);
					offerResult = await _engineTimerAdService.TryReduceAsync(slotId, _adTracker);
					PendingAdRewardPrefs.Clear(AdPlacement.EngineTimerReduce);
				}
				if (offerResult.IsGranted)
				{
					ShowSuccess();
					if ((bool)this)
					{
						Close();
					}
				}
				else if ((bool)this)
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
				_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			}
		}

		private async UniTaskVoid FinishByCoins()
		{
			if (_busy)
			{
				return;
			}
			_busy = true;
			try
			{
				if (!TryGetTimerInfo(out var info))
				{
					Close();
					return;
				}
				string timerId = info.TimerId;
				AdvancedInt price = new AdvancedInt(100);
				(await _currencyManagerController.TryCoinsSpend(SpendCoinsType.Others, price, null, null, allowZero: false)).Get(out var value);
				if (!value)
				{
					return;
				}
				_adTracker?.DeclinedPaid(100);
				if (TryFinishNow(timerId) || !TryGetTimerInfo(out var _))
				{
					ShowSuccess();
					if ((bool)this)
					{
						Close();
					}
					return;
				}
				ShowFailure();
				if ((bool)this)
				{
					FillEntries();
					RefreshAdButton();
				}
			}
			finally
			{
				_busy = false;
			}
		}
	}
}
