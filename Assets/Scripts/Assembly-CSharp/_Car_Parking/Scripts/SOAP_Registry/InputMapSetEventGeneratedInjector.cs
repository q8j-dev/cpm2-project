using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	[Preserve]
	internal class InputMapSetEventGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			InputMapSetEvent inputMapSetEvent = new InputMapSetEvent();
			Inject(inputMapSetEvent, resolver, parameters);
			return inputMapSetEvent;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
