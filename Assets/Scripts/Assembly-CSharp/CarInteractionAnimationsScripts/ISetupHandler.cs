namespace CarInteractionAnimationsScripts
{
	public interface ISetupHandler
	{
		bool IsAnimate { get; }

		bool IsBusy { get; }

		void Enter();

		void Exit();
	}
}
