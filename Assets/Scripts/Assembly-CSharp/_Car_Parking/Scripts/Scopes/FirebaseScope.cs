using CPM.Account;
using CPM.Database;
using CPM.RemoteConfig;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Database;

namespace _Car_Parking.Scripts.Scopes
{
	public static class FirebaseScope
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.RegisterEntryPoint<FirebaseInitializer>().AsSelf();
			builder.Register<FirebaseAccountController>(Lifetime.Singleton).As<IAccountController>();
			builder.Register<FirebaseSaveLoadService>(Lifetime.Singleton).As<IDatabaseSaveLoadService>();
			builder.Register<ControllerFirebaseRemoteConfig>(Lifetime.Singleton).As<IAuthorizationDependant>().As<IAuthorizationFailureDependant>()
				.As<IRemoteConfigManager>();
		}
	}
}
