using System;
using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class PlayerListWindowPlayerUserViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PlayerListWindowPlayerUserView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlayerListWindowPlayerUserView obj = (PlayerListWindowPlayerUserView)instance;
			PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
			obj.Construct(playerDataStorage);
		}
	}
}
