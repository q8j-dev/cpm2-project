using System;
using System.Collections.Generic;
using CPM.Common;
using CPM.PlayerData;
using VContainer;

[Preserve]
internal class PlayerDatasGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:PlayerDatas cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		PlayerDatas obj = (PlayerDatas)instance;
		PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
		IChatFilterUtility chatFilterUtility = (IChatFilterUtility)resolver.ResolveOrParameter(typeof(IChatFilterUtility), "chatFilterUtility", parameters);
		obj.Construct(playerDataStorage, chatFilterUtility);
	}
}
