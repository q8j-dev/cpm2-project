using System;
using System.Collections.Generic;
using CPM.Fallout;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class FalloutLimitsViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:FalloutLimitsView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FalloutLimitsView obj = (FalloutLimitsView)instance;
			FalloutLimitsCache limits = (FalloutLimitsCache)resolver.ResolveOrParameter(typeof(FalloutLimitsCache), "limits", parameters);
			obj.Construct(limits);
		}
	}
}
