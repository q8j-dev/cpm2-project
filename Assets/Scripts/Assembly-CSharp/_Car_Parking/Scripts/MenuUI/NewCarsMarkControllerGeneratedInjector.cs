using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI
{
	[Preserve]
	internal class NewCarsMarkControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			NewCarsMarkController newCarsMarkController = new NewCarsMarkController();
			Inject(newCarsMarkController, resolver, parameters);
			return newCarsMarkController;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
