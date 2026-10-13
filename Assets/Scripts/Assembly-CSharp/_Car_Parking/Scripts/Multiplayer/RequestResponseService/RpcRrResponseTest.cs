namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	public class RpcRrResponseTest : RpcRrResponseObjectBase
	{
		public int DataInt;

		public RpcRrResponseTest()
			: base(CodeType.Fail)
		{
		}

		public RpcRrResponseTest(int dataInt, CodeType code)
			: base(code)
		{
			DataInt = dataInt;
		}
	}
}
