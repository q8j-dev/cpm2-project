using System;

namespace CPM.Rating.Entities
{
	[Serializable]
	public class TaxiRatingResult
	{
		public uint coll_count;

		public uint count;

		public float distance;

		public uint money;

		public uint rating;

		public uint star;

		public float time;

		public override string ToString()
		{
			return $"coll count: {coll_count}" + $"count: {count}" + $"distance: {distance}" + $"money: {money}" + $"rating: {rating}" + $"star: {star}" + $"time: {time}";
		}
	}
}
