using System;
using System.Collections.Generic;
using VContainer;

[Preserve]
internal class ExteriorTunningUIGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:ExteriorTunningUI cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		((ExteriorTunningUI)instance).Construct();
	}
}
