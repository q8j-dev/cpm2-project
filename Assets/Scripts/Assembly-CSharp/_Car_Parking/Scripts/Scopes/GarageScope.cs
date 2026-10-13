using CPM.Scripts.Garage;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.DiscordPresence;

namespace _Car_Parking.Scripts.Scopes
{
	public class GarageScope : LifetimeScope
	{
		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterEntryPoint<DiscordTuningPresence>().WithParameter("idleKind", DiscordActivityKind.Garage);
			builder.RegisterEntryPoint<DiscordScenePresence>().WithParameter("kind", DiscordActivityKind.Garage).WithParameter("reportEnvironment", false);
			builder.Register<MenuGarage>(Lifetime.Singleton).As<IGarageEntity>();
		}
	}
}
