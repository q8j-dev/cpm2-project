using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Cdn;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CPM.Balance
{
	public sealed class BalanceDocumentSynchronizer : IBalanceDocumentSynchronizer
	{
		private readonly CdnFetchService _fetchService;

		private readonly EtagFileCache _cache;

		private readonly string _url;

		private readonly BalanceDocumentStore _store;

		private readonly List<UniTaskCompletionSource> _waiters = new List<UniTaskCompletionSource>();

		private bool _hasInFlight;

		private bool _succeededThisSession;

		public bool SucceededThisSession => _succeededThisSession;

		public BalanceDocumentSynchronizer(CdnFetchService fetchService, EtagFileCache cache, string url, BalanceDocumentStore store)
		{
			_fetchService = fetchService;
			_cache = cache;
			_url = url;
			_store = store;
		}

		public async UniTask EnsureFreshAsync(CancellationToken ct)
		{
			if (_succeededThisSession)
			{
				return;
			}
			UniTaskCompletionSource waiter = new UniTaskCompletionSource();
			_waiters.Add(waiter);
			if (!_hasInFlight)
			{
				_hasInFlight = true;
				RunAsync().Forget();
			}
			CancellationTokenRegistration registration = (ct.CanBeCanceled ? ct.Register(() =>
			{
				waiter.TrySetCanceled(ct);
			}) : default(CancellationTokenRegistration));
			try
			{
				await waiter.Task;
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				registration.Dispose();
			}
		}

		private async UniTaskVoid RunAsync()
		{
			try
			{
				CdnFetchRequest cdnFetchRequest = new CdnFetchRequest();
				cdnFetchRequest.Url = _url;
				cdnFetchRequest.Cache = _cache;
				cdnFetchRequest.AttemptTimeoutsSeconds = new int[3] { 5, 5, 5 };
				cdnFetchRequest.RetryDelay = TimeSpan.FromSeconds(2.0);
				cdnFetchRequest.AcceptPayload = TryApplyDownloadedDocument;
				cdnFetchRequest.Label = "Balance";
				CdnFetchRequest request = cdnFetchRequest;
				if ((await _fetchService.FetchAsync(request, CancellationToken.None)).Status != CdnFetchStatus.Failed)
				{
					_succeededThisSession = true;
				}
			}
			finally
			{
				_hasInFlight = false;
				ReleaseWaiters();
			}
		}

		private void ReleaseWaiters()
		{
			UniTaskCompletionSource[] array = _waiters.ToArray();
			_waiters.Clear();
			UniTaskCompletionSource[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].TrySetResult();
			}
		}

		private bool TryApplyDownloadedDocument(byte[] bytes)
		{
			BalanceDocument document = BalanceDocumentService.Parse(bytes);
			if (CarPriceProvider.TryInit(document))
			{
				_store.Set(document);
				return true;
			}
			Debug.LogError("BalanceDocumentSynchronizer: downloaded balance document is invalid (parse failed or 0 cars indexed) - keeping current prices, cache untouched.");
			return false;
		}
	}
}
