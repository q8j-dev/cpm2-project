using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ObstacleRaceStandingsUpdate
	{
		public readonly IReadOnlyList<ObstacleRaceStandingRow> Rows;

		public readonly int LocalPlace;

		public readonly float? LocalRouteProgress;

		public readonly float? LocalRemainingKm;

		public ObstacleRaceStandingsUpdate(IReadOnlyList<ObstacleRaceStandingRow> rows, int localPlace, float? localRouteProgress, float? localRemainingKm)
		{
			Rows = rows;
			LocalPlace = localPlace;
			LocalRouteProgress = localRouteProgress;
			LocalRemainingKm = localRemainingKm;
		}
	}
}
