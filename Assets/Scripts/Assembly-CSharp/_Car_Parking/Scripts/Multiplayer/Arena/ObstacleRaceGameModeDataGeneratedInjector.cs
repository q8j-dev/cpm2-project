using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ObstacleRaceGameModeDataGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ObstacleRaceGameModeData cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ObstacleRaceGameModeData obj = (ObstacleRaceGameModeData)instance;
			ArenaModeRegistry arenaModeRegistry = (ArenaModeRegistry)resolver.ResolveOrParameter(typeof(ArenaModeRegistry), "arenaModeRegistry", parameters);
			obj.Construct(arenaModeRegistry);
		}
	}
}
