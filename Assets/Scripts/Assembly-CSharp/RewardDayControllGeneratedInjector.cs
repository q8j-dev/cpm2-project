using System;
using System.Collections.Generic;
using CPM.DailyTasks;
using VContainer;

[Preserve]
internal class RewardDayControllGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:RewardDayControll cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RewardDayControll obj = (RewardDayControll)instance;
		MultiPlayerMenu multiPlayerMenu = (MultiPlayerMenu)resolver.ResolveOrParameter(typeof(MultiPlayerMenu), "multiPlayerMenu", parameters);
		DailyTasksManager dailyTasksManager = (DailyTasksManager)resolver.ResolveOrParameter(typeof(DailyTasksManager), "dailyTasksManager", parameters);
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		obj.Construct(multiPlayerMenu, dailyTasksManager, currencyManagerController);
	}
}
