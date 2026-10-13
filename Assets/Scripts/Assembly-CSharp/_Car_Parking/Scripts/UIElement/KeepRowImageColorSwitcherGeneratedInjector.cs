using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class KeepRowImageColorSwitcherGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:KeepRowImageColorSwitcher cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			KeepRowImageColorSwitcher obj = (KeepRowImageColorSwitcher)instance;
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			obj.Construct(freeDriveProvider);
		}
	}
}
