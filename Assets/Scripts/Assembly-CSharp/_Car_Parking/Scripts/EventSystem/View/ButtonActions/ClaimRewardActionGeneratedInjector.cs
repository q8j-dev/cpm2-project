using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.EventSystem.View.ButtonActions
{
	[Preserve]
	internal class ClaimRewardActionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ClaimRewardAction claimRewardAction = new ClaimRewardAction((IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters), (CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters), (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters));
			Inject(claimRewardAction, resolver, parameters);
			return claimRewardAction;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
