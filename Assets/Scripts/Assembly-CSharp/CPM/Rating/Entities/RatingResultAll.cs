using System;
using UnityEngine.Scripting;

namespace CPM.Rating.Entities
{
	[Serializable]
	[Preserve]
	public class RatingResultAll
	{
		public RatingData achievements;

		public RatingData general;

		public TaxiRatingResultAll taxi;

		public int race_win;

		public int? level;

		public float score;

		public int[] batches;
	}
}
