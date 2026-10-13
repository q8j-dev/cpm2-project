using UnityEngine.Scripting;

namespace CPM.DailyTasks.Progress
{
	public enum TYPE
	{
		daily = 0,
		weekly = 1,
		monthly = 2
	}
	[Preserve]
	public class Type
	{
		public int[] finished;
	}
}
