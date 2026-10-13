using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;

namespace Car_Parking.Scripts.Racing
{
	[Preserve]
	internal class RaceStartTriggerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:RaceStartTrigger cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RaceStartTrigger obj = (RaceStartTrigger)instance;
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			FreeTimeAttackController timeAttackController = (FreeTimeAttackController)resolver.ResolveOrParameter(typeof(FreeTimeAttackController), "timeAttackController", parameters);
			MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.Construct(freeDriveProvider, prefabFactory, timeAttackController, menuLevelParent, managerUI);
		}
	}
}
