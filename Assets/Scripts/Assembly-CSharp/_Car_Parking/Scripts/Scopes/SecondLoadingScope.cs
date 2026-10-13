using CPM.Database;
using Rating.DailyTasks.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Initializer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.VisualTuningFavourite;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

namespace _Car_Parking.Scripts.Scopes
{
	[Preserve]
	public class SecondLoadingScope : LifetimeScope
	{
		[SerializeField]
		private GarageAnalyticaInitializer GarageAnalyticaInitializer;

		private void OnEnable()
		{
			Debug.Log("SecondLoadingScope OnEnable");
		}

		private void Start()
		{
			Debug.Log("SecondLoadingScope Start");
			Debug.Log($"SecondLoadingScope built. Container={base.Container != null}, Parent={base.Parent?.name}");
		}

		protected override void Configure(IContainerBuilder builder)
		{
			Debug.Log("SecondLoadingScope Configure ");
			builder.Register<PreMenuState>(Lifetime.Singleton);
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<PreMenuState>();
			});
			builder.Register<PlayerStorageService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<VehicleSlotManager>(Lifetime.Singleton).As<ISlotService>();
			builder.RegisterInstance(Resources.Load<AllWheelsCounterConfig>("Configs/ExteriorTuning/AllWheelsCounterConfig"));
			builder.RegisterEntryPoint<DataBase>().AsSelf();
			builder.Register<CarInfoService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<VehicleSlotPaymentService>(Lifetime.Singleton);
			builder.Register<ProgressContainer>(Lifetime.Singleton);
			builder.RegisterEntryPoint<TimerTuningService>().As<ITimerTuningService>();
			builder.Register<EngineTimerAdService>(Lifetime.Singleton).As<IEngineTimerAdService>();
			builder.RegisterEntryPoint<ClientAdRewardRecoveryService>();
			builder.RegisterEntryPoint<EngineTuningTimerService>().As<IEngineTuningTimerService>();
			builder.Register<DailyServicesInitializer>(Lifetime.Singleton).As<IDailyServicesDependant>();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<IDailyServicesDependant>();
			});
			builder.RegisterEntryPoint<BootstrapHandler>().As<IBootstrapHandler>().AsSelf();
			builder.RegisterEntryPoint<SecondLoadingStartup>();
			builder.RegisterComponentInHierarchy<SecondLoadingInit>();
			builder.RegisterComponent(GarageAnalyticaInitializer);
		}
	}
}
