using System.Collections.Generic;
using VContainer;

namespace CPM.PlayerData
{
	[Preserve]
	internal class PlayerDataStorageGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlayerDataStorage playerDataStorage = new PlayerDataStorage();
			Inject(playerDataStorage, resolver, parameters);
			return playerDataStorage;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
