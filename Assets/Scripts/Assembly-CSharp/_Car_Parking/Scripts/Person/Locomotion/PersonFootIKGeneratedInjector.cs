using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Person.Locomotion.Config;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	[Preserve]
	internal class PersonFootIKGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PersonFootIK cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PersonFootIK obj = (PersonFootIK)instance;
			IPersonLocomotionConfigService configService = (IPersonLocomotionConfigService)resolver.ResolveOrParameter(typeof(IPersonLocomotionConfigService), "configService", parameters);
			obj.Construct(configService);
		}
	}
}
