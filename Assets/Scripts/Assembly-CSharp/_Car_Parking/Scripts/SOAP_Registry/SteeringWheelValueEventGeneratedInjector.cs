using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	[Preserve]
	internal class SteeringWheelValueEventGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SteeringWheelValueEvent steeringWheelValueEvent = new SteeringWheelValueEvent();
			Inject(steeringWheelValueEvent, resolver, parameters);
			return steeringWheelValueEvent;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
