using _Car_Parking.Scripts.Infrastructure.States;

namespace CodeBase.Infrastructure.States
{
	public class BootstrapStateFree : IState, IExitableState
	{
		private readonly IGameStateMachine _gameStateMachine;

		public BootstrapStateFree(IGameStateMachine gameStateMachine)
		{
			_gameStateMachine = gameStateMachine;
		}

		public void Enter()
		{
			_gameStateMachine.Enter<InitFreeDriveState>();
		}

		public void Exit()
		{
		}
	}
}
