using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

[Preserve]
internal class ParkingFreeSpaceSensorGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:ParkingFreeSpaceSensor cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		ParkingFreeSpaceSensor obj = (ParkingFreeSpaceSensor)instance;
		GlobalCollectionsProvider collectionsProvider = (GlobalCollectionsProvider)resolver.ResolveOrParameter(typeof(GlobalCollectionsProvider), "collectionsProvider", parameters);
		obj.Construct(collectionsProvider);
	}
}
