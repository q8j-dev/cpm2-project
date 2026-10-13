using System.Collections.Generic;
using CPM.Database;
using CPM.RemoteConfig;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Garage.GarageSellService
{
	[Preserve]
	internal class GarageSellServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageSellService garageSellService = new GarageSellService((IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters), (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters), (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters), (GaragePropertyAnalytic)resolver.ResolveOrParameter(typeof(GaragePropertyAnalytic), "garagePropertyAnalytic", parameters), (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters), (OptionalDependency<DataBase>)resolver.ResolveOrParameter(typeof(OptionalDependency<DataBase>), "dataBase", parameters));
			Inject(garageSellService, resolver, parameters);
			return garageSellService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
