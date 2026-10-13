using System;
using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Other.AntiCheat
{
	[Preserve]
	internal class CarDebugToolsGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:CarDebugTools cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarDebugTools obj = (CarDebugTools)instance;
			PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			obj.Construct(playerDataStorage, freeDriveProvider);
		}
	}
}
