using _Car_Parking.Scripts.EventSystem.Data;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ObstacleRaceEventProgress
	{
		public readonly int Before;

		public readonly int After;

		public readonly int Max;

		public readonly EventStatus Status;

		public int Gained => After - Before;

		public ObstacleRaceEventProgress(int before, int after, int max, EventStatus status)
		{
			Before = before;
			After = after;
			Max = max;
			Status = status;
		}
	}
}
