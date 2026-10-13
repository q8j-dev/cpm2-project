using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ArenaIdContextConditionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ArenaIdContextCondition cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ArenaIdContextCondition obj = (ArenaIdContextCondition)instance;
			ArenaFlowController arena = (ArenaFlowController)resolver.ResolveOrParameter(typeof(ArenaFlowController), "arena", parameters);
			obj.Construct(arena);
		}
	}
}
