using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Police
{
	[Preserve]
	internal class PoliceCarBodyKitsSetterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PoliceCarBodyKitsSetter cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PoliceCarBodyKitsSetter obj = (PoliceCarBodyKitsSetter)instance;
			IPoliceSystem policeSystem = (IPoliceSystem)resolver.ResolveOrParameter(typeof(IPoliceSystem), "policeSystem", parameters);
			obj.Construct(policeSystem);
		}
	}
}
