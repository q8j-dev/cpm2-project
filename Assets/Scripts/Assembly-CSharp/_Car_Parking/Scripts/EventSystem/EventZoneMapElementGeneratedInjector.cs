using System;
using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	internal class EventZoneMapElementGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:EventZoneMapElement cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventZoneMapElement obj = (EventZoneMapElement)instance;
			TimedActivityZoneController timedActivityZoneController = (TimedActivityZoneController)resolver.ResolveOrParameter(typeof(TimedActivityZoneController), "timedActivityZoneController", parameters);
			MapIconsModel mapIconsModel = (MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			obj.Construct(timedActivityZoneController, mapIconsModel, freeDriveProvider);
		}
	}
}
