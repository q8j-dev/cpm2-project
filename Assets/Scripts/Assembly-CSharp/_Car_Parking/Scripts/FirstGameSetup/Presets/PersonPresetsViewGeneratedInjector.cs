using System;
using System.Collections.Generic;
using CPM.Scripts.Garage;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.FirstGameSetup.Presets
{
	[Preserve]
	internal class PersonPresetsViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PersonPresetsView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PersonPresetsView obj = (PersonPresetsView)instance;
			CharactersService charactersService = (CharactersService)resolver.ResolveOrParameter(typeof(CharactersService), "charactersService", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			IGarageSelectorService garageSelectorService = (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters);
			GaragePositionsConfig garagePositionsConfig = (GaragePositionsConfig)resolver.ResolveOrParameter(typeof(GaragePositionsConfig), "garagePositionsConfig", parameters);
			ICharacterEquipmentDataService characterEquipmentDataService = (ICharacterEquipmentDataService)resolver.ResolveOrParameter(typeof(ICharacterEquipmentDataService), "characterEquipmentDataService", parameters);
			PresetGrantSession grantSession = (PresetGrantSession)resolver.ResolveOrParameter(typeof(PresetGrantSession), "grantSession", parameters);
			OptionalDependency<CameraManager> cameraManager = (OptionalDependency<CameraManager>)resolver.ResolveOrParameter(typeof(OptionalDependency<CameraManager>), "cameraManager", parameters);
			obj.Construct(charactersService, prefabFactory, garageSelectorService, garagePositionsConfig, characterEquipmentDataService, grantSession, cameraManager);
		}
	}
}
