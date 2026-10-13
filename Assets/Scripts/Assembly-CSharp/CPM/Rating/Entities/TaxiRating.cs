using System;

namespace CPM.Rating.Entities
{
	[Serializable]
	public class TaxiRating
	{
		public readonly float time;

		public readonly float distance;

		public readonly uint collisions;

		public readonly uint money;

		public readonly uint stars;

		public readonly uint count;
	}
}
