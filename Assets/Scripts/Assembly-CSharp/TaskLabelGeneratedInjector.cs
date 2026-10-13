using System;
using System.Collections.Generic;
using CPM.DailyTasks;
using VContainer;

[Preserve]
internal class TaskLabelGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:TaskLabel cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		TaskLabel obj = (TaskLabel)instance;
		DailyTasksManager dailyTasksManager = (DailyTasksManager)resolver.ResolveOrParameter(typeof(DailyTasksManager), "dailyTasksManager", parameters);
		obj.Construct(dailyTasksManager);
	}
}
