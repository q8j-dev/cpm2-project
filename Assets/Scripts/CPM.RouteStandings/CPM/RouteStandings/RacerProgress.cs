using UnityEngine;

namespace CPM.RouteStandings
{
	public readonly struct RacerProgress
	{
		public readonly int Id;

		public readonly Vector3 Position;

		public RacerProgress(int id, Vector3 position)
		{
			Id = id;
			Position = position;
		}
	}
}
