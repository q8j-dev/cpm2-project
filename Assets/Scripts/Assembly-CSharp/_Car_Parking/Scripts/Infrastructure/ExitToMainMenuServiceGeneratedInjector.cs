using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.Infrastructure
{
	[Preserve]
	internal class ExitToMainMenuServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ExitToMainMenuService exitToMainMenuService = new ExitToMainMenuService((CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters), (ArenaFlowController)resolver.ResolveOrParameter(typeof(ArenaFlowController), "arenaFlow", parameters), (OpenWorldAnalytic)resolver.ResolveOrParameter(typeof(OpenWorldAnalytic), "openWorldAnalytic", parameters));
			Inject(exitToMainMenuService, resolver, parameters);
			return exitToMainMenuService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
