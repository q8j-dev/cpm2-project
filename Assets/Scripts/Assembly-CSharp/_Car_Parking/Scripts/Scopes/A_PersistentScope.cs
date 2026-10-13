using CPM.Scripts.VinylDrawer;
using Persistence.Systems;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Scopes
{
	[UnityEngine.Scripting.Preserve]
	public class A_PersistentScope : LifetimeScope
	{
		[SerializeField]
		private VinylDrawer vinylsDrawerPrefab = default;

		[SerializeField]
		private PersistentConfigScope PersistentConfigScope = default;

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Configure(IContainerBuilder builder)
		{
			builder.Register(typeof(OptionalDependency<>), Lifetime.Transient);
			EasySaveDataHandler.Initialize();
			ConnectionScope.Initialize(builder);
			FirebaseScope.Initialize(builder);
			AnalyticsScope.Initialize(builder);
			PlayerDataScope.Initialize(builder);
			SteamScope.Initialize(builder);
			EconomyScope.Initialize(builder);
			BackendScope.Initialize(builder);
			CoreServicesScope.Initialize(builder, vinylsDrawerPrefab);
			ArenaScope.Initialize(builder);
			GooglePlayServicesScope.Initialize(builder);
			AdsScope.Initialize(builder);
			PersistentConfigScope.Configure(builder);
			builder.RegisterEntryPoint<PersistentStartup>();
			builder.Register<SceneScopeRegistry>(Lifetime.Singleton).AsSelf();
		}
	}
}
