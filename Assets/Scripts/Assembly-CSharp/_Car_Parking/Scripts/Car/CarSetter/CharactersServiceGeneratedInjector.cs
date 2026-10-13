using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Person.Locomotion.Config;

namespace _Car_Parking.Scripts.Car.CarSetter
{
	[Preserve]
	internal class CharactersServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CharactersService charactersService = new CharactersService((ICharacterEquipmentDataService)resolver.ResolveOrParameter(typeof(ICharacterEquipmentDataService), "characterEquipmentDataService", parameters), (PersonKitConfigs)resolver.ResolveOrParameter(typeof(PersonKitConfigs), "personKitConfigs", parameters), (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters), (IPersonLocomotionConfigService)resolver.ResolveOrParameter(typeof(IPersonLocomotionConfigService), "personLocomotionConfigService", parameters), (IForcedOutfitProvider)resolver.ResolveOrParameter(typeof(IForcedOutfitProvider), "forcedOutfit", parameters));
			Inject(charactersService, resolver, parameters);
			return charactersService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
