using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	public class RpcRrResponseBuyCar : RpcRrResponseObjectBase
	{
		public string TuningData;

		public RpcRrResponseBuyCar()
			: base(CodeType.Fail)
		{
		}

		public RpcRrResponseBuyCar(CodeType code, string tuningData)
			: base(code)
		{
			TuningData = tuningData;
		}
	}
}
