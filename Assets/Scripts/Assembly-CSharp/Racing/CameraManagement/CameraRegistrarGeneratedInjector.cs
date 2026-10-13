using System;
using System.Collections.Generic;
using VContainer;

namespace Racing.CameraManagement
{
	[Preserve]
	internal class CameraRegistrarGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:CameraRegistrar cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CameraRegistrar obj = (CameraRegistrar)instance;
			CameraManager cameraManager = (CameraManager)resolver.ResolveOrParameter(typeof(CameraManager), "cameraManager", parameters);
			obj.Construct(cameraManager);
		}
	}
}
