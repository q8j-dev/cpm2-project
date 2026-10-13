using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Car.Physics
{
	[Preserve]
	internal class MozaPollServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MozaPollService mozaPollService = new MozaPollService();
			Inject(mozaPollService, resolver, parameters);
			return mozaPollService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
