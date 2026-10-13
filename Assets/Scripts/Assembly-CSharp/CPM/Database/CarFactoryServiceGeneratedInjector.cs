using System.Collections.Generic;
using CPM.Scripts.VinylDrawer;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.CarLegitService;
using _Car_Parking.Scripts.Providers;

namespace CPM.Database
{
	[Preserve]
	internal class CarFactoryServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarFactoryService carFactoryService = new CarFactoryService((ICarTuningFactory)resolver.ResolveOrParameter(typeof(ICarTuningFactory), "carTuningFactory", parameters), (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters), (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters), (DrawQueue)resolver.ResolveOrParameter(typeof(DrawQueue), "drawQueue", parameters), (IPlayerStorageService)resolver.ResolveOrParameter(typeof(IPlayerStorageService), "playerStorageService", parameters), (IProgressBackend)resolver.ResolveOrParameter(typeof(IProgressBackend), "progressBackend", parameters), (ICarLegitService)resolver.ResolveOrParameter(typeof(ICarLegitService), "carLegitService", parameters));
			Inject(carFactoryService, resolver, parameters);
			return carFactoryService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
