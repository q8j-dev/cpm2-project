using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	internal class RallyCarGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:RallyCar cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyCar obj = (RallyCar)instance;
			QuickCarSettings quickCarSettings = (QuickCarSettings)resolver.ResolveOrParameter(typeof(QuickCarSettings), "quickCarSettings", parameters);
			obj.Construct(quickCarSettings);
		}
	}
}
