using System.Collections.Generic;
using CPM.Account;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.EngineTuning;

namespace CPM.Database
{
	[Preserve]
	internal class CarInfoServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarInfoService carInfoService = new CarInfoService((ICarTuningFactory)resolver.ResolveOrParameter(typeof(ICarTuningFactory), "carTuningFactory", parameters), (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters), (IDatabaseCarsService)resolver.ResolveOrParameter(typeof(IDatabaseCarsService), "databaseCarsService", parameters), (IDeleterUserDataGetter)resolver.ResolveOrParameter(typeof(IDeleterUserDataGetter), "deleterUserDataGetter", parameters));
			Inject(carInfoService, resolver, parameters);
			return carInfoService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
