using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Free_drive
{
	[Preserve]
	internal class TriggerSceneLoaderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:TriggerSceneLoader cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TriggerSceneLoader obj = (TriggerSceneLoader)instance;
			SceneControl sceneControl = (SceneControl)resolver.ResolveOrParameter(typeof(SceneControl), "sceneControl", parameters);
			obj.Construct(sceneControl);
		}
	}
}
