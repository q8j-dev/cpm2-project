using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class HornButtonHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:HornButtonHandler cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			HornButtonHandler obj = (HornButtonHandler)instance;
			QuickCarSettings quickCarSettings = (QuickCarSettings)resolver.ResolveOrParameter(typeof(QuickCarSettings), "quickCarSettings", parameters);
			obj.Construct(quickCarSettings);
		}
	}
}
