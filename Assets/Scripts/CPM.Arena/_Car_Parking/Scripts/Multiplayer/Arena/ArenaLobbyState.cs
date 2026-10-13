using ExitGames.Client.Photon;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaLobbyState
	{
		public const string PhaseKey = "aP";

		public const string DeadlineKey = "aD";

		public const string MatchStartKey = "aM";

		public ArenaPhase Phase;

		public double Deadline;

		public double MatchStartTime;

		public Hashtable ToRoomProperties()
		{
			return new Hashtable
			{
				{
					"aP",
					(int)Phase
				},
				{ "aD", Deadline },
				{ "aM", MatchStartTime }
			};
		}

		public static ArenaLobbyState FromRoomProperties(Hashtable props)
		{
			ArenaLobbyState arenaLobbyState = new ArenaLobbyState();
			if (props == null)
			{
				return arenaLobbyState;
			}
			if (props.ContainsKey("aP") && props["aP"] is int phase)
			{
				arenaLobbyState.Phase = (ArenaPhase)phase;
			}
			if (props.ContainsKey("aD") && props["aD"] is double deadline)
			{
				arenaLobbyState.Deadline = deadline;
			}
			if (props.ContainsKey("aM") && props["aM"] is double matchStartTime)
			{
				arenaLobbyState.MatchStartTime = matchStartTime;
			}
			return arenaLobbyState;
		}
	}
}
