namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ArenaPhaseFacts
	{
		public static bool IsGatheringPlayers(ArenaPhase phase)
		{
			if (phase != ArenaPhase.JoiningWaitingArea && phase != ArenaPhase.SearchPlayers)
			{
				return phase == ArenaPhase.Countdown;
			}
			return true;
		}

		public static bool IsPlaying(ArenaPhase phase)
		{
			return phase == ArenaPhase.InProgress;
		}

		public static bool IsActive(ArenaPhase phase)
		{
			if (phase != ArenaPhase.Idle)
			{
				return phase != ArenaPhase.Returning;
			}
			return false;
		}
	}
}
