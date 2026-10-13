using System.Collections.Generic;
using VContainer;

namespace CodeBase.Infrastructure.States
{
	[Preserve]
	internal class GameStateMachineGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GameStateMachine gameStateMachine = new GameStateMachine((IObjectResolver)resolver.ResolveOrParameter(typeof(IObjectResolver), "container", parameters));
			Inject(gameStateMachine, resolver, parameters);
			return gameStateMachine;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
