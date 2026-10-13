using UnityEngine.Scripting;

namespace Rating.DailyTasks.Scripts
{
	[Preserve]
	public interface IDailyServicesDependant
	{
		void Initialize(double timestamp);
	}
}
