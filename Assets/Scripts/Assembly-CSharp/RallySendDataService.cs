using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Racing_Scripts;

public class RallySendDataService
{
	public const string GetAllRallyTimesFn = "progress-service/fb/v1/racing/rally/times-client";

	public const string SetRallyTimeFn = "progress-service/fb/v1/racing/rally/update-client";

	public const string SetNewGamePlusFn = "progress-service/fb/v1/racing/rally/new-game-client";

	public async UniTask<RallyData> GetRallyProgress()
	{
		return (await new FirebaseWrappedFunctionFactory().Create("progress-service/fb/v1/racing/rally/times-client").CallAsyncResult<RallyData>()).Data;
	}

	public async UniTask<RallyProgressResult> SetRaceResutsAsync(RallyProgressData rallyProgressData)
	{
		FirebaseWrappedFunctionFactory firebaseWrappedFunctionFactory = new FirebaseWrappedFunctionFactory();
		string data = JsonConvert.SerializeObject(rallyProgressData);
		FirebaseWrappedFunction firebaseWrappedFunction = firebaseWrappedFunctionFactory.Create("progress-service/fb/v1/racing/rally/update-client");
		IDecoder decoder = new AesDecoder();
		return (await firebaseWrappedFunction.CallAsyncResult<RallyProgressResult>(decoder, data)).Data;
	}

	public async UniTask<RallyData> SetNewGamePlusResutsAsync()
	{
		FunctionCallResult<RallyData> functionCallResult = await new FirebaseWrappedFunctionFactory().Create("progress-service/fb/v1/racing/rally/new-game-client").CallAsyncResult<RallyData>();
		if (functionCallResult.Status == FunctionStatus.Success && functionCallResult.Data != null)
		{
			return functionCallResult.Data;
		}
		return null;
	}
}
