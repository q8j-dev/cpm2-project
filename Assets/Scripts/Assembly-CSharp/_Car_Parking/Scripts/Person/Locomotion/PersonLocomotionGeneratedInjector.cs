using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Person.Locomotion.Config;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	[Preserve]
	internal class PersonLocomotionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PersonLocomotion cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PersonLocomotion obj = (PersonLocomotion)instance;
			IPersonLocomotionConfigService configService = (IPersonLocomotionConfigService)resolver.ResolveOrParameter(typeof(IPersonLocomotionConfigService), "configService", parameters);
			IPersonActivation activation = (IPersonActivation)resolver.ResolveOrParameter(typeof(IPersonActivation), "activation", parameters);
			IObjectResolver resolver2 = (IObjectResolver)resolver.ResolveOrParameter(typeof(IObjectResolver), "resolver", parameters);
			obj.Construct(configService, activation, resolver2);
		}
	}
}
