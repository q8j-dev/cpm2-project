using System.Collections.Generic;
using CPM.Scripts.Garage;
using Cinemachine;
using VContainer;

namespace Racing.CameraManagement
{
	[Preserve]
	internal class CustomMoveCameraToTargetGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CustomMoveCameraToTarget customMoveCameraToTarget = new CustomMoveCameraToTarget();
			Inject(customMoveCameraToTarget, resolver, parameters);
			return customMoveCameraToTarget;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CustomMoveCameraToTarget obj = (CustomMoveCameraToTarget)instance;
			GaragePositionsConfig garagePositionsConfig = (GaragePositionsConfig)resolver.ResolveOrParameter(typeof(GaragePositionsConfig), "garagePositionsConfig", parameters);
			CameraManager cameraManager = (CameraManager)resolver.ResolveOrParameter(typeof(CameraManager), "cameraManager", parameters);
			IGarageSelectorService garageSelectorService = (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters);
			CinemachineBrain cinemachineBrain = (CinemachineBrain)resolver.ResolveOrParameter(typeof(CinemachineBrain), "cinemachineBrain", parameters);
			obj.Construct(garagePositionsConfig, cameraManager, garageSelectorService, cinemachineBrain);
		}
	}
}
