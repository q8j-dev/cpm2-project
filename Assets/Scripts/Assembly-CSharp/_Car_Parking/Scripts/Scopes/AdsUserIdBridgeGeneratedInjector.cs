using System.Collections.Generic;
using CPM.Ads;
using CPM.Ads.Policy;
using CPM.Balance;
using CPM.RemoteConfig;
using VContainer;

namespace _Car_Parking.Scripts.Scopes
{
	[Preserve]
	internal class AdsUserIdBridgeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AdsUserIdBridge adsUserIdBridge = new AdsUserIdBridge((IAdsUserIdentity)resolver.ResolveOrParameter(typeof(IAdsUserIdentity), "adsIdentity", parameters), (IAdsService)resolver.ResolveOrParameter(typeof(IAdsService), "ads", parameters), (IAdPlacementCatalog)resolver.ResolveOrParameter(typeof(IAdPlacementCatalog), "catalog", parameters), (IBalanceDocumentStore)resolver.ResolveOrParameter(typeof(IBalanceDocumentStore), "documents", parameters), (IBalanceDocumentSynchronizer)resolver.ResolveOrParameter(typeof(IBalanceDocumentSynchronizer), "documentSync", parameters), (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfig", parameters));
			Inject(adsUserIdBridge, resolver, parameters);
			return adsUserIdBridge;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
