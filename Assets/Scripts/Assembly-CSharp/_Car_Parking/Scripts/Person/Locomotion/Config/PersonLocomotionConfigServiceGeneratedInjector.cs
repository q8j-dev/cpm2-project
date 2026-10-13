using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Person.Locomotion.Config
{
	[Preserve]
	internal class PersonLocomotionConfigServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PersonLocomotionConfigService personLocomotionConfigService = new PersonLocomotionConfigService();
			Inject(personLocomotionConfigService, resolver, parameters);
			return personLocomotionConfigService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
