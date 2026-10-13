using System.Collections.Generic;
using VContainer;

namespace CPM.Connection
{
	[Preserve]
	internal class ConnectionDataHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ConnectionDataHandler connectionDataHandler = new ConnectionDataHandler((IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters));
			Inject(connectionDataHandler, resolver, parameters);
			return connectionDataHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
