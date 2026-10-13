using System.Collections.Generic;
using RacingScripts.RaceControllers;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyProps
{
	[Preserve]
	internal class RallyPropControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyPropController rallyPropController = new RallyPropController((IRallyPropManager)resolver.ResolveOrParameter(typeof(IRallyPropManager), "propManager", parameters), (RaceModeController)resolver.ResolveOrParameter(typeof(RaceModeController), "raceModeController", parameters), (RallyPropControllerConfig)resolver.ResolveOrParameter(typeof(RallyPropControllerConfig), "config", parameters), (Camera)resolver.ResolveOrParameter(typeof(Camera), "camera", parameters));
			Inject(rallyPropController, resolver, parameters);
			return rallyPropController;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
