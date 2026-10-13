using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class GearboxFilterWindowGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GearboxFilterWindow cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GearboxFilterWindow obj = (GearboxFilterWindow)instance;
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
			obj.ConstructBase(managerUI, inputMapSetEvent);
		}
	}
}
