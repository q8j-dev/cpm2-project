using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class ManagerUIGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ManagerUI cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ManagerUI obj = (ManagerUI)instance;
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			ScreenFader screenFader = (ScreenFader)resolver.ResolveOrParameter(typeof(ScreenFader), "screenFader", parameters);
			CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
			ICarTuningFactory carTuningFactory = (ICarTuningFactory)resolver.ResolveOrParameter(typeof(ICarTuningFactory), "carTuningFactory", parameters);
			IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
			LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
			IConnectionDataHandler connectionDataHandler = (IConnectionDataHandler)resolver.ResolveOrParameter(typeof(IConnectionDataHandler), "connectionDataHandler", parameters);
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
			obj.Construct(prefabFactory, freeDriveProvider, screenFader, currencyManagerController, carTuningFactory, accountController, levelsProvider, connectionDataHandler, eventRegistry, inputMapSetEvent);
		}
	}
}
