using System;
using System.Threading;
using CAS;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace CPM.Ads
{
	public class CasAdsService : IAdsService, IAdsUserIdentity, IInitializable, IDisposable
	{
		private const int PlacementMaxLength = 100;

		private IMediationManager _manager;

		private string _managerUserId;

		private Audience _taggedAudience;

		private UniTaskCompletionSource<RewardedAdOutcome> _pendingRewarded;

		private bool _rewardEarned;

		private AdFailureCause _lastRewardedLoadFailure;

		private string _lastRewardedLoadMessage;

		public bool IsInitialized => _manager != null;

		public AdFailureCause LastRewardedLoadFailure => _lastRewardedLoadFailure;

		public bool IsRewardedReady
		{
			get
			{
				if (_manager == null)
				{
					return false;
				}
				try
				{
					return _manager.IsReadyAd(AdType.Rewarded);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					return false;
				}
			}
		}

		public event Action<AdImpressionInfo> RewardedImpression;

		public void Initialize()
		{
		}

		public void SetUserId(string userId)
		{
			string text = userId ?? string.Empty;
			if (_manager == null || !(_managerUserId == text))
			{
				if (_manager == null)
				{
					BuildManager(text);
				}
				else
				{
					SwitchUserId(text);
				}
			}
		}

		private void SwitchUserId(string userId)
		{
			try
			{
				MobileAds.targetingOptions.userID = userId;
				_managerUserId = userId;
			}
			catch (Exception exception)
			{
				Debug.LogWarning("[Ads] Failed to switch the CAS user id to \"" + userId + "\"; server-side verification keeps reporting \"" + _managerUserId + "\".");
				Debug.LogException(exception);
			}
		}

		public void SetTaggedAudience(int audienceTarget)
		{
			switch (audienceTarget)
			{
			case 1:
				_taggedAudience = Audience.Children;
				break;
			case 2:
				_taggedAudience = Audience.NotChildren;
				break;
			default:
				_taggedAudience = Audience.Mixed;
				break;
			}
			ApplyTaggedAudience();
		}

		private void ApplyTaggedAudience()
		{
			try
			{
				if (MobileAds.settings.taggedAudience != _taggedAudience)
				{
					MobileAds.settings.taggedAudience = _taggedAudience;
					Debug.Log($"[Ads] Tagged audience set to {_taggedAudience}.");
				}
			}
			catch (Exception exception)
			{
				Debug.LogWarning("[Ads] Failed to set the tagged audience.");
				Debug.LogException(exception);
			}
		}

		private void BuildManager(string userId)
		{
			ApplyTaggedAudience();
			try
			{
				_manager = MobileAds.BuildManager().WithUserID(userId).WithCompletionListener((InitialConfiguration config) =>
				{
					if (string.IsNullOrEmpty(config.error))
					{
						Debug.Log("[Ads] CAS initialized. Country: " + config.countryCode + ", " + $"consent required: {config.isConsentRequired}, " + $"consent flow: {config.consentFlowStatus}");
					}
					else
					{
						Debug.LogWarning("[Ads] CAS initialization error: " + config.error);
					}
				})
					.Build();
			}
			catch (Exception exception)
			{
				Debug.LogError("[Ads] Failed to build the CAS mediation manager.");
				Debug.LogException(exception);
				_lastRewardedLoadFailure = AdFailureCause.None;
				_lastRewardedLoadMessage = null;
				return;
			}
			_managerUserId = userId;
			Debug.Log("[Ads] CAS manager built for player id \"" + userId + "\".");
			_lastRewardedLoadFailure = AdFailureCause.None;
			_lastRewardedLoadMessage = null;
			SendClientVersionToSsv();
			_manager.OnRewardedAdFailedToLoad += OnRewardedFailedToLoad;
			_manager.OnRewardedAdLoaded += OnRewardedLoaded;
		}

		private void SendClientVersionToSsv()
		{
			try
			{
				_manager.SetServerSideVerificationData("{\"AppVersion\":\"" + Application.version + "\"}");
			}
			catch (Exception exception)
			{
				Debug.LogWarning("[Ads] Failed to attach the client version to CAS server-side verification data.");
				Debug.LogException(exception);
			}
		}

		private void UnsubscribeFromManager()
		{
			if (_manager != null)
			{
				_manager.OnRewardedAdFailedToLoad -= OnRewardedFailedToLoad;
				_manager.OnRewardedAdLoaded -= OnRewardedLoaded;
			}
		}

		private void BuildManagerWithoutUserId()
		{
			Debug.LogWarning("[Ads] An ad was requested before any player id arrived. Building CAS without one — this session's callbacks are unattributed.");
			BuildManager(string.Empty);
		}

		public async UniTask<RewardedAdOutcome> ShowRewardedAsync(string placement, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_manager == null)
			{
				BuildManagerWithoutUserId();
			}
			if (_manager == null)
			{
				Debug.LogWarning("[Ads] Rewarded ad requested but the CAS manager could not be built.");
				return new RewardedAdOutcome(RewardedAdResult.Failed, AdFailureCause.NotInitialized);
			}
			if (_pendingRewarded != null)
			{
				Debug.LogWarning("[Ads] Rewarded ad already in progress, ignoring the new request.");
				return new RewardedAdOutcome(RewardedAdResult.Failed, AdFailureCause.AlreadyDisplayed);
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return new RewardedAdOutcome(RewardedAdResult.Failed, AdFailureCause.Unknown);
			}
			IMediationManager manager = _manager;
			UniTaskCompletionSource<RewardedAdOutcome> completion = new UniTaskCompletionSource<RewardedAdOutcome>();
			_pendingRewarded = completion;
			_rewardEarned = false;
			manager.OnRewardedAdCompleted += OnRewardedCompleted;
			manager.OnRewardedAdClosed += OnRewardedClosed;
			manager.OnRewardedAdFailedToShow += OnRewardedFailedToShow;
			manager.OnRewardedAdImpression += OnRewardedImpression;
			try
			{
				using (cancellationToken.Register(() =>
				{
					completion.TrySetResult(new RewardedAdOutcome(RewardedAdResult.Failed, AdFailureCause.Unknown));
				}))
				{
					try
					{
						manager.ShowAd(AdType.Rewarded, Sanitize(placement));
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						return new RewardedAdOutcome(RewardedAdResult.Failed, AdFailureCause.Unknown, ex.Message);
					}
					return await completion.Task;
				}
			}
			finally
			{
				manager.OnRewardedAdCompleted -= OnRewardedCompleted;
				manager.OnRewardedAdClosed -= OnRewardedClosed;
				manager.OnRewardedAdFailedToShow -= OnRewardedFailedToShow;
				manager.OnRewardedAdImpression -= OnRewardedImpression;
				_pendingRewarded = null;
			}
		}

		public void Dispose()
		{
			UnsubscribeFromManager();
			_pendingRewarded?.TrySetResult(new RewardedAdOutcome(RewardedAdResult.Failed, AdFailureCause.Unknown));
		}

		internal static AdFailureCause ToCause(int adErrorCode)
		{
			return adErrorCode switch
			{
				9 => AdFailureCause.NoConnection, 
				3 => AdFailureCause.NoFill, 
				1 => AdFailureCause.NotReady, 
				8 => AdFailureCause.Timeout, 
				6 => AdFailureCause.ReachedCap, 
				2 => AdFailureCause.Rejected, 
				10 => AdFailureCause.Configuration, 
				7 => AdFailureCause.NotInitialized, 
				15 => AdFailureCause.ManagerDisabled, 
				12 => AdFailureCause.AlreadyDisplayed, 
				13 => AdFailureCause.AppNotForeground, 
				_ => AdFailureCause.Unknown, 
			};
		}

		private static string Sanitize(string placement)
		{
			if (string.IsNullOrEmpty(placement))
			{
				return null;
			}
			if (placement.Length > 100)
			{
				return placement.Substring(0, 100);
			}
			return placement;
		}

		private void OnRewardedFailedToLoad(AdError error)
		{
			_lastRewardedLoadFailure = ToCause(error.GetCode());
			_lastRewardedLoadMessage = error.GetMessage();
			Debug.LogWarning($"[Ads] Rewarded ad failed to load: {_lastRewardedLoadFailure} " + "(" + _lastRewardedLoadMessage + ")");
		}

		private void OnRewardedLoaded()
		{
			_lastRewardedLoadFailure = AdFailureCause.None;
			_lastRewardedLoadMessage = null;
		}

		private void OnRewardedCompleted()
		{
			_rewardEarned = true;
		}

		private void OnRewardedImpression(AdMetaData meta)
		{
			if (meta == null)
			{
				return;
			}
			AdImpressionInfo obj;
			try
			{
				PriceAccuracy priceAccuracy = meta.priceAccuracy;
				bool revenueDisclosed = priceAccuracy != PriceAccuracy.Undisclosed;
				string creativeIdentifier = meta.creativeIdentifier;
				obj = new AdImpressionInfo(meta.network.ToString(), string.IsNullOrEmpty(creativeIdentifier) ? null : creativeIdentifier, meta.sourceUnitId, meta.revenue, meta.cpm, priceAccuracy.ToString(), revenueDisclosed);
			}
			catch (Exception exception)
			{
				Debug.LogWarning("[Ads] Could not read the impression metadata off the SDK.");
				Debug.LogException(exception);
				return;
			}
			try
			{
				RewardedImpression?.Invoke(obj);
			}
			catch (Exception exception2)
			{
				Debug.LogException(exception2);
			}
		}

		private void OnRewardedClosed()
		{
			_pendingRewarded?.TrySetResult(new RewardedAdOutcome((!_rewardEarned) ? RewardedAdResult.Skipped : RewardedAdResult.Completed));
		}

		private void OnRewardedFailedToShow(string error)
		{
			AdFailureCause adFailureCause = ((_lastRewardedLoadFailure == AdFailureCause.None) ? AdFailureCause.Unknown : _lastRewardedLoadFailure);
			Debug.LogWarning($"[Ads] Rewarded ad failed to show ({adFailureCause}): {error}");
			_pendingRewarded?.TrySetResult(new RewardedAdOutcome(RewardedAdResult.Failed, adFailureCause, error));
		}
	}
}
