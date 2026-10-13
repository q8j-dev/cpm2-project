using System.Collections.Generic;
using RacingScripts.RaceControllers;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyCrowd
{
	[Preserve]
	internal class RallyCrowdControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyCrowdController rallyCrowdController = new RallyCrowdController((ICPMCrowdManager)resolver.ResolveOrParameter(typeof(ICPMCrowdManager), "crowdManager", parameters), (RaceModeController)resolver.ResolveOrParameter(typeof(RaceModeController), "raceModeController", parameters), (RallyCrowdControllerConfig)resolver.ResolveOrParameter(typeof(RallyCrowdControllerConfig), "config", parameters), (Camera)resolver.ResolveOrParameter(typeof(Camera), "camera", parameters));
			Inject(rallyCrowdController, resolver, parameters);
			return rallyCrowdController;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
