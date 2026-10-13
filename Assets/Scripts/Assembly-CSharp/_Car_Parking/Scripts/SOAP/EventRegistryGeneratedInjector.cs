using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.SOAP
{
	[Preserve]
	internal class EventRegistryGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventRegistry eventRegistry = new EventRegistry();
			Inject(eventRegistry, resolver, parameters);
			return eventRegistry;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
