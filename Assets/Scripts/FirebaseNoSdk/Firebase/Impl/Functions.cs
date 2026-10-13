using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Errors;
using Firebase.Functions;
using Newtonsoft.Json;
using Unions;
using UnityEngine;
using UnityEngine.Scripting;

namespace Firebase.Impl
{
	public class Functions : FirebaseFunctions
	{
		private class Function : HttpsCallableReference
		{
			[Preserve]
			private class ServerFunctionResponse<TRes>
			{
				[JsonProperty("result")]
				public ResultData<TRes> Result = default;
			}

			private readonly string _url;

			private readonly int? _timeout;

			public Function(string endpoint, int? timeout)
			{
				_timeout = timeout;
				_url = _region switch
				{
					Region.USCentral1 => "https://us-central1-cpm-2-7cea1.cloudfunctions.net/" + endpoint, 
					Region.EuropeWest1 => "https://europe-west1-cpm-2-7cea1.cloudfunctions.net/" + endpoint, 
					_ => "https://us-central1-cpm-2-7cea1.cloudfunctions.net/" + endpoint, 
				};
			}

			public override UniTask<TRes> CallAsync<TRes>(object data = null)
			{
				return Call<TRes>(data);
			}

			public override UniTask<string> CallAsyncRaw(object data = null)
			{
				return CallRaw(data);
			}

			public override UniTask<FunctionCallResult<TRes>> CallAsyncResult<TRes>(object data = null, int networkAttempts = 1, Dictionary<string, string> extraHeaders = null)
			{
				return CallResult<TRes>(data, 0, networkAttempts, extraHeaders);
			}

			private async UniTask<TRes> Call<TRes>(object data, int attempts = 0)
			{
				return await (await PostAsync(data)).Match((Func<string, UniTask<TRes>>)(async (string r) =>
				{
					GoogleErrorHolder googleErrorHolder = JsonConvert.DeserializeObject<GoogleErrorHolder>(r);
					if (googleErrorHolder?.error != null)
					{
						if (googleErrorHolder.error.message == "TOKEN_EXPIRED" && attempts < 3)
						{
							await FirebaseAuth.DefaultInstance.RefreshToken(force: true);
							return await Call<TRes>(data, attempts + 1);
						}
						return default;
					}
					return FunctionsHelper.UseOneStep<TRes>() ? JsonConvert.DeserializeObject<CallAsyncResOneStep<TRes>>(r).result : JsonConvert.DeserializeObject<CallAsyncResTwoSteps<TRes>>(r).Result;
				}), (Func<NoInternetConnection, UniTask<TRes>>)ErrorUtils.AsyncHandler<NoInternetConnection, TRes>, (Func<HttpError, UniTask<TRes>>)ErrorUtils.AsyncHandler<HttpError, TRes>);
			}

			private async UniTask<string> CallRaw(object data, int attempts = 0)
			{
				return await (await PostAsync(data)).Match((Func<string, UniTask<string>>)(async (string r) =>
				{
					GoogleErrorHolder googleErrorHolder = JsonConvert.DeserializeObject<GoogleErrorHolder>(r);
					if (googleErrorHolder?.error != null)
					{
						if (googleErrorHolder.error.message == "TOKEN_EXPIRED" && attempts < 3)
						{
							await FirebaseAuth.DefaultInstance.RefreshToken(force: true);
							return await CallRaw(data, attempts + 1);
						}
						return (string)null;
					}
					return JsonConvert.DeserializeObject<CallAsyncResRaw>(r).result;
				}), (Func<NoInternetConnection, UniTask<string>>)ErrorUtils.AsyncHandler<NoInternetConnection, string>, (Func<HttpError, UniTask<string>>)ErrorUtils.AsyncHandler<HttpError, string>);
			}

