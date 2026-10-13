namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ArenaDecision
	{
		public readonly ArenaPhase NextPhase;

		public readonly ArenaCommand Command;

		public ArenaDecision(ArenaPhase nextPhase, ArenaCommand command)
		{
			NextPhase = nextPhase;
			Command = command;
		}
	}
}
