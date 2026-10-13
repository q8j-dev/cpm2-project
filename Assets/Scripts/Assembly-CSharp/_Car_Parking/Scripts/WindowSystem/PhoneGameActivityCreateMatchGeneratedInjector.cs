using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class PhoneGameActivityCreateMatchGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PhoneGameActivityCreateMatch cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PhoneGameActivityCreateMatch obj = (PhoneGameActivityCreateMatch)instance;
			ArenaFlowController arenaFlowController = (ArenaFlowController)resolver.ResolveOrParameter(typeof(ArenaFlowController), "arenaFlowController", parameters);
			ArenaModeRegistry arenaModeRegistry = (ArenaModeRegistry)resolver.ResolveOrParameter(typeof(ArenaModeRegistry), "arenaModeRegistry", parameters);
			obj.Construct(arenaFlowController, arenaModeRegistry);
		}
	}
}
