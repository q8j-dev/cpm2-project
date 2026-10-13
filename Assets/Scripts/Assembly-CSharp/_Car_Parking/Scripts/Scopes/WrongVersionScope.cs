using CPM.Account;
using CPM.Database;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.Account;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.VisualTuningFavourite;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

namespace _Car_Parking.Scripts.Scopes
{
	public class WrongVersionScope : LifetimeScope
	{
		[SerializeField]
		private ManagerUI ManagerUI = default;

		protected override void Configure(IContainerBuilder builder)
		{
			builder.Register<PrefabFactory>(Lifetime.Singleton).As<IPrefabFactory>();
			builder.RegisterEntryPoint<DataBase>().AsSelf();
			builder.Register<PlayerStorageService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<CarInfoService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<RegistrationController>(Lifetime.Singleton).As<IRegistrationController>();
			builder.Register<VehicleSlotManager>(Lifetime.Singleton).As<ISlotService>();
			builder.Register<VehicleSlotPaymentService>(Lifetime.Singleton);
			builder.Register<ProgressContainer>(Lifetime.Singleton);
			builder.RegisterEntryPoint<TimerTuningService>().As<ITimerTuningService>().As<IAuthorizationDependant>();
			builder.Register<EngineTimerAdService>(Lifetime.Singleton).As<IEngineTimerAdService>();
			builder.RegisterEntryPoint<ClientAdRewardRecoveryService>();
			builder.RegisterEntryPoint<EngineTuningTimerService>().As<IEngineTuningTimerService>();
			builder.RegisterInstance(Resources.Load<AllWheelsCounterConfig>("Configs/ExteriorTuning/AllWheelsCounterConfig"));
			builder.RegisterComponent(ManagerUI);
		}
	}
}