			private async UniTask<FunctionCallResult<TRes>> CallResult<TRes>(object data, int tokenAttempts = 0, int networkAttempts = 1, Dictionary<string, string> extraHeaders = null)
			{
				return await (await PostAsync(data, extraHeaders)).Match((Func<string, UniTask<FunctionCallResult<TRes>>>)(async (string r) =>
				{
					GoogleErrorHolder googleErrorHolder = JsonConvert.DeserializeObject<GoogleErrorHolder>(r);
					if (googleErrorHolder?.error != null)
					{
						if (googleErrorHolder.error.message == "TOKEN_EXPIRED" && tokenAttempts < 3)
						{
							await FirebaseAuth.DefaultInstance.RefreshToken(force: true);
							return await CallResult<TRes>(data, tokenAttempts + 1, networkAttempts, extraHeaders);
						}
						return FunctionCallResult<TRes>.Fail(FunctionStatus.TokenExpired, googleErrorHolder.error.message);
					}
					ResultData<TRes> result;
					try
					{
						result = JsonConvert.DeserializeObject<ServerFunctionResponse<TRes>>(r).Result;
						if (result == null)
						{
							return FunctionCallResult<TRes>.Fail(FunctionStatus.ResultDataIsNull, "Result is null");
						}
					}
					catch (Exception ex)
					{
						return FunctionCallResult<TRes>.Fail(FunctionStatus.ParseErrorServerFunctionResponse, ex.Message);
					}
					if (result.StatusCode != 1)
					{
						string errorMessage = result.ErrorData?.Message;
						_ = result.StatusCode;
						_ = 184;
						return FunctionCallResult<TRes>.Fail((FunctionStatus)result.StatusCode, errorMessage);
					}
					return FunctionCallResult<TRes>.Ok(result.Data);
				}), (Func<NoInternetConnection, UniTask<FunctionCallResult<TRes>>>)(async (NoInternetConnection _) =>
				{
					if (networkAttempts > 1)
					{
						await UniTask.Delay(TimeSpan.FromSeconds(2.0));
						return await CallResult<TRes>(data, tokenAttempts, networkAttempts - 1, extraHeaders);
					}
					return FunctionCallResult<TRes>.Fail(FunctionStatus.NoInternet);
				}), (Func<HttpError, UniTask<FunctionCallResult<TRes>>>)(async (HttpError httpError) =>
				{
					if (httpError.IsAuthFailure && tokenAttempts < 3)
					{
						await FirebaseAuth.DefaultInstance.RefreshToken(force: true);
						return await CallResult<TRes>(data, tokenAttempts + 1, networkAttempts, extraHeaders);
					}
					if (networkAttempts > 1)
					{
						await UniTask.Delay(TimeSpan.FromSeconds(2.0));
						return await CallResult<TRes>(data, tokenAttempts, networkAttempts - 1, extraHeaders);
					}
					return FunctionCallResult<TRes>.Fail(FunctionStatus.HttpError);
				}));
			}

			private async UniTask<Union<string, NoInternetConnection, HttpError>> PostAsync(object data, Dictionary<string, string> extraHeaders = null)
			{
				await FirebaseAuth.DefaultInstance.RefreshToken();
				Dictionary<string, string> dictionary = FunctionHeaders;
				if (extraHeaders != null)
				{
					dictionary = new Dictionary<string, string>(FunctionHeaders);
					foreach (KeyValuePair<string, string> extraHeader in extraHeaders)
					{
						dictionary[extraHeader.Key] = extraHeader.Value;
					}
				}
				return await Utils.PostRawAsync(_url, new CallAsyncArg(data), _timeout, dictionary);
			}
		}

		private class BackendFunction : HttpsCallableReference
		{
			private readonly string _url;

			private readonly int? _timeout;

			public BackendFunction(string endpoint, int? timeout)
			{
				_timeout = timeout;
				_url = "https://cpm-2.ogames.kz/api/" + endpoint;
			}

			public override async UniTask<TRes> CallAsync<TRes>(object data = null)
			{
				FunctionCallResult<TRes> functionCallResult = await CallResult<TRes>(data, 1);
				return functionCallResult.IsSuccess ? functionCallResult.Data : default(TRes);
			}

			public override async UniTask<string> CallAsyncRaw(object data = null)
			{
				return await (await PostAsync(data)).Match((string r) => UniTask.FromResult(r), ErrorUtils.AsyncHandler<NoInternetConnection, string>, ErrorUtils.AsyncHandler<HttpError, string>);
			}

			public override UniTask<FunctionCallResult<TRes>> CallAsyncResult<TRes>(object data = null, int networkAttempts = 1, Dictionary<string, string> extraHeaders = null)
			{
				return CallResult<TRes>(data, networkAttempts);
			}

