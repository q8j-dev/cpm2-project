using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SummaryOfEvents.ItemViews;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public abstract class EventView : MonoBehaviour, IBigBannerView
	{
		protected const string ViewingProgressCacheKey = "event_progress_";

		[SerializeField]
		private Ease ClaimAnimation;

		[SerializeField]
		private TextMeshProUGUI TimeRemainingText;

		[SerializeField]
		private TextMeshProUGUI NameText;

		[SerializeField]
		private TextMeshProUGUI DescriptionText;

		[SerializeField]
		protected Button ClaimRewardButton;

		[SerializeField]
		private Button CloseButton;

		private ServerTimeService _serverTimeService;

		protected EventsAnalytic _eventsAnalytic;

		private IDisposable _timerSubscription;

		public static Action Claimed;

		public static Action Closed;

		private TimeUnitSuffixes _timeUnitSuffixes;

		[field: SerializeField]
		protected GameObject ClaimedPanel { get; private set; }

		[field: SerializeField]
		protected CanvasGroup ClaimButtonAnimationParent { get; private set; }

		[field: SerializeField]
		public EventType EventType { get; private set; }

		protected _Car_Parking.Scripts.EventSystem.Data.Event Event { get; private set; }

		protected IEventService EventService { get; private set; }

		protected virtual bool ShowClaimButton => Event.Status == EventStatus.Finished;

		[Inject]
		public void Construct(ServerTimeService serverTimeService, IEventService eventService, EventsAnalytic eventsAnalytic)
		{
			_serverTimeService = serverTimeService;
			EventService = eventService;
			_eventsAnalytic = eventsAnalytic;
		}

		protected virtual void Awake()
		{
			_timeUnitSuffixes = TextExtensions.CreateTimeUnitSuffixes();
			StartTimer();
		}

		protected virtual void OnEnable()
		{
			ClaimRewardButton?.onClick.AddListener(OnClaim);
			CloseButton?.onClick.AddListener(OnClosed);
		}

		protected virtual void OnDisable()
		{
			ClaimRewardButton?.onClick.RemoveListener(OnClaim);
			CloseButton?.onClick.RemoveListener(OnClosed);
		}

		public virtual void Init(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			ClaimedPanel.SetActive(value: false);
			Event = @event;
			string key = "event_first_show_" + Event.Type;
			if (PlayerPrefs.GetInt(key, 0) == 0)
			{
				PlayerPrefs.SetInt(key, 1);
				SendFirstShowEvent();
			}
			IItemEventView[] componentsInChildren = GetComponentsInChildren<IItemEventView>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].UpdateView(@event);
			}
			if ((bool)NameText)
			{
				NameText.text = new LocalizedString(Event.NameLocalizeKey);
			}
			if ((bool)DescriptionText)
			{
				DescriptionText.text = new LocalizedString(Event.DescriptionLocalizeKey);
			}
		}

		protected virtual void SendFirstShowEvent()
		{
			_eventsAnalytic.LogEventFirstShow(Event);
		}

		private void StartTimer()
		{
			_timerSubscription?.Dispose();
			_timerSubscription = Observable.Interval(TimeSpan.FromSeconds(1.0)).StartWith(0L).Subscribe((long _) =>
			{
				UpdateTimer();
			})
				.AddTo(this);
		}

		public void ClaimButtonAnimation()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ClaimAppearSwitchState, base.gameObject);
			UIAnimator.SetActiveWithPopFade(ClaimButtonAnimationParent.gameObject, show: true, 0.25f, ClaimAnimation);
		}

		private void OnClaim()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ClaimRewardSwitchState, base.gameObject);
			TryClaimRewardAsync().Forget();
		}

		private void OnClosed()
		{
			Closed?.Invoke();
		}

		private async UniTaskVoid TryClaimRewardAsync()
		{
			if ((await EventService.ClaimReward(Event.Type)).Success)
			{
				Claimed?.Invoke();
			}
		}

		protected async UniTask SetupBanner()
		{
			int cachedProgress = PlayerPrefs.GetInt("event_progress_" + Event.Type, 0);
			PlayerPrefs.SetInt("event_progress_" + Event.Type, Event.Progress);
			CancellationToken token = this.GetCancellationTokenOnDestroy();
			await SetProgress(cachedProgress, token);
			if (!token.IsCancellationRequested)
			{
				ClaimButtonAnimationParent.gameObject.SetActive(ShowClaimButton);
				if (Event.Status == EventStatus.Finished && cachedProgress != Event.Progress)
				{
					ClaimButtonAnimation();
				}
				if (Event.Status == EventStatus.Claimed)
				{
					SetupClaimedPanel();
				}
			}
		}

		protected virtual void SetupClaimedPanel()
		{
			ClaimedPanel?.SetActive(value: true);
		}

		protected void UpdateTimer()
		{
			if (Event != null && _serverTimeService != null)
			{
				TimeSpan remaining = TimeFormatter.RemainingTime(Event.EndTime, _serverTimeService.CurrentServerUnixTimeSec);
				TimeRemainingText.text = TimeFormatter.FormatRemainingFull(remaining, in _timeUnitSuffixes);
			}
		}

		protected virtual async UniTask SetProgress(int cachedProgress, CancellationToken token)
		{
		}

		public void InitBanner(IBigBannerView.BannerData bannerData)
		{
			Init(EventService.GetEventByName(bannerData.EventType));
		}

		public bool IsCorrectPrefab(IBigBannerView.BannerData bannerData)
		{
			if (bannerData == null || bannerData.EventType == EventType.None)
			{
				return false;
			}
			return bannerData.EventType == EventType;
		}

		public void Shown()
		{
		}
	}
}
