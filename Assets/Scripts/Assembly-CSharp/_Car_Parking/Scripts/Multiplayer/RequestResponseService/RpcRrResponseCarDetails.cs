using UnityEngine.Scripting;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	public class RpcRrResponseCarDetails : RpcRrResponseObjectBase
	{
		public CarTuningData CarTuningData;

		public RpcRrResponseCarDetails()
			: base(CodeType.Fail)
		{
		}

		public RpcRrResponseCarDetails(CarTuningData carTuningData, CodeType code)
			: base(code)
		{
			CarTuningData = carTuningData;
		}
	}
}
