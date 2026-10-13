using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	public class RpcRrResponseCarTuningDataString : RpcRrResponseObjectBase
	{
		public string TuningData;

		public RpcRrResponseCarTuningDataString()
			: base(CodeType.Fail)
		{
		}

		public RpcRrResponseCarTuningDataString(CodeType code, string tuningData)
			: base(code)
		{
			TuningData = tuningData;
		}
	}
}
