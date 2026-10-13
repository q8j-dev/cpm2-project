using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.UIElement.HUD;

namespace _Car_Parking.Scripts.Car.CruiseControl
{
	[Preserve]
	internal class CruiseControlButtonGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:CruiseControlButton cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CruiseControlButton obj = (CruiseControlButton)instance;
			CruiseControl cruiseControl = (CruiseControl)resolver.ResolveOrParameter(typeof(CruiseControl), "cruiseControl", parameters);
			HudColorChanger hudColorChanger = (HudColorChanger)resolver.ResolveOrParameter(typeof(HudColorChanger), "hudColorChanger", parameters);
			obj.Construct(cruiseControl, hudColorChanger);
		}
	}
}
