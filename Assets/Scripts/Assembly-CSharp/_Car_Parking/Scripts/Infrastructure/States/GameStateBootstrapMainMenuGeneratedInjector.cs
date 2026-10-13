using System;
using System.Collections.Generic;
using CodeBase.Infrastructure.States;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure.States
{
	[Preserve]
	internal class GameStateBootstrapMainMenuGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GameStateBootstrapMainMenu cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GameStateBootstrapMainMenu obj = (GameStateBootstrapMainMenu)instance;
			IGameStateMachine gameStateMachine = (IGameStateMachine)resolver.ResolveOrParameter(typeof(IGameStateMachine), "gameStateMachine", parameters);
			IObjectResolver container = (IObjectResolver)resolver.ResolveOrParameter(typeof(IObjectResolver), "container", parameters);
			obj.Construct(gameStateMachine, container);
		}
	}
}
