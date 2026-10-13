using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace Services.Drone
{
	[Preserve]
	internal class FreeCameraControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:FreeCameraController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FreeCameraController obj = (FreeCameraController)instance;
			FreeCameraConfig config = (FreeCameraConfig)resolver.ResolveOrParameter(typeof(FreeCameraConfig), "config", parameters);
			MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
			CarCameras carCameras = (CarCameras)resolver.ResolveOrParameter(typeof(CarCameras), "carCameras", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			obj.Construct(config, menuLevelParent, carCameras, freeDriveProvider);
		}
	}
}
