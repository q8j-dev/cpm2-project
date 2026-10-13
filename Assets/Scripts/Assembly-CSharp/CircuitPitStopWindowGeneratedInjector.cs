using System;
using System.Collections.Generic;
using RacingScripts.RaceControllers;
using VContainer;
using _Car_Parking.Scripts.Drag;

[Preserve]
internal class CircuitPitStopWindowGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CircuitPitStopWindow cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CircuitPitStopWindow obj = (CircuitPitStopWindow)instance;
		RaceModeController raceModeController = (RaceModeController)resolver.ResolveOrParameter(typeof(RaceModeController), "raceModeController", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		obj.Construct(raceModeController, managerUI);
	}
}
