using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;

[Preserve]
internal class UIPartCategoryCheckerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:UIPartCategoryChecker cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		UIPartCategoryChecker obj = (UIPartCategoryChecker)instance;
		AllParts allParts = (AllParts)resolver.ResolveOrParameter(typeof(AllParts), "allParts", parameters);
		obj.Construct(allParts);
	}
}
