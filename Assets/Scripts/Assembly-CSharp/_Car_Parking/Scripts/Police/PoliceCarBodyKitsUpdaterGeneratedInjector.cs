using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Police
{
	[Preserve]
	internal class PoliceCarBodyKitsUpdaterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PoliceCarBodyKitsUpdater cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PoliceCarBodyKitsUpdater obj = (PoliceCarBodyKitsUpdater)instance;
			PoliceSystemConfig policeConfig = (PoliceSystemConfig)resolver.ResolveOrParameter(typeof(PoliceSystemConfig), "policeConfig", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			obj.Construct(policeConfig, prefabFactory);
		}
	}
}
