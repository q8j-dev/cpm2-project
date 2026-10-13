using CPM.DailyTasks.Progress;
using UnityEngine.Scripting;

namespace CPM.Rating.Entities
{
	[Preserve]
	public class SetRatingResponse
	{
		public RatingData data;

		public string error;

		public RatingData callback;

		public ResponseData battlepass;
	}
}
