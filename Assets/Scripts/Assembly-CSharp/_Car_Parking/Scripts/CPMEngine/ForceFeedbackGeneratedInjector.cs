using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.CPMEngine
{
	[Preserve]
	internal class ForceFeedbackGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ForceFeedback forceFeedback = new ForceFeedback();
			Inject(forceFeedback, resolver, parameters);
			return forceFeedback;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ForceFeedback obj = (ForceFeedback)instance;
			SteeringWheelValueEvent steeringWheelValueEvent = (SteeringWheelValueEvent)resolver.ResolveOrParameter(typeof(SteeringWheelValueEvent), "steeringWheelValueEvent", parameters);
			obj.Construct(steeringWheelValueEvent);
		}
	}
}
