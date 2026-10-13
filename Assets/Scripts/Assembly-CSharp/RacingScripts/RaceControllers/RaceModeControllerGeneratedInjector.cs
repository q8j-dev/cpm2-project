using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Racing_Scripts;

namespace RacingScripts.RaceControllers
{
	[Preserve]
	internal class RaceModeControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:RaceModeController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RaceModeController obj = (RaceModeController)instance;
			RaceModeDataSO raceModeDataSO = (RaceModeDataSO)resolver.ResolveOrParameter(typeof(RaceModeDataSO), "raceModeDataSO", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			obj.Construct(raceModeDataSO, prefabFactory);
		}
	}
}
