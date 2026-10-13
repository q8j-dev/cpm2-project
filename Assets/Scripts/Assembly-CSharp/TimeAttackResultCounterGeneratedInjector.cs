using System.Collections.Generic;
using Car_Parking.Scripts.Racing;
using VContainer;
using _Car_Parking.Scripts.EventSystem;

[Preserve]
internal class TimeAttackResultCounterGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		TimeAttackResultCounter timeAttackResultCounter = new TimeAttackResultCounter();
		Inject(timeAttackResultCounter, resolver, parameters);
		return timeAttackResultCounter;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		TimeAttackResultCounter obj = (TimeAttackResultCounter)instance;
		FreeDriveControl freeDriveControl = (FreeDriveControl)resolver.ResolveOrParameter(typeof(FreeDriveControl), "freeDriveControl", parameters);
		FreeTimeAttackController timeAttackController = (FreeTimeAttackController)resolver.ResolveOrParameter(typeof(FreeTimeAttackController), "timeAttackController", parameters);
		EventRestrictionService eventRestrictionService = (EventRestrictionService)resolver.ResolveOrParameter(typeof(EventRestrictionService), "eventRestrictionService", parameters);
		obj.Construct(freeDriveControl, timeAttackController, eventRestrictionService);
	}
}
