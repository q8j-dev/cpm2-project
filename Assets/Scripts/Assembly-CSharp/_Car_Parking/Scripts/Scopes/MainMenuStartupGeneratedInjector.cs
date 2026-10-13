using System.Collections.Generic;
using CPM.Database;
using Persistence.Systems;
using Rating.DailyTasks.Scripts;
using VContainer;
using _Car_Parking.Scripts.ArtistTools;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Scopes
{
	[Preserve]
	internal class MainMenuStartupGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MainMenuStartup mainMenuStartup = new MainMenuStartup((DataBase)resolver.ResolveOrParameter(typeof(DataBase), "dataBase", parameters), (IDailyServicesDependant)resolver.ResolveOrParameter(typeof(IDailyServicesDependant), "dailyServicesDependant", parameters), (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters), (CharactersService)resolver.ResolveOrParameter(typeof(CharactersService), "charactersService", parameters), (PrefabFactoryLinkage)resolver.ResolveOrParameter(typeof(PrefabFactoryLinkage), "prefabFactoryLinkage", parameters), (CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters), (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters), (GlobalCollectionsProvider)resolver.ResolveOrParameter(typeof(GlobalCollectionsProvider), "globalCollectionsProvider", parameters), (IOfflineModeSystem)resolver.ResolveOrParameter(typeof(IOfflineModeSystem), "offlineModeSystem", parameters), (ICheckCarDataBase)resolver.ResolveOrParameter(typeof(ICheckCarDataBase), "checkCarDataBase", parameters), (VfxPoolProvider)resolver.ResolveOrParameter(typeof(VfxPoolProvider), "vfxPoolProvider", parameters), (INotificationService)resolver.ResolveOrParameter(typeof(INotificationService), "notificationService", parameters), (ShadersVaultConfig)resolver.ResolveOrParameter(typeof(ShadersVaultConfig), "shadersVaultConfig", parameters));
			Inject(mainMenuStartup, resolver, parameters);
			return mainMenuStartup;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
