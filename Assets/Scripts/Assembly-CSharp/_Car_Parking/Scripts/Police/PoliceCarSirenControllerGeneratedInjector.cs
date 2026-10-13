using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Police
{
	[Preserve]
	internal class PoliceCarSirenControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PoliceCarSirenController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PoliceCarSirenController obj = (PoliceCarSirenController)instance;
			IPoliceSystem policeSystem = (IPoliceSystem)resolver.ResolveOrParameter(typeof(IPoliceSystem), "policeSystem", parameters);
			PoliceSystemConfig policeSystemConfig = (PoliceSystemConfig)resolver.ResolveOrParameter(typeof(PoliceSystemConfig), "policeSystemConfig", parameters);
			obj.Construct(policeSystem, policeSystemConfig);
		}
	}
}
