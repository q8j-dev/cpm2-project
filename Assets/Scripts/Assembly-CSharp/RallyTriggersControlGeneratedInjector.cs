using System;
using System.Collections.Generic;
using Car_Parking.Scripts.Racing;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class RallyTriggersControlGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:RallyTriggersControl cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RallyTriggersControl obj = (RallyTriggersControl)instance;
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		OptionalDependency<FreeTimeAttackController> freeTimeAttackControllerDep = (OptionalDependency<FreeTimeAttackController>)resolver.ResolveOrParameter(typeof(OptionalDependency<FreeTimeAttackController>), "freeTimeAttackControllerDep", parameters);
		obj.Construct(prefabFactory, freeTimeAttackControllerDep);
	}
}
