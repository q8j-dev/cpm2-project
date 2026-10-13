using System;
using System.Collections.Generic;
using CPM.Fallout;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class FalloutRaceSessionClaimResultGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:FalloutRaceSessionClaimResult cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FalloutRaceSessionClaimResult obj = (FalloutRaceSessionClaimResult)instance;
			FalloutRaceSession raceSession = (FalloutRaceSession)resolver.ResolveOrParameter(typeof(FalloutRaceSession), "raceSession", parameters);
			obj.Construct(raceSession);
		}
	}
}
