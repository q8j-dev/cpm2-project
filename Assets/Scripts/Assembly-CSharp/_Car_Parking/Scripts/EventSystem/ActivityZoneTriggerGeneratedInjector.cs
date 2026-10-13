using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	internal class ActivityZoneTriggerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ActivityZoneTrigger cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ActivityZoneTrigger obj = (ActivityZoneTrigger)instance;
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			CollectiblePoolContainer collectiblePoolContainer = (CollectiblePoolContainer)resolver.ResolveOrParameter(typeof(CollectiblePoolContainer), "collectiblePoolContainer", parameters);
			OptionalDependency<TimedActivityZoneController> timedActivityZoneController = (OptionalDependency<TimedActivityZoneController>)resolver.ResolveOrParameter(typeof(OptionalDependency<TimedActivityZoneController>), "timedActivityZoneController", parameters);
			OptionalDependency<MapCamera> mapCamera = (OptionalDependency<MapCamera>)resolver.ResolveOrParameter(typeof(OptionalDependency<MapCamera>), "mapCamera", parameters);
			obj.Construct(prefabFactory, freeDriveProvider, collectiblePoolContainer, timedActivityZoneController, mapCamera);
		}
	}
}
