using System;
using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class PlayerIdUIGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PlayerIdUI cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlayerIdUI obj = (PlayerIdUI)instance;
			PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
			obj.Construct(playerDataStorage);
		}
	}
}
