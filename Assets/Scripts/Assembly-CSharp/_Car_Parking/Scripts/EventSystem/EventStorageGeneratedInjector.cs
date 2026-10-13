using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	internal class EventStorageGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventStorage eventStorage = new EventStorage();
			Inject(eventStorage, resolver, parameters);
			return eventStorage;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
