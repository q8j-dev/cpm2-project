using System.Collections.Generic;
using VContainer;

namespace CPM.Database
{
	[Preserve]
	internal class DatabaseOutputHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DatabaseOutputHandler databaseOutputHandler = new DatabaseOutputHandler();
			Inject(databaseOutputHandler, resolver, parameters);
			return databaseOutputHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
