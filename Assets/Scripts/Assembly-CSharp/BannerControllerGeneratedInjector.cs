using System;
using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Providers;

[Preserve]
internal class BannerControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:BannerController cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		BannerController obj = (BannerController)instance;
		PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		obj.Construct(playerDataStorage, freeDriveProvider);
	}
}
