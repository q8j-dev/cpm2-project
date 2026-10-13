using Cysharp.Threading.Tasks;
using Photon.Realtime;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	public class RpcRrRequestShortCarTuningData : RpcRrRequestObjectBase
	{
		private const string Error = "-1";

		private readonly FreeDriveProvider _freeDriveProvider;

		public override CodeType Code => CodeType.CarTuningDataInt;

		public static string GetRequestData()
		{
			return "";
		}

		public RpcRrRequestShortCarTuningData(FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
		}

		public override async UniTask<string> ExecuteRequestAsync(Player sender, string requestData)
		{
			if ((bool)_freeDriveProvider.FirstCarMultiPlayer)
			{
				return _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>().carInfo.TuningData.CalcShortTuningStates().ToString();
			}
			return "-1";
		}

		public override RpcRrResponseObjectBase CreateResponse(string executedRequestData)
		{
			if (executedRequestData == "-1")
			{
				return new RpcRrResponseShortCarTuningData(RpcRrResponseObjectBase.CodeType.Fail, 0);
			}
			return new RpcRrResponseShortCarTuningData(RpcRrResponseObjectBase.CodeType.Success, int.Parse(executedRequestData));
		}
	}
}
