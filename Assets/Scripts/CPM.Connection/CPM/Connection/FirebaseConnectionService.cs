using System;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using VContainer;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.Connection;

namespace CPM.Connection
{
	public class FirebaseConnectionService : IConnectionService
	{
		public enum CheckHashSourceAnalyticType
		{
			firebase = 0,
			ogames = 1
		}

		public enum CheckHashResultAnalyticType
		{
			failed = 0,
			success = 1,
			timeout = 2
		}

		public class UserConnectionDataRequest
		{
			public string deviceId;

			public bool isTestingBuild;

			public UserConnectionDataRequest(string deviceId, bool isTestingBuild)
			{
				this.deviceId = deviceId;
				this.isTestingBuild = isTestingBuild;
			}
		}

		public const string CheckHashFn = "Check24_1";

		private const int LimitErrorLength = 500;

		private const int CountRetry = 2;

		private const string ValidateTrue = "true";

		private const int TimeoutInSeconds = 30;

		private readonly FirebaseWrappedFunctionFactory factory;

		private readonly CheckBackendService _checkBackendService;

		public event Action<string, int, string, bool, string> OnCheckedHash;

		public event Action<CheckHashResultForAnalytics> OnCheckHashResult;

		[Inject]
		public FirebaseConnectionService(CheckBackendService checkBackendService)
		{
			factory = new FirebaseWrappedFunctionFactory();
			_checkBackendService = checkBackendService;
		}

		public async UniTask<bool> CheckHash(string hash)
		{
			int amountRetry = 0;
			CheckHashResultAnalyticType result = CheckHashResultAnalyticType.failed;
			bool isValidated = false;
			string error = "";
			for (int i = 0; i < 2; i++)
			{
				amountRetry++;
				try
				{
					Result<bool, string> result2 = await _checkBackendService.CheckHashAsync(30, hash);
					if (result2.IsFail)
					{
						throw new Exception(result2.Error);
					}
					if (result2.IsSuccess)
					{
						CheckHashSourceAnalyticType checkHashSourceAnalyticType = CheckHashSourceAnalyticType.ogames;
						result = CheckHashResultAnalyticType.success;
						isValidated = result2.Value;
						error = "";
						OnCheckHashResult?.Invoke(new CheckHashResultForAnalytics(checkHashSourceAnalyticType.ToString(), amountRetry, result.ToString(), isValidated, error));
						return result2.Value;
					}
				}
				catch (Exception ex)
				{
					result = (ex.Message.Contains("Request timeout") ? CheckHashResultAnalyticType.timeout : result);
					error += $"{amountRetry} attempt : {ex.Message}";
					error = ((error.Length <= 500) ? error : error.Substring(0, 500));
					error += "\n";
				}
			}
			OnCheckHashResult?.Invoke(new CheckHashResultForAnalytics(CheckHashSourceAnalyticType.firebase.ToString(), amountRetry, result.ToString(), isValidated, error));
			FunctionCallResult<bool> functionCallResult = await factory.Create("Check24_1").CallAsyncResult<bool>(hash);
			return functionCallResult.IsSuccess && functionCallResult.Data;
		}
	}
}
