using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace CPM.Cdn
{
	public sealed class UnityWebRequestCdnClient : ICdnClient
	{
		public async UniTask<CdnGetResult> GetAsync(string url, string etagOrNull, int timeoutSeconds, CancellationToken ct)
		{
			using UnityWebRequest request = UnityWebRequest.Get(url);
			if (timeoutSeconds > 0)
			{
				request.timeout = timeoutSeconds;
			}
			if (!string.IsNullOrEmpty(etagOrNull))
			{
				request.SetRequestHeader("If-None-Match", etagOrNull);
			}
			request.SendWebRequest();
			while (!request.isDone)
			{
				if (ct.IsCancellationRequested)
				{
					request.Abort();
					return CdnGetResult.Error("canceled");
				}
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), DelayType.UnscaledDeltaTime);
			}
			return Map(request.responseCode, request.result, request.downloadHandler?.data, request.GetResponseHeader("ETag"), request.error);
		}

		internal static CdnGetResult Map(long responseCode, UnityWebRequest.Result result, byte[] data, string etagHeader, string error)
		{
			if (responseCode == 304)
			{
				return CdnGetResult.NotModified();
			}
			if (result == UnityWebRequest.Result.Success)
			{
				if (data == null || data.Length == 0)
				{
					return CdnGetResult.Error("empty 200 body");
				}
				return CdnGetResult.Ok(data, string.IsNullOrEmpty(etagHeader) ? null : etagHeader);
			}
			return CdnGetResult.Error(string.IsNullOrEmpty(error) ? $"HTTP {responseCode} ({result})" : $"{error} (HTTP {responseCode})");
		}
	}
}
