using System;
using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

namespace Car_Parking.Scripts.Racing
{
	[Preserve]
	internal class UITimeBannerControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:UITimeBannerController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			UITimeBannerController obj = (UITimeBannerController)instance;
			OnlineCollectionsProvider onlineCollectionsProvider = (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters);
			PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			OptionalDependency<DragRacingController> dragRacingController = (OptionalDependency<DragRacingController>)resolver.ResolveOrParameter(typeof(OptionalDependency<DragRacingController>), "dragRacingController", parameters);
			obj.Construct(onlineCollectionsProvider, playerDataStorage, freeDriveProvider, dragRacingController);
		}
	}
}
