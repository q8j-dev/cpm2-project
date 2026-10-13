using System;
using CPM.Rating;

namespace CPM.DailyTasks.Daily
{
	[Serializable]
	public class DailyTask
	{
		public TYPE type;

		public float current;

		public float goal;

		public int status;

		public Prize prize;

		public string name;

		public string description;

		public string secret;
	}
}
