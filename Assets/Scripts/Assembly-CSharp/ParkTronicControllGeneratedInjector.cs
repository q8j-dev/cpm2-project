using System;
using System.Collections.Generic;
using VContainer;

[Preserve]
internal class ParkTronicControllGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:ParkTronicControll cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		ParkTronicControll obj = (ParkTronicControll)instance;
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		obj.Construct(carInputManager);
	}
}
