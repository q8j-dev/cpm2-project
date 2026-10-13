using System;
using System.Collections.Generic;
using Car_Parking.Scripts.Racing;
using VContainer;

[Preserve]
internal class RaceFinishLineTriggerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:RaceFinishLineTrigger cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RaceFinishLineTrigger obj = (RaceFinishLineTrigger)instance;
		FreeTimeAttackController controller = (FreeTimeAttackController)resolver.ResolveOrParameter(typeof(FreeTimeAttackController), "controller", parameters);
		obj.Construct(controller);
	}
}
