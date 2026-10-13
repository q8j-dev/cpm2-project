using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	[Preserve]
	internal class PersonActivationGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PersonActivation personActivation = new PersonActivation();
			Inject(personActivation, resolver, parameters);
			return personActivation;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
