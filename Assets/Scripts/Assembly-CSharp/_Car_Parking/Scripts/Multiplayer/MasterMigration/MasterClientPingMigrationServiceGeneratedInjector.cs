using System;
using System.Collections.Generic;
using CPM.RemoteConfig;
using VContainer;
using _Car_Parking.Scripts.Multiplayer.RequestResponseService;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.MasterMigration
{
	[Preserve]
	internal class MasterClientPingMigrationServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:MasterClientPingMigrationService cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MasterClientPingMigrationService obj = (MasterClientPingMigrationService)instance;
			IRemoteConfigManager remoteConfig = (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfig", parameters);
			RpcRrService rpcRr = (RpcRrService)resolver.ResolveOrParameter(typeof(RpcRrService), "rpcRr", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			obj.Construct(remoteConfig, rpcRr, freeDriveProvider);
		}
	}
}
