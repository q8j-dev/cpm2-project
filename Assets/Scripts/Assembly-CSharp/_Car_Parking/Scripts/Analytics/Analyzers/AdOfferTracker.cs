using System;
using CPM.Ads;
using CPM.Ads.Policy;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	public sealed class AdOfferTracker : IDisposable
	{
		private readonly AdsAnalytic _owner;

		private readonly AdPlacement _placement;

		private readonly string _windowId;

		private string _adId;

		private bool _entered;

		private bool _live;

		private bool _accepted;

		private string _gameMode;

		private string _raceMode;

		private int _paidPrice;

		public string AdId => _adId;

		internal AdOfferTracker(AdsAnalytic owner, AdPlacement placement, string windowId)
		{
			_owner = owner;
			_placement = placement;
			_windowId = windowId;
		}

		public void SetRaceMode(string raceMode)
		{
			_raceMode = raceMode;
		}

		public void Present(OfferState state, int paidPrice = 0)
		{
			if (state.Availability != OfferAvailability.Hidden && !_live && (!_entered || state.CanShowNow))
			{
				_adId = Guid.NewGuid().ToString("N");
				_entered = true;
				_accepted = false;
				_paidPrice = paidPrice;
				_gameMode = AdsAnalytic.CurrentGameMode();
				if (state.CanShowNow)
				{
					_live = true;
					Send("OFFERED");
				}
				else
				{
					Send("CAP_REACHED", BlockedReason(state.Availability));
				}
			}
		}

		public void Accepted()
		{
			if (_live)
			{
				_accepted = true;
				_owner.SetShowing(this);
				Send("ACCEPTED");
			}
		}

		public void DeclinedClosed()
		{
			if (_live && !_accepted)
			{
				Send("DECLINED", "closed");
				Close();
			}
		}

		public void DeclinedPaid(int price)
		{
			if (_live && !_accepted)
			{
				_paidPrice = price;
				Send("DECLINED", "paid_hard");
				Close();
			}
		}

		internal void Impression(AdImpressionInfo info)
		{
			if (!_live)
			{
				return;
			}
			Send("IMPRESSION", null, null, (AdActionContext context) =>
			{
				context.AdNetwork = info.Network;
				context.AdCreativeId = info.CreativeId;
				context.SourceUnitId = info.SourceUnitId;
				context.AdRevenuePrecision = info.RevenuePrecision;
				if (info.RevenueDisclosed)
				{
					context.AdRevenueUsd = info.RevenueUsd;
					context.AdCpm = info.Cpm;
				}
			});
		}

		public void ShowFinished(OfferResult result)
		{
			if (_live)
			{
				_owner.ClearShowing(this);
				switch (result.Outcome)
				{
				case OfferOutcome.Granted:
					Send("COMPLETED");
					return;
				case OfferOutcome.Skipped:
					Send("SKIPPED");
					break;
				case OfferOutcome.Failed:
					Send((result.Cause == AdFailureCause.NoFill) ? "NO_FILL" : "LOAD_FAILED", null, result.Cause.ToString());
					break;
				default:
					Send("CAP_REACHED", BlockedReason(result.StateAfter.Availability));
					break;
				}
				Close();
			}
		}

		public void RewardGranted(string rewardType = null, int? amount = null, int? rewardSoft = null, int? rewardHard = null, string serverAdId = null)
		{
			if (_live)
			{
				Send("REWARD_GRANTED", null, null, (AdActionContext context) =>
				{
					context.RewardId = _placement.ToString();
					context.RewardType = rewardType;
					context.Amount = amount;
					context.RewardSoft = rewardSoft;
					context.RewardHard = rewardHard;
					context.RewardReceived = true;
					context.AdServerId = serverAdId;
				});
				Close();
			}
		}

		public void RewardFailed(string errorCode, string errorMessage = null, string serverAdId = null)
		{
			if (_live)
			{
				Send("REWARD_FAILED", null, errorCode, (AdActionContext context) =>
				{
					context.RewardId = _placement.ToString();
					context.ErrorMessage = errorMessage;
					context.AdServerId = serverAdId;
				});
				Close();
			}
		}

		public void Dispose()
		{
			DeclinedClosed();
		}

		private void Close()
		{
			_live = false;
			_accepted = false;
			_owner.ClearShowing(this);
		}

		private static string BlockedReason(OfferAvailability availability)
		{
			return availability switch
			{
				OfferAvailability.ExhaustedToday => "daily_cap", 
				OfferAvailability.ExhaustedSession => "session_cap", 
				OfferAvailability.ExhaustedContext => "context_cap", 
				OfferAvailability.Cooldown => "cooldown", 
				OfferAvailability.NoFill => "no_fill_lock", 
				_ => "not_ready", 
			};
		}

		private void Send(string status, string result = null, string errorCode = null, Action<AdActionContext> configure = null)
		{
			AdActionContext adActionContext = new AdActionContext
			{
				Status = status,
				AdId = _adId,
				AdPlacement = _placement.ToString(),
				AdFormat = "REWARDED",
				Result = result,
				ErrorCode = errorCode,
				GameMode = _gameMode,
				WindowId = _windowId,
				RaceMode = _raceMode
			};
			if (_paidPrice > 0)
			{
				adActionContext.Price = _paidPrice;
				adActionContext.PriceCurrency = CurrencyType.HARD.ToString();
			}
			configure?.Invoke(adActionContext);
			_owner.Send(adActionContext);
		}
	}
}
