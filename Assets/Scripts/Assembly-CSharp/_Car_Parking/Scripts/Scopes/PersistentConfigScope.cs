using System;
using System.IO;
using CPM.Balance;
using CPM.Cdn;
using CPM.Database;
using CPM.Rating;
using CPM.RemoteConfig.Scriptables;
using CPMEngine;
using Persistence.Systems;
using Services.Drone;
using Services.PriceStyleService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using _Car_Parking.Scripts.Analytics.Root.Data;
using _Car_Parking.Scripts.Animations.Emotes;
using _Car_Parking.Scripts.Animations.Emotes.Locker;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Car.CruiseControl;
using _Car_Parking.Scripts.DataStorageService.Homes;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.GameInput.Rebinding;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.Person.Locomotion.Config;
using _Car_Parking.Scripts.PhotoMode;
using _Car_Parking.Scripts.PlayerProperty;
using _Car_Parking.Scripts.PlayerProperty.Business;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.Realty;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.VinylsAi;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Scopes
{
	[Serializable]
	public class PersistentConfigScope
	{
		private const string WheelSurfaceListPath = "WheelSurfaceDataConfig";

		private const string TyresTemperatureConfigPath = "Configs/TemperatureTyresInfo";

		private const string CruiseControlConfigPath = "Configs/CruiseControl/CruiseControlConfig";

		private const string CarDirtConfigPath = "Configs/CarDirtConfig";

		private const string SkidmarkColorConfigPath = "Configs/SkidmarkColorConfig";

		private const string AllPartsTuningConfigPath = "Configs/AllPartsTuning";

		private const string AllCarsInfoConfigPath = "Configs/AllCarsInfo";

		private const string TuningTimerToWaitConfig = "Configs/TuningTimerToWaitConfig";

		private const string LevelsConfigPath = "Configs/LevelsConfig/LevelsConfig";

		private const string MalePersonKitConfigPath = "Configs/PersonKitConfig/PersonKitConfigMale";

		private const string FemalePersonKitConfigPath = "Configs/PersonKitConfig/PersonKitConfigFemale";

		private const string GaragePositionsConfigPath = "Configs/Garage/GaragePositionsConfig";

		private const string EventStorageConfigPath = "Configs/EventStorage";

		private const string BusinessStorageConfigPath = "Configs/BusinessStorage";

		private const string EventRestrictionsConfigPath = "Configs/EventRestrictionsConfig";

		private const string AnalyticConfigPath = "CPMAnalytic/AnalyticConfig";

		private const string RealtyConfigPath = "Configs/RealtyConfig";

		private const string PriceStyleConfigPath = "Configs/PriceStyleConfig";

		private const string DefaultMaterialsConfigPath = "Configs/DefaultMaterialsConfig";

		private const string CarControlValuesConfigPath = "Configs/CarControlValuesConfig";

		private const string HomesConfigPath = "Configs/HomesConfig";

		private const string PropertyConfigPath = "Configs/PropertyConfig";

		private const string FreeCameraConfigPath = "Configs/Cameras/FreeCameraConfig";

		private const string CarCamerasConfigPath = "Configs/Cameras/CarCamerasConfigAlter";

		private const string AlterChaseCameraConfigPath = "Configs/Cameras/AlterChaseCameraConfig";

		private const string PoliceSystemConfigPath = "Configs/PoliceSystemConfig";

		private const string NewAccountSystemV2ConfigPath = "Configs/NewAccountSystemV2Config";

		private const string NewCarWindowConfigPath = "Configs/NewCarWindowConfig";

		private const string BodyTrimConfigsPath = "Configs/BodyKits/TrimsConfigs";

		private const string PurchaseItemsCollectionConfigPath = "Configs/PurchaseItem/PurchaseItemsCollection";

		private const string PhotoModeConfigPath = "Configs/PhotoMode/PhotoModeConfig";

		private const string TestOffersConfigPath = "Configs/Test/OffersConfig";

		private const string EventCollectablesPopupConfigPath = "Configs/EventCollectablesPopupConfig";

		private const string DetailVendorsConfigPath = "Configs/DetailVendorsConfig";

		private const string CarTuningDetailRaritySpritesConfigPath = "Configs/CarTuningDetailRaritySpritesConfig";

		private const string VinylsCheckApiConfigPath = "Configs/VisualTuning/New Vinyls Check Api";

		private const string BadCarConfigPath = "Configs/BadCarConfig";

		private const string BalanceJsonPath = "Balance/balance";

		private const string EventRegistryPath = "Configs/InputStorageSO/_EventRegistry";

		private const string InputMapSetEventPath = "Configs/InputStorageSO/_InputMapSetEvent";

		private const string UiNavigationValueEventPath = "Configs/InputStorageSO/_UiNavigationValueEvent";

		private const string CurrentDeviceValueEventPath = "Configs/InputStorageSO/_CurrentDeviceValueEvent";

		private const string RebindableActionsPath = "Configs/InputStorageSO/_RebindableActionsConfig";

		private const string SteeringWheelValueEventPath = "Configs/InputStorageSO/_SteeringWheelValueEvent";

		private const string DesktopControlsSettingsPath = "Configs/InputStorageSO/_DesktopControlsSettings";

		internal static readonly string[] PreloadPaths = new string[43]
		{
			"Balance/balance", "CPMAnalytic/AnalyticConfig", "Configs/TemperatureTyresInfo", "Configs/CruiseControl/CruiseControlConfig", "Configs/CarDirtConfig", "Configs/SkidmarkColorConfig", "Configs/AllPartsTuning", "Configs/LevelsConfig/LevelsConfig", "Configs/PersonKitConfig/PersonKitConfigMale", "Configs/PersonKitConfig/PersonKitConfigFemale",
			"Configs/Garage/GaragePositionsConfig", "Configs/EventStorage", "Configs/BusinessStorage", "Configs/EventRestrictionsConfig", "Configs/RealtyConfig", "Configs/PriceStyleConfig", "Configs/DefaultMaterialsConfig", "Configs/CarControlValuesConfig", "Configs/DetailVendorsConfig", "Configs/CarTuningDetailRaritySpritesConfig",
			"Configs/AllCarsInfo", "Configs/TuningTimerToWaitConfig", "Configs/HomesConfig", "Configs/PropertyConfig", "Configs/VisualTuning/New Vinyls Check Api", "Configs/Cameras/FreeCameraConfig", "Configs/Cameras/CarCamerasConfigAlter", "Configs/PoliceSystemConfig", "Configs/NewCarWindowConfig", "Configs/BodyKits/TrimsConfigs",
			"Configs/PurchaseItem/PurchaseItemsCollection", "Configs/PhotoMode/PhotoModeConfig", "Configs/EventCollectablesPopupConfig", "Configs/BadCarConfig", "Configs/AnimationData", "Configs/PoseCatalog", "Configs/EmojiCatalog", "Configs/SuitPacksConfig", "Configs/EmoteOfferBannersConfig", "Configs/WheelSurfaceVfxLibrary",
			"Graphics/GraphicsManager", "Graphics/ScreenFader", "Graphics/MirrorComposition"
		};

		[SerializeField]
		public RatingDataCollection ratingDataCollection;

		[SerializeField]
		public DataClassesConfig dataClassesConfig;

		[SerializeField]
		public DefaultRemoteConfig defaultRemoteConfig;

		[SerializeField]
		public VersionedDataClassesConfig versionedDataClassesConfig;

		[SerializeField]
		public ShadersVaultConfig shadersVaultConfig;

		[SerializeField]
		private SoundEventChannel soundEventChannel = default;

		public void Configure(IContainerBuilder builder)
		{
			TextAsset textAsset = PersistentConfigPreloader.Get<TextAsset>("Balance/balance");
			byte[] bundledBytes = ((textAsset != null) ? textAsset.bytes : null);
			string text = BalanceCdnUrl.Build();
			EtagFileCache cache = new EtagFileCache(Path.Combine(Application.persistentDataPath, "balance"), "balance.json");
			BalanceDocumentStore balanceDocumentStore = new BalanceDocumentStore();
			BalanceBootSeeder.Seed(cache, text, bundledBytes, balanceDocumentStore);
			builder.RegisterInstance((IBalanceDocumentStore)balanceDocumentStore);
			builder.Register((Func<IObjectResolver, IInstallTimeCatalog>)((IObjectResolver c) => new BalanceInstallTimeCatalog(c.Resolve<IBalanceDocumentStore>())), Lifetime.Singleton);
			builder.RegisterInstance((IBalanceDocumentSynchronizer)new BalanceDocumentSynchronizer(new CdnFetchService(new UnityWebRequestCdnClient()), cache, text, balanceDocumentStore));
			if (shadersVaultConfig != null)
			{
				shadersVaultConfig.Init();
				builder.RegisterInstance(shadersVaultConfig);
			}
			LoadWheelSurfaceDataAsync();
			ratingDataCollection.Initialize();
			builder.RegisterInstance(ratingDataCollection);
			builder.RegisterInstance(PersistentConfigPreloader.Get<AnalyticConfig>("CPMAnalytic/AnalyticConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<TemperatureTyresInfo>("Configs/TemperatureTyresInfo"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<CruiseControlConfig>("Configs/CruiseControl/CruiseControlConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<CarDirtConfig>("Configs/CarDirtConfig"));
			SkidmarkColorContainer.Construct(PersistentConfigPreloader.Get<SkidmarkColorConfigSO>("Configs/SkidmarkColorConfig"));
			AllParts instance = PersistentConfigPreloader.Get<AllParts>("Configs/AllPartsTuning");
			builder.RegisterInstance(instance).AsImplementedInterfaces().AsSelf();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<AllParts>().Initialize();
			});
			builder.RegisterInstance(PersistentConfigPreloader.Get<LevelsConfig>("Configs/LevelsConfig/LevelsConfig"));
			PersonKitConfig male = PersistentConfigPreloader.Get<PersonKitConfig>("Configs/PersonKitConfig/PersonKitConfigMale");
			PersonKitConfig female = PersistentConfigPreloader.Get<PersonKitConfig>("Configs/PersonKitConfig/PersonKitConfigFemale");
			builder.RegisterInstance(new PersonKitConfigs(male, female));
			builder.Register<PersonLocomotionConfigService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<PersonActivation>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.RegisterInstance(PersistentConfigPreloader.Get<GaragePositionsConfig>("Configs/Garage/GaragePositionsConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<EventStorage>("Configs/EventStorage"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<BusinessStorage>("Configs/BusinessStorage"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<EventRestrictionsConfig>("Configs/EventRestrictionsConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<RealtyConfig>("Configs/RealtyConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<PriceStyleConfig>("Configs/PriceStyleConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<DefaultMaterialsConfig>("Configs/DefaultMaterialsConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<CarControlValuesSO>("Configs/CarControlValuesConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<DetailVendorsConfigs>("Configs/DetailVendorsConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<CarTuningDetailRaritySpritesConfig>("Configs/CarTuningDetailRaritySpritesConfig"));
			AllCarsInfo allCarsInfo = PersistentConfigPreloader.Get<AllCarsInfo>("Configs/AllCarsInfo");
			builder.RegisterInstance(allCarsInfo);
			builder.RegisterBuildCallback((IObjectResolver container) =>
			{
				container.Inject(allCarsInfo);
			});
			builder.RegisterInstance(PersistentConfigPreloader.Get<TimesToWaitTuningConfig>("Configs/TuningTimerToWaitConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<HomesConfig>("Configs/HomesConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<PropertyConfig>("Configs/PropertyConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<VinylsCheckApi>("Configs/VisualTuning/New Vinyls Check Api"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<FreeCameraConfig>("Configs/Cameras/FreeCameraConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<CarCamerasParametersConfig>("Configs/Cameras/CarCamerasConfigAlter"));
			builder.RegisterInstance(Resources.Load<AlterChaseCameraConfig>("Configs/Cameras/AlterChaseCameraConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<PoliceSystemConfig>("Configs/PoliceSystemConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<NewCarWindowConfig>("Configs/NewCarWindowConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<BodyKitsTrimConfigs>("Configs/BodyKits/TrimsConfigs"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<PurchaseItemsCollection>("Configs/PurchaseItem/PurchaseItemsCollection"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<PhotoModeConfig>("Configs/PhotoMode/PhotoModeConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<EventCollectablesPopupConfig>("Configs/EventCollectablesPopupConfig"));
			builder.RegisterInstance(PersistentConfigPreloader.Get<BadCarConfig>("Configs/BadCarConfig"));
			builder.RegisterInstance(new EmoteCatalog(PersistentConfigPreloader.Get<AnimationDataSO>("Configs/AnimationData"), PersistentConfigPreloader.Get<PoseCatalogSO>("Configs/PoseCatalog")));
			builder.RegisterInstance(PersistentConfigPreloader.Get<EmojiCatalogSO>("Configs/EmojiCatalog"));
			builder.RegisterInstance(defaultRemoteConfig);
			builder.RegisterInstance(versionedDataClassesConfig);
			builder.RegisterInstance(soundEventChannel);
			builder.RegisterInstance(dataClassesConfig);
			builder.RegisterInstance(Resources.Load<EventRegistry>("Configs/InputStorageSO/_EventRegistry"));
			builder.RegisterInstance(Resources.Load<UiNavigationValueEvent>("Configs/InputStorageSO/_UiNavigationValueEvent"));
			builder.RegisterInstance(Resources.Load<InputMapSetEvent>("Configs/InputStorageSO/_InputMapSetEvent"));
			builder.RegisterInstance(Resources.Load<CurrentDeviceValueEvent>("Configs/InputStorageSO/_CurrentDeviceValueEvent"));
			builder.RegisterInstance(Resources.Load<RebindableActionsConfig>("Configs/InputStorageSO/_RebindableActionsConfig"));
			builder.RegisterInstance(Resources.Load<SteeringWheelValueEvent>("Configs/InputStorageSO/_SteeringWheelValueEvent"));
			DesktopControlsSettingsSO instance2 = Resources.Load<DesktopControlsSettingsSO>("Configs/InputStorageSO/_DesktopControlsSettings");
			builder.RegisterInstance(instance2);
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<DesktopControlsSettingsSO>().Load();
			});
			PersistentConfigPreloader.ReportAndClear();
		}

		private void LoadWheelSurfaceDataAsync()
		{
			AsyncOperationHandle<WheelSurfaceDataListSO> handle = default;
			try
			{
				handle = Addressables.LoadAssetAsync<WheelSurfaceDataListSO>("WheelSurfaceDataConfig");
				handle.Completed += (AsyncOperationHandle<WheelSurfaceDataListSO> operationHandle) =>
				{
					WheelSurfaceConfigContainer.Construct(operationHandle.Result);
					Addressables.Release(handle);
				};
			}
			catch (Exception message)
			{
				if (handle.IsValid())
				{
					Addressables.Release(handle);
				}
				Debug.LogError(message);
			}
		}
	}
}
