using System;
using System.Collections.Generic;
using VContainer;

[Preserve]
internal class RoomDataItemGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:RoomDataItem cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RoomDataItem obj = (RoomDataItem)instance;
		PlayerDatas.FriendsUtility friendsUtility = (PlayerDatas.FriendsUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.FriendsUtility), "friendsUtility", parameters);
		MultiPlayerMenu multiPlayerMenu = (MultiPlayerMenu)resolver.ResolveOrParameter(typeof(MultiPlayerMenu), "multiPlayerMenu", parameters);
		obj.Construct(friendsUtility, multiPlayerMenu);
	}
}
