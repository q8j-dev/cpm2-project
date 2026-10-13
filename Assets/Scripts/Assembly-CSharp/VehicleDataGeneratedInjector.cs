using System;
using System.Collections.Generic;
using CPM.Connection;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using Car_Parking.Scripts.Drag.CarCompositions;
using Flags;
using MVPFree.MapIcons.Models;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Car.Visual;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class VehicleDataGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:VehicleData cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		VehicleData obj = (VehicleData)instance;
		PlayerDatas.VinylsDataUtility vinylsDataUtility = (PlayerDatas.VinylsDataUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.VinylsDataUtility), "vinylsDataUtility", parameters);
		VinylDrawer vinylDrawer = (VinylDrawer)resolver.ResolveOrParameter(typeof(VinylDrawer), "vinylDrawer", parameters);
		FlagsDataSO flagsDataSo = (FlagsDataSO)resolver.ResolveOrParameter(typeof(FlagsDataSO), "flagsDataSo", parameters);
		ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
		CarFactoryService carFactoryService = (CarFactoryService)resolver.ResolveOrParameter(typeof(CarFactoryService), "carFactoryService", parameters);
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		ColorsService colorsService = (ColorsService)resolver.ResolveOrParameter(typeof(ColorsService), "colorsService", parameters);
		IConnectionManager connectionManager = (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters);
		DefaultMaterialsConfig defaultMaterialsConfig = (DefaultMaterialsConfig)resolver.ResolveOrParameter(typeof(DefaultMaterialsConfig), "defaultMaterialsConfig", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		IPricesService pricesService = (IPricesService)resolver.ResolveOrParameter(typeof(IPricesService), "pricesService", parameters);
		MirrorComposition mirrorComposition = (MirrorComposition)resolver.ResolveOrParameter(typeof(MirrorComposition), "mirrorComposition", parameters);
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		RacingProvider racingProvider = (RacingProvider)resolver.ResolveOrParameter(typeof(RacingProvider), "racingProvider", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		OptionalDependency<MapIconsModel> mapIconModel = (OptionalDependency<MapIconsModel>)resolver.ResolveOrParameter(typeof(OptionalDependency<MapIconsModel>), "mapIconModel", parameters);
		OptionalDependency<PlayerDatas.CarLoadingUtility> carLoadingUtility = (OptionalDependency<PlayerDatas.CarLoadingUtility>)resolver.ResolveOrParameter(typeof(OptionalDependency<PlayerDatas.CarLoadingUtility>), "carLoadingUtility", parameters);
		OptionalDependency<WindshieldBrakeService> windshieldBrakeService = (OptionalDependency<WindshieldBrakeService>)resolver.ResolveOrParameter(typeof(OptionalDependency<WindshieldBrakeService>), "windshieldBrakeService", parameters);
		OptionalDependency<CameraManager> cameraManager = (OptionalDependency<CameraManager>)resolver.ResolveOrParameter(typeof(OptionalDependency<CameraManager>), "cameraManager", parameters);
		SoundBankLoadService soundBankService = (SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "soundBankService", parameters);
		obj.Construct(vinylsDataUtility, vinylDrawer, flagsDataSo, carInfoService, carFactoryService, currencyManagerController, colorsService, connectionManager, defaultMaterialsConfig, managerUI, pricesService, mirrorComposition, prefabFactory, freeDriveProvider, levelsProvider, racingProvider, carInputManager, mapIconModel, carLoadingUtility, windshieldBrakeService, cameraManager, soundBankService);
	}
}
