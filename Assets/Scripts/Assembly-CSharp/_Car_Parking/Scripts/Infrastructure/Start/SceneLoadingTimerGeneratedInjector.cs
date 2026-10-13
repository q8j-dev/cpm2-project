using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	[Preserve]
	internal class SceneLoadingTimerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SceneLoadingTimer sceneLoadingTimer = new SceneLoadingTimer();
			Inject(sceneLoadingTimer, resolver, parameters);
			return sceneLoadingTimer;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
