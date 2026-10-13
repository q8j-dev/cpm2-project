using VContainer;
using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.GameActivity.Providers;

namespace _Car_Parking.Scripts.Scopes
{
	public static class ActivitiesScopeUtility
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<ArenaActivityProvider>(Lifetime.Singleton).As<IGameActivityProvider>();
			builder.Register<GameActivityCatalog>(Lifetime.Singleton).As<IGameActivityCatalog>();
		}
	}
}
