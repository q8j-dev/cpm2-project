using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Impl;

namespace _Car_Parking.Scripts.Backend
{
	public static class BackendFactory
	{
		public static async UniTask<HttpRequest> CreateRequest(string endPoint, HttpVerb method, CancellationToken token = default(CancellationToken))
		{
			token.ThrowIfCancellationRequested();
			await FirebaseAuth.DefaultInstance.RefreshToken();
			token.ThrowIfCancellationRequested();
			HttpRequest httpRequest = HttpRequest.Request(endPoint, method);
			string valueOrDefault = Functions.FunctionHeaders.GetValueOrDefault("Authorization", string.Empty);
			httpRequest.Headers = new Dictionary<string, string> { ["X-Firebase-Token"] = valueOrDefault };
			return httpRequest;
		}

		public static async UniTask<bool> WaitForAuthTokenAsync(CancellationToken token = default(CancellationToken), int timeoutMs = 15000)
		{
			for (int waited = 0; waited < timeoutMs; waited += 200)
			{
				if (Functions.FunctionHeaders.GetValueOrDefault("Authorization", string.Empty).Length > "Bearer ".Length)
				{
					return true;
				}
				await UniTask.Delay(200, ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			}
			return false;
		}
	}
}
