using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	internal class ZoneParticleMoverGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ZoneParticleMover cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ZoneParticleMover obj = (ZoneParticleMover)instance;
			OptionalDependency<TimedActivityZoneController> timedActivityZoneController = (OptionalDependency<TimedActivityZoneController>)resolver.ResolveOrParameter(typeof(OptionalDependency<TimedActivityZoneController>), "timedActivityZoneController", parameters);
			obj.Construct(timedActivityZoneController);
		}
	}
}
