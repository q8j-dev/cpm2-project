using System.Collections.Generic;
using VContainer;

namespace CPM.RemoteConfig.Scriptables
{
	[Preserve]
	internal class DefaultRemoteConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DefaultRemoteConfig defaultRemoteConfig = new DefaultRemoteConfig();
			Inject(defaultRemoteConfig, resolver, parameters);
			return defaultRemoteConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
