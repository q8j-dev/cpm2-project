using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Scopes;

namespace CPM.DailyTasks
{
	[Preserve]
	internal class DailyTaskWindowManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:DailyTaskWindowManager cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DailyTaskWindowManager obj = (DailyTaskWindowManager)instance;
			FreeDriveDB freeDriveDB = (FreeDriveDB)resolver.ResolveOrParameter(typeof(FreeDriveDB), "freeDriveDB", parameters);
			DailyTasksManager dailyTasksManager = (DailyTasksManager)resolver.ResolveOrParameter(typeof(DailyTasksManager), "dailyTasksManager", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
			OptionalDependency<MultiPlayerMenu> multiPlayerMenu = (OptionalDependency<MultiPlayerMenu>)resolver.ResolveOrParameter(typeof(OptionalDependency<MultiPlayerMenu>), "multiPlayerMenu", parameters);
			OptionalDependency<LevelScript> levelScript = (OptionalDependency<LevelScript>)resolver.ResolveOrParameter(typeof(OptionalDependency<LevelScript>), "levelScript", parameters);
			obj.Construct(freeDriveDB, dailyTasksManager, prefabFactory, managerUI, accountController, multiPlayerMenu, levelScript);
		}
	}
}
