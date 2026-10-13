using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	[Preserve]
	internal class RebindControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RebindController rebindController = new RebindController((InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters), (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters));
			Inject(rebindController, resolver, parameters);
			return rebindController;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
