using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace Car_Parking.Scripts.Racing
{
	[Preserve]
	internal class ZoneTriggerHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ZoneTriggerHandler cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ZoneTriggerHandler obj = (ZoneTriggerHandler)instance;
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			obj.Construct(freeDriveProvider);
		}
	}
}
