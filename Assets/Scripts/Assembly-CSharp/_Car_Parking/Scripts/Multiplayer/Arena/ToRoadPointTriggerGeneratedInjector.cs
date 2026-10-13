using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ToRoadPointTriggerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ToRoadPointTrigger cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ToRoadPointTrigger obj = (ToRoadPointTrigger)instance;
			ArenaFlowController arenaFlow = (ArenaFlowController)resolver.ResolveOrParameter(typeof(ArenaFlowController), "arenaFlow", parameters);
			ArenaModeRegistry modeRegistry = (ArenaModeRegistry)resolver.ResolveOrParameter(typeof(ArenaModeRegistry), "modeRegistry", parameters);
			obj.Construct(arenaFlow, modeRegistry);
		}
	}
}
