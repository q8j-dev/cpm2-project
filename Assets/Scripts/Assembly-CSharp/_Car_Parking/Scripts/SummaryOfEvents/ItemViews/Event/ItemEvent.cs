using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.SummaryOfEvents.ItemViews.Event
{
	public class ItemEvent : SummaryOfEventItem
	{
		[SerializeField]
		private List<Toggle> CheckPoints = new List<Toggle>();

		[SerializeField]
		private ProgressDisplayMode ProgressDisplay = default;

		[SerializeField]
		private TextMeshProUGUI ProgressText = default;

		[SerializeField]
		private List<Image> ProgressIcons = new List<Image>();

		[SerializeField]
		private Sprite FilledIconSprite = default;

		[SerializeField]
		private Sprite EmptyIconSprite = default;

		[SerializeField]
		private GameObject Claim = default;

		[SerializeField]
		private GameObject Claimed = default;

		[SerializeField]
		private TextMeshProUGUI TimeRemainingText = default;

		private IDisposable _timerSubscription;

		private _Car_Parking.Scripts.EventSystem.Data.Event _event;

		private ServerTimeService _serverTimeService;

		private TimeUnitSuffixes _timeUnitSuffixes;

		[field: SerializeField]
		public _Car_Parking.Scripts.EventSystem.EventType EventType { get; private set; }

		protected ServerTimeService ServerTime => _serverTimeService;

		protected _Car_Parking.Scripts.EventSystem.Data.Event Event => _event;

		public void Initialize(_Car_Parking.Scripts.EventSystem.Data.Event @event, ServerTimeService serverTimeService)
		{
			_event = @event;
			_serverTimeService = serverTimeService;
			_timeUnitSuffixes = TextExtensions.CreateTimeUnitSuffixes();
			CheckPointsInitialize(@event);
			RefreshProgress();
			ApplyClaimState(@event);
			StartTimer();
			UpdateTimer();
			IItemEventView[] componentsInChildren = GetComponentsInChildren<IItemEventView>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].UpdateView(@event);
			}
		}

		protected virtual void ApplyClaimState(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			SetClaimVisible(@event.Status == EventStatus.Finished);
			SetClaimedVisible(@event.Status == EventStatus.Claimed);
		}

		protected void SetClaimVisible(bool visible)
		{
			if ((bool)Claim)
			{
				Claim.SetActive(visible);
			}
		}

		protected void SetClaimedVisible(bool visible)
		{
			if ((bool)Claimed)
			{
				Claimed.SetActive(visible);
			}
		}

		private void CheckPointsInitialize(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			if (CheckPoints != null)
			{
				int num = 1;
				for (int i = 0; i < CheckPoints.Count; i++)
				{
					CheckPoints[i].isOn = num <= @event.Progress;
					num++;
				}
			}
		}

		protected virtual (int Current, int Max) GetProgressValues(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			return (Current: @event.Progress, Max: @event.MaxProgress);
		}

		protected void RefreshProgress()
		{
			if (_event == null)
			{
				return;
			}
			(int Current, int Max) progressValues = GetProgressValues(_event);
			int item = progressValues.Current;
			int item2 = progressValues.Max;
			bool flag = ProgressDisplay == ProgressDisplayMode.Icons && CanUseProgressIcons(item, item2);
			if (flag)
			{
				UpdateProgressIcons(item, item2);
			}
			else
			{
				SetProgressIconsVisible(visible: false);
			}
			if ((bool)ProgressText)
			{
				ProgressText.gameObject.SetActive(!flag && item2 > 0);
				if (!flag)
				{
					ProgressText.text = $"{item}/{item2}";
				}
			}
		}

		private bool CanUseProgressIcons(int current, int max)
		{
			if (ProgressIcons == null || ProgressIcons.Count == 0 || !FilledIconSprite || !EmptyIconSprite)
			{
				return false;
			}
			if (!ProgressIconLayoutUtility.CanShowIcons(ProgressIcons.Count, max))
			{
				return false;
			}
			return true;
		}

		private void UpdateProgressIcons(int current, int max)
		{
			for (int i = 0; i < ProgressIcons.Count; i++)
			{
				Image image = ProgressIcons[i];
				if ((bool)image)
				{
					ProgressIconState state = ProgressIconLayoutUtility.GetState(i, current, max);
					image.gameObject.SetActive(state != ProgressIconState.Hidden);
					if (state != ProgressIconState.Hidden)
					{
						image.sprite = ((state == ProgressIconState.Filled) ? FilledIconSprite : EmptyIconSprite);
					}
				}
			}
		}

		private void SetProgressIconsVisible(bool visible)
		{
			if (ProgressIcons == null)
			{
				return;
			}
			foreach (Image progressIcon in ProgressIcons)
			{
				if ((bool)progressIcon)
				{
					progressIcon.gameObject.SetActive(visible);
				}
			}
		}

		private void StartTimer()
		{
			if ((bool)TimeRemainingText)
			{
				_timerSubscription?.Dispose();
				_timerSubscription = Observable.Interval(TimeSpan.FromSeconds(1.0)).StartWith(0L).Subscribe((long _) =>
				{
					UpdateTimer();
				})
					.AddTo(this);
			}
		}

		private void UpdateTimer()
		{
			if (_event != null && _serverTimeService != null && (bool)TimeRemainingText)
			{
				TimeSpan remaining = TimeFormatter.RemainingTime(_event.EndTime, _serverTimeService.CurrentServerUnixTimeSec);
				TimeRemainingText.text = TimeFormatter.FormatRemainingShort(remaining, in _timeUnitSuffixes);
			}
		}
	}
}
