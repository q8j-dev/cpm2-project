using System;
using System.Collections.Generic;
using CPM.RemoteConfig;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI
{
	[Preserve]
	internal class MessageFromDeveloperUIGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:MessageFromDeveloperUI cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MessageFromDeveloperUI obj = (MessageFromDeveloperUI)instance;
			IRemoteConfigManager remoteConfigManager = (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters);
			obj.Construct(remoteConfigManager);
		}
	}
}
