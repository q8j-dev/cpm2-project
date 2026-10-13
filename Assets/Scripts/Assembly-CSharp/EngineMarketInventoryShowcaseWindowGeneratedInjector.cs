using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;

[Preserve]
internal class EngineMarketInventoryShowcaseWindowGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:EngineMarketInventoryShowcaseWindow cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		EngineMarketInventoryShowcaseWindow obj = (EngineMarketInventoryShowcaseWindow)instance;
		AllParts allParts = (AllParts)resolver.ResolveOrParameter(typeof(AllParts), "allParts", parameters);
		obj.Construct(allParts);
	}
}
