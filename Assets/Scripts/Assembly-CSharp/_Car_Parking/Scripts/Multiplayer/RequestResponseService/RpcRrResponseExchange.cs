using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	public class RpcRrResponseExchange : RpcRrResponseObjectBase
	{
		public string TuningData;

		public RpcRrResponseExchange()
			: base(CodeType.Fail)
		{
		}

		public RpcRrResponseExchange(CodeType code, string tuningData)
			: base(code)
		{
			TuningData = tuningData;
		}
	}
}
