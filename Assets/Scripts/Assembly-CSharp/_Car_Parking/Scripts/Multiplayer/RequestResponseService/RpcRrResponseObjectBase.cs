namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	public abstract class RpcRrResponseObjectBase
	{
		public enum CodeType
		{
			Fail = 0,
			Success = 1
		}

		public enum ErrorType
		{
			None = 0,
			Timeout = 1,
			PlayerLeft = 2,
			Unknown = 3
		}

		public CodeType Code { get; set; }

		public ErrorType Error { get; set; }

		public RpcRrResponseObjectBase(CodeType code)
		{
			Code = code;
		}
	}
}
