using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Common;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	[Preserve]
	internal class SecondLoadingInitGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:SecondLoadingInit cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SecondLoadingInit obj = (SecondLoadingInit)instance;
			SceneLoadingTimer sceneLoadingTimer = (SceneLoadingTimer)resolver.ResolveOrParameter(typeof(SceneLoadingTimer), "sceneLoadingTimer", parameters);
			IBootstrapHandler bootstrapHandler = (IBootstrapHandler)resolver.ResolveOrParameter(typeof(IBootstrapHandler), "bootstrapHandler", parameters);
			IGarageBuyService garageBuyService = (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters);
			IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
			PreMenuState preMenuState = (PreMenuState)resolver.ResolveOrParameter(typeof(PreMenuState), "preMenuState", parameters);
			AppStateHandler appStateHandler = (AppStateHandler)resolver.ResolveOrParameter(typeof(AppStateHandler), "appStateHandler", parameters);
			obj.Construct(sceneLoadingTimer, bootstrapHandler, garageBuyService, accountController, preMenuState, appStateHandler);
		}
	}
}
