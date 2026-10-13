using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CPM.Cdn
{
	public sealed class CdnFetchService
	{
		private readonly ICdnClient _client;

		private readonly Func<TimeSpan, CancellationToken, UniTask> _delay;

		public CdnFetchService(ICdnClient client)
			: this(client, (TimeSpan interval, CancellationToken ct) => UniTask.Delay(interval, DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, ct))
		{
		}

		internal CdnFetchService(ICdnClient client, Func<TimeSpan, CancellationToken, UniTask> delay)
		{
			_client = client;
			_delay = delay;
		}

		public async UniTask<CdnFetchResult> FetchAsync(CdnFetchRequest request, CancellationToken ct)
		{
			byte[] cachedBytes = null;
			string cachedEtag = null;
			request.Cache?.TryRead(request.Url, out cachedBytes, out cachedEtag);
			int[] attempts = request.AttemptTimeoutsSeconds;
			if (attempts == null || attempts.Length == 0)
			{
				return CdnFetchResult.Failed("no attempts configured");
			}
			string lastError = "no attempts configured";
			for (int attempt = 0; attempt < attempts.Length; attempt++)
			{
				if (ct.IsCancellationRequested)
				{
					return CdnFetchResult.Failed("canceled");
				}
				CdnGetResult cdnGetResult;
				try
				{
					cdnGetResult = await _client.GetAsync(request.Url, cachedEtag, attempts[attempt], ct);
				}
				catch (OperationCanceledException)
				{
					return CdnFetchResult.Failed("canceled");
				}
				catch (Exception ex2)
				{
					cdnGetResult = CdnGetResult.Error(ex2.Message);
				}
				switch (cdnGetResult.Status)
				{
				case CdnGetStatus.NotModified:
					return CdnFetchResult.NotModified(cachedBytes);
				case CdnGetStatus.Ok:
					return AcceptAndPersist(request, cdnGetResult);
				}
				lastError = cdnGetResult.ErrorMessage;
				if (attempt < attempts.Length - 1)
				{
					try
					{
						await _delay(request.RetryDelay, ct);
					}
					catch (OperationCanceledException)
					{
						return CdnFetchResult.Failed("canceled");
					}
				}
			}
			return CdnFetchResult.Failed(lastError);
		}

		private static CdnFetchResult AcceptAndPersist(CdnFetchRequest request, CdnGetResult response)
		{
			try
			{
				if (request.AcceptPayload != null && !request.AcceptPayload(response.Bytes))
				{
					return CdnFetchResult.Failed("rejected");
				}
			}
			catch (Exception ex)
			{
				return CdnFetchResult.Failed("rejected: " + ex.Message);
			}
			if (request.Cache != null)
			{
				try
				{
					request.Cache.Write(request.Url, response.Bytes, response.Etag);
				}
				catch (Exception)
				{
				}
			}
			return CdnFetchResult.Updated(response.Bytes);
		}
	}
}