			private async UniTask<FunctionCallResult<TRes>> CallResult<TRes>(object data, int networkAttempts, int tokenAttempts = 0)
			{
				return await (await PostAsync(data)).Match((Func<string, UniTask<FunctionCallResult<TRes>>>)((string r) => UniTask.FromResult(Parse<TRes>(r))), (Func<NoInternetConnection, UniTask<FunctionCallResult<TRes>>>)(async (NoInternetConnection _) =>
				{
					if (networkAttempts > 1)
					{
						await UniTask.Delay(TimeSpan.FromSeconds(2.0));
						return await CallResult<TRes>(data, networkAttempts - 1, tokenAttempts);
					}
					return FunctionCallResult<TRes>.Fail(FunctionStatus.NoInternet);
				}), (Func<HttpError, UniTask<FunctionCallResult<TRes>>>)(async (HttpError httpError) =>
				{
					if (httpError.IsAuthFailure && tokenAttempts < 3)
					{
						await FirebaseAuth.DefaultInstance.RefreshToken(force: true);
						return await CallResult<TRes>(data, networkAttempts, tokenAttempts + 1);
					}
					if (networkAttempts > 1)
					{
						await UniTask.Delay(TimeSpan.FromSeconds(2.0));
						return await CallResult<TRes>(data, networkAttempts - 1, tokenAttempts);
					}
					return FunctionCallResult<TRes>.Fail(FunctionStatus.HttpError);
				}));
			}

			private FunctionCallResult<TRes> Parse<TRes>(string raw)
			{
				ResultData<TRes> resultData;
				try
				{
					resultData = JsonConvert.DeserializeObject<ResultData<TRes>>(raw);
					if (resultData == null)
					{
						return FunctionCallResult<TRes>.Fail(FunctionStatus.ResultDataIsNull, "Result is null");
					}
				}
				catch (Exception ex)
				{
					return FunctionCallResult<TRes>.Fail(FunctionStatus.ParseErrorServerFunctionResponse, ex.Message);
				}
				if (resultData.StatusCode != 1)
				{
					string errorMessage = resultData.ErrorData?.Message;
					return FunctionCallResult<TRes>.Fail((FunctionStatus)resultData.StatusCode, errorMessage);
				}
				return FunctionCallResult<TRes>.Ok(resultData.Data);
			}

			private async UniTask<Union<string, NoInternetConnection, HttpError>> PostAsync(object data)
			{
				await FirebaseAuth.DefaultInstance.RefreshToken();
				Dictionary<string, string> headers = new Dictionary<string, string>
				{
					["Content-Type"] = "application/json; charset=utf-8",
					["Accept-Encoding"] = "gzip",
					["X-Firebase-Token"] = FunctionHeaders.GetValueOrDefault("Authorization", string.Empty),
					["X-Client-Platform"] = GetPlatform(),
					["X-Client-Version"] = Application.version,
					["X-Client-DeviceId"] = SystemInfo.deviceUniqueIdentifier
				};
				return await Utils.PostRawAsync(_url, new CallAsyncArg(data), _timeout, headers);
			}

			private static string GetPlatform()
			{
				return "UNKNOWN";
			}
		}

		[Preserve]
		private class ResultData<TRes>
		{
			[JsonProperty("code")]
			public int StatusCode = default;

			[JsonProperty("data")]
			public TRes Data = default;

			[JsonProperty("errorData")]
			public ErrorDataHolder ErrorData = default;
		}

		[Preserve]
		private class ErrorDataHolder
		{
			[JsonProperty("message")]
			public string Message = default;
		}

		private const string TokenExpired = "TOKEN_EXPIRED";

		private const float DelayBeforeNextNetworkAttempt = 2f;

		public static readonly Dictionary<string, string> FunctionHeaders = new Dictionary<string, string>
		{
			["Content-Type"] = "application/json; charset=utf-8",
			["Accept-Encoding"] = "gzip"
		};

		public const string BackendBaseUrl = "https://cpm-2.ogames.kz/api/";

		private static Region _region;

		public static void SetRegion(int region)
		{
			_region = (Region)region;
		}

		public override HttpsCallableReference GetHttpsCallable(string endpoint, int? timeout = null)
		{
			if (endpoint == null || endpoint.IndexOf('/') < 0)
			{
				return new Function(endpoint, timeout);
			}
			return new BackendFunction(endpoint, timeout);
		}
	}
}
