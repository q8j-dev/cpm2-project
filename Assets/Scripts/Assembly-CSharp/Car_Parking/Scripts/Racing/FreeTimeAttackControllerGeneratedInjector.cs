using System.Collections.Generic;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Base;

namespace Car_Parking.Scripts.Racing
{
	[Preserve]
	internal class FreeTimeAttackControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FreeTimeAttackController freeTimeAttackController = new FreeTimeAttackController();
			Inject(freeTimeAttackController, resolver, parameters);
			return freeTimeAttackController;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FreeTimeAttackController obj = (FreeTimeAttackController)instance;
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			EventsAnalytic eventsAnalytic = (EventsAnalytic)resolver.ResolveOrParameter(typeof(EventsAnalytic), "eventsAnalytic", parameters);
			MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
			LevelScript levelScript = (LevelScript)resolver.ResolveOrParameter(typeof(LevelScript), "levelScript", parameters);
			FreeDriveControl freeDriveControl = (FreeDriveControl)resolver.ResolveOrParameter(typeof(FreeDriveControl), "freeDriveControl", parameters);
			UiNotificationsManager uiNotificationsManager = (UiNotificationsManager)resolver.ResolveOrParameter(typeof(UiNotificationsManager), "uiNotificationsManager", parameters);
			TimeBannersManager timeBannersManager = (TimeBannersManager)resolver.ResolveOrParameter(typeof(TimeBannersManager), "timeBannersManager", parameters);
			EventRestrictionService eventRestrictionService = (EventRestrictionService)resolver.ResolveOrParameter(typeof(EventRestrictionService), "eventRestrictionService", parameters);
			RacingProvider racingProvider = (RacingProvider)resolver.ResolveOrParameter(typeof(RacingProvider), "racingProvider", parameters);
			SoundBankLoadService soundBankLoadService = (SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "soundBankLoadService", parameters);
			OptionalDependency<TimeAttackDataSO> timeAttackDataSo = (OptionalDependency<TimeAttackDataSO>)resolver.ResolveOrParameter(typeof(OptionalDependency<TimeAttackDataSO>), "timeAttackDataSo", parameters);
			OptionalDependency<FreeDriveProvider> freeDriveProvider = (OptionalDependency<FreeDriveProvider>)resolver.ResolveOrParameter(typeof(OptionalDependency<FreeDriveProvider>), "freeDriveProvider", parameters);
			ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
			obj.Construct(managerUI, prefabFactory, eventsAnalytic, menuLevelParent, levelScript, freeDriveControl, uiNotificationsManager, timeBannersManager, eventRestrictionService, racingProvider, soundBankLoadService, timeAttackDataSo, freeDriveProvider, steamAchievementService);
		}
	}
}
