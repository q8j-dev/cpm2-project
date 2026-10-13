using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	[Preserve]
	internal class CurrentDeviceValueEventGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CurrentDeviceValueEvent currentDeviceValueEvent = new CurrentDeviceValueEvent();
			Inject(currentDeviceValueEvent, resolver, parameters);
			return currentDeviceValueEvent;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
