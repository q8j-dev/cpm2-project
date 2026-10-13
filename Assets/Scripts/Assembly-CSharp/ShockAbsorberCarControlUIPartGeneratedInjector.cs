using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;

[Preserve]
internal class ShockAbsorberCarControlUIPartGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:ShockAbsorberCarControlUIPart cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		ShockAbsorberCarControlUIPart obj = (ShockAbsorberCarControlUIPart)instance;
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		obj.ConstructBase(managerUI);
	}
}
