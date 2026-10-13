using CodeBase.Infrastructure.States;
using VContainer;
using _Car_Parking.Scripts.Infrastructure.States;

namespace _Car_Parking.Scripts.Scopes
{
	public static class GameStatesScopeUtility
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<InitMainMenuState>(Lifetime.Singleton);
			builder.Register<MainMenuState>(Lifetime.Singleton);
			builder.Register<GameLoopState>(Lifetime.Singleton);
			builder.Register<InitFreeDriveState>(Lifetime.Singleton);
			builder.Register<VContainerGameStateMachine>(Lifetime.Singleton).As<IGameStateMachine>();
		}
	}
}
