using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.DataStorageService
{
	[Preserve]
	internal class StorageServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			StorageService storageService = new StorageService((GaragePositionsConfig)resolver.ResolveOrParameter(typeof(GaragePositionsConfig), "garagePositionsConfig", parameters));
			Inject(storageService, resolver, parameters);
			return storageService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
