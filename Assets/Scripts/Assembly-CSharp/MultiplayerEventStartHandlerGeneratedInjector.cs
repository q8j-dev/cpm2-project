using System;
using System.Collections.Generic;
using CPM.Connection;
using VContainer;
using _Car_Parking.Scripts.Drag;

[Preserve]
internal class MultiplayerEventStartHandlerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:MultiplayerEventStartHandler cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		MultiplayerEventStartHandler obj = (MultiplayerEventStartHandler)instance;
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		PhotonReconnectionHandler photonReconnectionHandler = (PhotonReconnectionHandler)resolver.ResolveOrParameter(typeof(PhotonReconnectionHandler), "photonReconnectionHandler", parameters);
		obj.Construct(managerUI, photonReconnectionHandler);
	}
}
