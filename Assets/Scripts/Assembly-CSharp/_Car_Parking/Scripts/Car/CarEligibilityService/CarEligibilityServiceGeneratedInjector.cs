using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.Car.CarEligibilityService
{
	[Preserve]
	internal class CarEligibilityServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarEligibilityService carEligibilityService = new CarEligibilityService((AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters), (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters), (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters), (OptionalDependency<ITimerTuningService>)resolver.ResolveOrParameter(typeof(OptionalDependency<ITimerTuningService>), "timerTuningService", parameters), (IGameFactManager)resolver.ResolveOrParameter(typeof(IGameFactManager), "gameFactManager", parameters));
			Inject(carEligibilityService, resolver, parameters);
			return carEligibilityService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
