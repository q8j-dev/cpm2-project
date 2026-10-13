using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.CommonUtility.Contracts;

namespace CPM.Connection
{
	[Preserve]
	internal class PhotonConnectionHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PhotonConnectionHandler photonConnectionHandler = new PhotonConnectionHandler((PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters), (IPhotonCarValidator)resolver.ResolveOrParameter(typeof(IPhotonCarValidator), "photonCarValidator", parameters));
			Inject(photonConnectionHandler, resolver, parameters);
			return photonConnectionHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
