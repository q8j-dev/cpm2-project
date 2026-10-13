using System;
using System.Collections.Generic;
using CPM.Account;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.DailyRewards
{
	[Preserve]
	internal class ButtonOpenDailyRewardsWindowGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ButtonOpenDailyRewardsWindow cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ButtonOpenDailyRewardsWindow obj = (ButtonOpenDailyRewardsWindow)instance;
			IDailyRewardsServer dailyRewardsServer = (IDailyRewardsServer)resolver.ResolveOrParameter(typeof(IDailyRewardsServer), "dailyRewardsServer", parameters);
			IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.Construct(dailyRewardsServer, accountController, managerUI);
		}
	}
}
