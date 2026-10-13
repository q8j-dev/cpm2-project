using System.Collections.Generic;
using CodeBase.Infrastructure.States;
using VContainer;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.Infrastructure.States
{
	[Preserve]
	internal class InitMainMenuStateGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			InitMainMenuState initMainMenuState = new InitMainMenuState((IGameStateMachine)resolver.ResolveOrParameter(typeof(IGameStateMachine), "gameStateMachine", parameters), (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters), (CarCameras)resolver.ResolveOrParameter(typeof(CarCameras), "carCameras", parameters), (OptionalDependency<AnimationShop>)resolver.ResolveOrParameter(typeof(OptionalDependency<AnimationShop>), "animationShop", parameters));
			Inject(initMainMenuState, resolver, parameters);
			return initMainMenuState;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
