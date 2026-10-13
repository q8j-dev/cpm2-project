using System.Collections.Generic;
using VContainer;

namespace CodeBase.Infrastructure.States
{
	[Preserve]
	internal class GameLoopStateGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GameLoopState gameLoopState = new GameLoopState((IGameStateMachine)resolver.ResolveOrParameter(typeof(IGameStateMachine), "gameStateMachine", parameters));
			Inject(gameLoopState, resolver, parameters);
			return gameLoopState;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
