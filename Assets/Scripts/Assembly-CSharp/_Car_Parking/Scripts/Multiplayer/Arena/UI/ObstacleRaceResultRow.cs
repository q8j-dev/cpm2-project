using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	public readonly struct ObstacleRaceResultRow : IResultRow
	{
		public string UserName { get; }

		public bool IsLocal { get; }

		public ObstacleRacePlayerStatus Status { get; }

		public double? EliminatedAt { get; }

		public ObstacleRaceResultRow(string userName, bool isLocal, ObstacleRacePlayerStatus status, double? eliminatedAt)
		{
			UserName = userName;
			IsLocal = isLocal;
			Status = status;
			EliminatedAt = eliminatedAt;
		}
	}
}
