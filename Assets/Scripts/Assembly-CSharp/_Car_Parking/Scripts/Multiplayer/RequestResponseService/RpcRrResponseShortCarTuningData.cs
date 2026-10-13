using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	public class RpcRrResponseShortCarTuningData : RpcRrResponseObjectBase
	{
		public int ShortTuningData { get; private set; }

		public RpcRrResponseShortCarTuningData()
			: base(CodeType.Fail)
		{
		}

		public RpcRrResponseShortCarTuningData(CodeType code, int shortTuningData)
			: base(code)
		{
			ShortTuningData = shortTuningData;
		}
	}
}
