using System;
using System.Collections.Generic;
using CPM.RemoteConfig;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EventSystem.View
{
	[Preserve]
	internal class TimeAttackBannerHelperGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:TimeAttackBannerHelper cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TimeAttackBannerHelper obj = (TimeAttackBannerHelper)instance;
			IRemoteConfigManager remoteConfigManager = (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			Camera mainCamera = (Camera)resolver.ResolveOrParameter(typeof(Camera), "mainCamera", parameters);
			OptionalDependency<IAutoSaveService> autoSaveService = (OptionalDependency<IAutoSaveService>)resolver.ResolveOrParameter(typeof(OptionalDependency<IAutoSaveService>), "autoSaveService", parameters);
			obj.Construct(remoteConfigManager, managerUI, mainCamera, autoSaveService);
		}
	}
}
