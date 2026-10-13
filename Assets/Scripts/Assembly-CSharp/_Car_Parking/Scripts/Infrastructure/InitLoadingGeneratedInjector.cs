using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.Infrastructure
{
	[Preserve]
	internal class InitLoadingGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:InitLoading cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			InitLoading obj = (InitLoading)instance;
			SceneLoadingTimer sceneLoadingTimer = (SceneLoadingTimer)resolver.ResolveOrParameter(typeof(SceneLoadingTimer), "sceneLoadingTimer", parameters);
			obj.Construct(sceneLoadingTimer);
		}
	}
}
