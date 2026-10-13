using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class ObstacleRaceDetailViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ObstacleRaceDetailView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ObstacleRaceDetailView obj = (ObstacleRaceDetailView)instance;
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			ArenaModeRegistry arenaModeRegistry = (ArenaModeRegistry)resolver.ResolveOrParameter(typeof(ArenaModeRegistry), "arenaModeRegistry", parameters);
			obj.Construct(freeDriveProvider, arenaModeRegistry);
		}
	}
}
