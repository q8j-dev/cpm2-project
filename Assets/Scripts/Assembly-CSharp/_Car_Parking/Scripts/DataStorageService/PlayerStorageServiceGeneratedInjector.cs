using System.Collections.Generic;
using CPM.Account;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Characters;

namespace _Car_Parking.Scripts.DataStorageService
{
	[Preserve]
	internal class PlayerStorageServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlayerStorageService playerStorageService = new PlayerStorageService();
			Inject(playerStorageService, resolver, parameters);
			return playerStorageService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlayerStorageService obj = (PlayerStorageService)instance;
			PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
			IDeleterUserDataGetter deleterUserDataGetter = (IDeleterUserDataGetter)resolver.ResolveOrParameter(typeof(IDeleterUserDataGetter), "deleterUserDataGetter", parameters);
			PresetGrantSession presetGrantSession = (PresetGrantSession)resolver.ResolveOrParameter(typeof(PresetGrantSession), "presetGrantSession", parameters);
			obj.Construct(playerDataStorage, deleterUserDataGetter, presetGrantSession);
		}
	}
}
