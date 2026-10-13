using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class CustomCameraSettingsWindowGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:CustomCameraSettingsWindow cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CustomCameraSettingsWindow obj = (CustomCameraSettingsWindow)instance;
			CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
			obj.Construct(coreGameManager);
		}
	}
}
