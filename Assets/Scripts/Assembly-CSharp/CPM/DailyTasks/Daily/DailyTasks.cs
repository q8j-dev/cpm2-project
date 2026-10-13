using System;
using UnityEngine.Scripting;

namespace CPM.DailyTasks.Daily
{
	[Serializable]
	[Preserve]
	public class DailyTasks
	{
		public int id;

		public int status;

		public DailyTask[] tasks;
	}
}
