using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ObstacleRaceZoneInfo
	{
		public readonly int Id;

		public readonly ObstacleRaceZoneDifficulty Difficulty;

		public readonly IReadOnlyList<ObstacleRaceMechanicType> Mechanics;

		public readonly ObstacleRaceZoneTrait Traits;

		public ObstacleRaceZoneInfo(int id, ObstacleRaceZoneDifficulty difficulty, IReadOnlyList<ObstacleRaceMechanicType> mechanics = null, ObstacleRaceZoneTrait traits = ObstacleRaceZoneTrait.None)
		{
			Id = id;
			Difficulty = difficulty;
			Mechanics = mechanics;
			Traits = traits;
		}

		public bool Is(ObstacleRaceZoneTrait trait)
		{
			return (Traits & trait) != 0;
		}
	}
}
