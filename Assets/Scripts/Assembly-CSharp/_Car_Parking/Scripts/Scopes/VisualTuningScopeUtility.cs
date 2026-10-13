using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.MenuUI.VisualTuning;
using _Car_Parking.Scripts.Other.FsoStrobeLights;
using _Car_Parking.Scripts.VisualTuningFavourite;

namespace _Car_Parking.Scripts.Scopes
{
	public static class VisualTuningScopeUtility
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<BodyKitsVisualScope>(Lifetime.Singleton);
			builder.Register<ColorsScope>(Lifetime.Singleton);
			builder.Register<FlagsVisualScope>(Lifetime.Singleton);
			builder.Register<FsoTuningScope>(Lifetime.Singleton);
			builder.Register<WheelsTuningScope>(Lifetime.Singleton);
			builder.Register<MainScope>(Lifetime.Singleton);
			builder.Register<BrakesVisualScope>(Lifetime.Singleton);
			builder.Register<CalipersVisualTuningScope>(Lifetime.Singleton);
			builder.Register<PoliceVisualScope>(Lifetime.Singleton);
			builder.Register<VisualTuningPricesService>(Lifetime.Singleton);
			builder.RegisterInstance(Resources.Load<WheelsDiskTuningConfig>("Configs/ExteriorTuning/WheelsTuningConfig"));
			builder.RegisterInstance(Resources.Load<BrakeTuningConfig>("Configs/ExteriorTuning/BrakesTuningConfig"));
			builder.RegisterInstance(Resources.Load<AllWheelsCounterConfig>("Configs/ExteriorTuning/AllWheelsCounterConfig"));
		}
	}
}
