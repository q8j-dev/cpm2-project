using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.CPMEngine;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.DiscordPresence;
using _Car_Parking.Scripts.GameInput;
using _Car_Parking.Scripts.GameInput.Rebinding;
using _Car_Parking.Scripts.Steam;
using _Car_Parking.Scripts.Steam.Account;
using _Car_Parking.Scripts.Steam.Achievements;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Steam.Graphics;
using _Car_Parking.Scripts.Steam.InputHints;
using _Car_Parking.Scripts.Steam.Purchase;
using _Car_Parking.Scripts.Steam.Session;

namespace _Car_Parking.Scripts.Scopes
{
	public class SteamScope
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<GameInputManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.RegisterInstance(Resources.Load<HapticConfigSO>("Configs/HapticConfig"));
			builder.RegisterEntryPoint<GamepadMirrorTracker>().AsSelf();
			builder.RegisterEntryPoint<GamepadHapticService>().AsSelf();
			builder.Register<ForceFeedback>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.RegisterEntryPoint<MozaPollService>().AsSelf();
			builder.RegisterEntryPoint<SteeringWheelSettingsService>().AsSelf();
			builder.Register<SteamItemCatalog>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<SteamPurchaseService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.RegisterEntryPoint<InputDeviceDetector>().AsSelf();
			builder.Register<SteamItemApplier>(Lifetime.Singleton).As<ISteamItemApplier>();
			builder.Register<SteamInventoryService>(Lifetime.Singleton).As<ISteamInventoryService>();
			builder.Register<SteamAccountService>(Lifetime.Singleton).As<ISteamAccountService>();
			builder.Register<SteamPurchaseRestoreService>(Lifetime.Singleton).As<ISteamPurchaseRestoreService>();
			builder.Register<SteamPurchaseGate>(Lifetime.Singleton).As<ISteamPurchaseGate>();
			builder.Register<SteamAchievementService>(Lifetime.Singleton).As<ISteamAchievementService>();
			builder.Register<BindingsPrefsStore>(Lifetime.Singleton);
			builder.RegisterEntryPoint<ActiveWheelService>().AsSelf();
			builder.Register<BindingConflictResolver>(Lifetime.Singleton);
			builder.RegisterEntryPoint<BindingOverridesService>().AsSelf();
			builder.Register<RebindController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.RegisterEntryPoint<SteamCarPreviewPreloadService>().AsSelf();
			builder.RegisterEntryPoint<DesktopSessionWatchdog>().AsSelf();
			builder.RegisterEntryPoint<FpvShadowCascadeService>().AsSelf();
			builder.Register((IObjectResolver _) => Resources.Load<DiscordPresenceConfigSO>("Configs/DiscordPresenceConfig"), Lifetime.Singleton);
			builder.RegisterEntryPoint<DiscordPresenceService>().As<IDiscordPresenceService>();
		}
	}
}
