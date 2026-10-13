using UnityEngine;

namespace CodeBase.Infrastructure.States
{
	public class PauseGameState : IState, IExitableState
	{
		public void Enter()
		{
			SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OffReverState);
			Time.timeScale = 0f;
		}

		public void Exit()
		{
			SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OnReverState);
			Time.timeScale = 1f;
		}
	}
}
