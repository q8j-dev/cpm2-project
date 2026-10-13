using System.Collections.Generic;
using CodeBase.Infrastructure.States;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure.States
{
	[Preserve]
	internal class InitFreeDriveStateGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			InitFreeDriveState initFreeDriveState = new InitFreeDriveState((IGameStateMachine)resolver.ResolveOrParameter(typeof(IGameStateMachine), "gameStateMachine", parameters), (FreeDriveControl)resolver.ResolveOrParameter(typeof(FreeDriveControl), "freeDriveControl", parameters), (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters), (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters), (QuickCarSettings)resolver.ResolveOrParameter(typeof(QuickCarSettings), "quickCarSettings", parameters), (MapControl)resolver.ResolveOrParameter(typeof(MapControl), "mapControl", parameters), (MapCamera)resolver.ResolveOrParameter(typeof(MapCamera), "mapCamera", parameters), (CarCameras)resolver.ResolveOrParameter(typeof(CarCameras), "carCameras", parameters), (SceneControl)resolver.ResolveOrParameter(typeof(SceneControl), "sceneControl", parameters), (AnimationShop)resolver.ResolveOrParameter(typeof(AnimationShop), "animationShop", parameters));
			Inject(initFreeDriveState, resolver, parameters);
			return initFreeDriveState;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
