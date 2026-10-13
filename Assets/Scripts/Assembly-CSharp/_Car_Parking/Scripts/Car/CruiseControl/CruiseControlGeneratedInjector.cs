using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Car.CruiseControl
{
	[Preserve]
	internal class CruiseControlGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CruiseControl cruiseControl = new CruiseControl((CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters), (CruiseControlConfig)resolver.ResolveOrParameter(typeof(CruiseControlConfig), "config", parameters), (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters));
			Inject(cruiseControl, resolver, parameters);
			return cruiseControl;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
