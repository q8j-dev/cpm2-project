using System;
using Cysharp.Threading.Tasks;
using Photon.Realtime;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	public class RpcRrRequestCarDetails : RpcRrRequestObjectBase
	{
		public const float Timeout = 20f;

		private FreeDriveProvider _freeDriveProvider;

		public override CodeType Code => CodeType.CarDetails;

		public static string GetRequestData()
		{
			return "";
		}

		public RpcRrRequestCarDetails(FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
		}

		public override async UniTask<string> ExecuteRequestAsync(Player sender, string requestData)
		{
			return _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>().carInfo.GetStringTuningData();
		}

		public override RpcRrResponseObjectBase CreateResponse(string executedRequestData)
		{
			try
			{
				string carInstanceId;
				return new RpcRrResponseCarDetails(PartDataExtensions.GetTuningDataByString(executedRequestData, 0, out carInstanceId), RpcRrResponseObjectBase.CodeType.Success);
			}
			catch (Exception)
			{
				return new RpcRrResponseCarDetails(null, RpcRrResponseObjectBase.CodeType.Fail);
			}
		}
	}
}
