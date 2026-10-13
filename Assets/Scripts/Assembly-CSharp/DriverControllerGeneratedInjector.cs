using System;
using System.Collections.Generic;
using CPM.PlayerData;
using Car_Parking.Scripts.Racing;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class DriverControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:DriverController cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		DriverController obj = (DriverController)instance;
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		CharactersService charactersService = (CharactersService)resolver.ResolveOrParameter(typeof(CharactersService), "charactersService", parameters);
		ICharacterEquipmentDataService characterEquipmentDataService = (ICharacterEquipmentDataService)resolver.ResolveOrParameter(typeof(ICharacterEquipmentDataService), "characterEquipmentDataService", parameters);
		PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		OptionalDependency<FreeTimeAttackController> freeTimeAttackController = (OptionalDependency<FreeTimeAttackController>)resolver.ResolveOrParameter(typeof(OptionalDependency<FreeTimeAttackController>), "freeTimeAttackController", parameters);
		obj.Construct(prefabFactory, charactersService, characterEquipmentDataService, playerDataStorage, levelsProvider, carInputManager, freeTimeAttackController);
	}
}
