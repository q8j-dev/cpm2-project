using System;
using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	internal class EventZoneCollectibleGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:EventZoneCollectible cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventZoneCollectible obj = (EventZoneCollectible)instance;
			MapCamera mapCamera = (MapCamera)resolver.ResolveOrParameter(typeof(MapCamera), "mapCamera", parameters);
			MapIconsModel mapIconsModel = (MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			obj.Construct(mapCamera, mapIconsModel, freeDriveProvider);
		}
	}
}
