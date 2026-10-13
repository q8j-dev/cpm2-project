using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Police
{
	[Preserve]
	internal class PoliceButtonsControllerRegistryGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PoliceButtonsControllerRegistry policeButtonsControllerRegistry = new PoliceButtonsControllerRegistry();
			Inject(policeButtonsControllerRegistry, resolver, parameters);
			return policeButtonsControllerRegistry;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
