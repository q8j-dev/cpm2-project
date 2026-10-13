using Cysharp.Threading.Tasks;
using Photon.Realtime;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	public class RpcRrRequestCarTuningDataString : RpcRrRequestObjectBase
	{
		private const string Error = "-1";

		private FreeDriveProvider _freeDriveProvider;

		public override CodeType Code => CodeType.CarTuningDataString;

		public static string GetRequestData()
		{
			return "";
		}

		public RpcRrRequestCarTuningDataString(FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
		}

		public override async UniTask<string> ExecuteRequestAsync(Player sender, string requestData)
		{
			if ((bool)_freeDriveProvider.FirstCarMultiPlayer)
			{
				return _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>().carInfo.GetStringTuningData();
			}
			return "-1";
		}

		public override RpcRrResponseObjectBase CreateResponse(string executedRequestData)
		{
			if (executedRequestData == "-1")
			{
				return new RpcRrResponseCarTuningDataString(RpcRrResponseObjectBase.CodeType.Fail, "");
			}
			return new RpcRrResponseCarTuningDataString(RpcRrResponseObjectBase.CodeType.Success, executedRequestData);
		}
	}
}
