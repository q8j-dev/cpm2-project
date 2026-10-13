using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;

namespace _Car_Parking.Scripts.FirstGameSetup
{
	[Preserve]
	internal class GenderSwitchViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GenderSwitchView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GenderSwitchView obj = (GenderSwitchView)instance;
			ICharacterEquipmentDataService characterEquipmentDataService = (ICharacterEquipmentDataService)resolver.ResolveOrParameter(typeof(ICharacterEquipmentDataService), "characterEquipmentDataService", parameters);
			obj.Construct(characterEquipmentDataService);
		}
	}
}
