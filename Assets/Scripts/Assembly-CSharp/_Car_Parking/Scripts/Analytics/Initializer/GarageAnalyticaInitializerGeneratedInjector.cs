using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;

namespace _Car_Parking.Scripts.Analytics.Initializer
{
	[Preserve]
	internal class GarageAnalyticaInitializerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GarageAnalyticaInitializer cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageAnalyticaInitializer obj = (GarageAnalyticaInitializer)instance;
			ICharacterEquipmentDataService characterEquipmentDataService = (ICharacterEquipmentDataService)resolver.ResolveOrParameter(typeof(ICharacterEquipmentDataService), "characterEquipmentDataService", parameters);
			obj.Construct(characterEquipmentDataService);
		}
	}
}
