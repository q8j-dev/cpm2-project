using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.SOAP_Registry;

[Preserve]
internal class AllInventoryFilterWindowGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:AllInventoryFilterWindow cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		AllInventoryFilterWindow obj = (AllInventoryFilterWindow)instance;
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		obj.ConstructBase(managerUI, inputMapSetEvent);
	}
}
