using System;
using System.Collections.Generic;
using VContainer;

[Preserve]
internal class FPSDisplay2GeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:FPSDisplay2 cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		FPSDisplay2 obj = (FPSDisplay2)instance;
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		obj.Construct(menuLevelParent);
	}
}
