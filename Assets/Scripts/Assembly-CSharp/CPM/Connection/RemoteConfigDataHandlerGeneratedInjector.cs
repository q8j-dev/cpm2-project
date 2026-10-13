using System.Collections.Generic;
using VContainer;

namespace CPM.Connection
{
	[Preserve]
	internal class RemoteConfigDataHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RemoteConfigDataHandler remoteConfigDataHandler = new RemoteConfigDataHandler();
			Inject(remoteConfigDataHandler, resolver, parameters);
			return remoteConfigDataHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
