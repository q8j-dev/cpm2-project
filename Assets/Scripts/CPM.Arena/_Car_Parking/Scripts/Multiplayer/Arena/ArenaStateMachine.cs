namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ArenaStateMachine
	{
		public struct Inputs
		{
			public int MinPlayers;

			public int MaxPlayers;

			public int PlayerCount;

			public double ServerTime;

			public double Deadline;

			public float MinPlayersWaitSeconds;
		}

		public static ArenaDecision Tick(ArenaPhase phase, Inputs input)
		{
			switch (phase)
			{
			case ArenaPhase.SearchPlayers:
				if (input.PlayerCount >= input.MaxPlayers)
				{
					return new ArenaDecision(ArenaPhase.Countdown, ArenaCommand.StartCountdown);
				}
				if (input.ServerTime >= input.Deadline)
				{
					if (input.PlayerCount >= input.MinPlayers)
					{
						return new ArenaDecision(ArenaPhase.Countdown, ArenaCommand.StartCountdown);
					}
					return new ArenaDecision(ArenaPhase.SearchPlayers, ArenaCommand.ExtendWait);
				}
				if (input.PlayerCount >= input.MinPlayers)
				{
					return new ArenaDecision(ArenaPhase.SearchPlayers, ArenaCommand.ClampWait);
				}
				return new ArenaDecision(ArenaPhase.SearchPlayers, ArenaCommand.None);
			case ArenaPhase.Countdown:
				if (input.ServerTime >= input.Deadline)
				{
					return new ArenaDecision(ArenaPhase.InProgress, ArenaCommand.BeginMode);
				}
				return new ArenaDecision(ArenaPhase.Countdown, ArenaCommand.None);
			default:
				return new ArenaDecision(phase, ArenaCommand.None);
			}
		}
	}
}
