using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	[Preserve]
	internal class DataVersionsServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DataVersionsService dataVersionsService = new DataVersionsService();
			Inject(dataVersionsService, resolver, parameters);
			return dataVersionsService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
