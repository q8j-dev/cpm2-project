using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace _Car_Parking.Scripts.MenuUI.Person
{
	[Preserve]
	internal class PersonClothesEventCheckerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PersonClothesEventChecker personClothesEventChecker = new PersonClothesEventChecker((IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters));
			Inject(personClothesEventChecker, resolver, parameters);
			return personClothesEventChecker;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
