using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace Service.Cargo
{
	[Preserve]
	internal class ServiceControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ServiceController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ServiceController obj = (ServiceController)instance;
			NamesScriptables namesScriptables = (NamesScriptables)resolver.ResolveOrParameter(typeof(NamesScriptables), "namesScriptables", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			obj.Construct(namesScriptables, freeDriveProvider);
		}
	}
}
