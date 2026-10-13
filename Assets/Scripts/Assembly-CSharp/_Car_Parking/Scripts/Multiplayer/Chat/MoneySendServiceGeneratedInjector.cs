using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	[Preserve]
	internal class MoneySendServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MoneySendService moneySendService = new MoneySendService();
			Inject(moneySendService, resolver, parameters);
			return moneySendService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MoneySendService obj = (MoneySendService)instance;
			CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
			MultiPlayer multiPlayer = (MultiPlayer)resolver.ResolveOrParameter(typeof(MultiPlayer), "multiPlayer", parameters);
			PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
			OnlineCollectionsProvider onlineCollectionsProvider = (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			ChatControl chatControl = (ChatControl)resolver.ResolveOrParameter(typeof(ChatControl), "chatControl", parameters);
			obj.Construct(currencyManagerController, multiPlayer, playerDataStorage, onlineCollectionsProvider, managerUI, freeDriveProvider, chatControl);
		}
	}
}
