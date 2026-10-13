namespace CodeBase.Infrastructure.States
{
	public interface IPayloadState<TPayload> : IExitableState
	{
		void Enter(TPayload payLoad);
	}
}
