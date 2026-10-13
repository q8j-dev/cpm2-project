using System;
using System.Collections.Generic;
using CodeBase.Infrastructure.States;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure.States
{
	[Preserve]
	internal class GameStateBootstrapFreeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GameStateBootstrapFree cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GameStateBootstrapFree obj = (GameStateBootstrapFree)instance;
			IGameStateMachine gameStateMachine = (IGameStateMachine)resolver.ResolveOrParameter(typeof(IGameStateMachine), "gameStateMachine", parameters);
			IObjectResolver diContainer = (IObjectResolver)resolver.ResolveOrParameter(typeof(IObjectResolver), "diContainer", parameters);
			obj.Construct(gameStateMachine, diContainer);
		}
	}
}
