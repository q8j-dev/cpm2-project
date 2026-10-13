using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.CPMRating.DailyTasks.Scripts
{
	[Preserve]
	internal class DailyTasksInfoContainerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DailyTasksInfoContainer dailyTasksInfoContainer = new DailyTasksInfoContainer();
			Inject(dailyTasksInfoContainer, resolver, parameters);
			return dailyTasksInfoContainer;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
