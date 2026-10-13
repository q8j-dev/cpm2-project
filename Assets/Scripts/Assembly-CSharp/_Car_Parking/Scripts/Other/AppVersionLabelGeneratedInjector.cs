using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.Other
{
	[Preserve]
	internal class AppVersionLabelGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:AppVersionLabel cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AppVersionLabel obj = (AppVersionLabel)instance;
			IConnectionManager connectionManager = (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
			obj.Construct(connectionManager, managerUI, accountController);
		}
	}
}
