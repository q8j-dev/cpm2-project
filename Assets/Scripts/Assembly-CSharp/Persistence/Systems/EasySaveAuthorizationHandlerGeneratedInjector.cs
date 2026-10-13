using System.Collections.Generic;
using VContainer;

namespace Persistence.Systems
{
	[Preserve]
	internal class EasySaveAuthorizationHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EasySaveAuthorizationHandler easySaveAuthorizationHandler = new EasySaveAuthorizationHandler();
			Inject(easySaveAuthorizationHandler, resolver, parameters);
			return easySaveAuthorizationHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
