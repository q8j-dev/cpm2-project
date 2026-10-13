using System;
using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Providers;

[Preserve]
internal class LeaderBoardGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:LeaderBoard cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		LeaderBoard obj = (LeaderBoard)instance;
		PlayerDatas.FriendsUtility friendsUtility = (PlayerDatas.FriendsUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.FriendsUtility), "friendsUtility", parameters);
		PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		obj.Construct(friendsUtility, playerDataStorage, freeDriveProvider);
	}
}
