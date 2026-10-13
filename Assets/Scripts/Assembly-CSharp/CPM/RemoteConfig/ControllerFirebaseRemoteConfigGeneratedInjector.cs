using System.Collections.Generic;
using CPM.RemoteConfig.Scriptables;
using VContainer;

namespace CPM.RemoteConfig
{
	[Preserve]
	internal class ControllerFirebaseRemoteConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ControllerFirebaseRemoteConfig controllerFirebaseRemoteConfig = new ControllerFirebaseRemoteConfig((DefaultRemoteConfig)resolver.ResolveOrParameter(typeof(DefaultRemoteConfig), "defaultRemoteConfig", parameters));
			Inject(controllerFirebaseRemoteConfig, resolver, parameters);
			return controllerFirebaseRemoteConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
