using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using VContainer;

namespace _Car_Parking.Scripts.Connection
{
	public class TokenService : ITokenService
	{
		[UnityEngine.Scripting.Preserve]
		private class LoadTokenResponse
		{
			[JsonProperty("token")]
			public string Token = default;

			[UnityEngine.Scripting.Preserve]
			public LoadTokenResponse()
			{
			}
		}

		public const string GetCircuitRacingTokenFn = "progress-service/fb/v1/racing/circuit/token-client";

		private FirebaseWrappedFunctionFactory _factory;

		public string Token { get; set; }

		[Inject]
		public TokenService()
		{
			_factory = new FirebaseWrappedFunctionFactory();
		}

		public async UniTask LoadToken()
		{
			FunctionCallResult<LoadTokenResponse> functionCallResult = await _factory.Create("progress-service/fb/v1/racing/circuit/token-client").CallAsyncResult<LoadTokenResponse>();
			Token = ((functionCallResult.IsFail || functionCallResult.Data == null) ? "" : functionCallResult.Data.Token);
		}
	}
}
