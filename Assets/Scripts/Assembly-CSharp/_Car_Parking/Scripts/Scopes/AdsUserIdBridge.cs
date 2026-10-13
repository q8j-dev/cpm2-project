using System;
using System.Threading;
using CPM.Account;
using CPM.Ads;
using CPM.Ads.Policy;
using CPM.Balance;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

namespace _Car_Parking.Scripts.Scopes
{
	public class AdsUserIdBridge : IAuthorizationDependant
	{
		private readonly IAdsUserIdentity _adsIdentity;

		private readonly IAdsService _ads;

		private readonly IAdPlacementCatalog _catalog;

		private readonly IBalanceDocumentStore _documents;

		private readonly IBalanceDocumentSynchronizer _documentSync;

		private readonly IRemoteConfigManager _remoteConfig;

		private string _pendingUserId;

		private bool _waitingForDocument;

		public AdsUserIdBridge(IAdsUserIdentity adsIdentity, IAdsService ads, IAdPlacementCatalog catalog, IBalanceDocumentStore documents, IBalanceDocumentSynchronizer documentSync, IRemoteConfigManager remoteConfig)
		{
			_adsIdentity = adsIdentity;
			_ads = ads;
			_catalog = catalog;
			_documents = documents;
			_documentSync = documentSync;
			_remoteConfig = remoteConfig;
		}

		public UniTaskVoid OnSuccessfulAuthorization()
		{
			PushCurrentUserId();
			return default;
		}

		public void OnSignOut()
		{
		}

		private async UniTaskVoid PushUserIdAsync(string userId)
		{
			await _documentSync.EnsureFreshAsync(CancellationToken.None);
			if (!_documentSync.SucceededThisSession)
			{
				WaitForDocument(userId);
				return;
			}
			await ApplyAudienceAsync();
			PushUserId(userId);
		}

		private async UniTask ApplyAudienceAsync()
		{
			try
			{
				ConfigValuesStorage configValuesStorage = await _remoteConfig.GetConfigValuesStorage();
				if (configValuesStorage != null)
				{
					_ads.SetTaggedAudience(configValuesStorage.adsAudienceTarget);
				}
			}
			catch (Exception)
			{
			}
		}

		private void PushUserId(string userId)
		{
			if (_catalog.SystemEnabled)
			{
				_adsIdentity.SetUserId(userId);
			}
			else
			{
				WaitForDocument(userId);
			}
		}

		private void WaitForDocument(string userId)
		{
			_pendingUserId = userId;
			if (!_waitingForDocument)
			{
				_waitingForDocument = true;
				_documents.Changed += OnDocumentChanged;
			}
		}

		private void OnDocumentChanged()
		{
			if (_documentSync.SucceededThisSession && _catalog.SystemEnabled && _pendingUserId != null)
			{
				_documents.Changed -= OnDocumentChanged;
				_waitingForDocument = false;
				string pendingUserId = _pendingUserId;
				_pendingUserId = null;
				_adsIdentity.SetUserId(pendingUserId);
			}
		}

		private void PushCurrentUserId()
		{
			try
			{
				FirebaseUser currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
				if (currentUser != null && !string.IsNullOrEmpty(currentUser.UserId))
				{
					PushUserIdAsync(currentUser.UserId).Forget();
				}
			}
			catch (Exception exception)
			{
				Debug.LogWarning("[Ads] Failed to read the Firebase user id for CAS.");
				Debug.LogException(exception);
			}
		}
	}
}
