using System;
using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace CPM.Leaderboard.UI
{
	[Preserve]
	internal class LeaderboardListViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:LeaderboardListView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			LeaderboardListView obj = (LeaderboardListView)instance;
			PlayerDataStorage playerData = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerData", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			obj.Construct(playerData, prefabFactory);
		}
	}
}
