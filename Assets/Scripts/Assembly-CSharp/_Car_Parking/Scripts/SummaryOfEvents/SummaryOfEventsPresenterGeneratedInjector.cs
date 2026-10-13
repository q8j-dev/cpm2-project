using System.Collections.Generic;
using CPM.Account;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.SummaryOfEvents
{
	[Preserve]
	internal class SummaryOfEventsPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SummaryOfEventsPresenter summaryOfEventsPresenter = new SummaryOfEventsPresenter((SummaryOfEventsView)resolver.ResolveOrParameter(typeof(SummaryOfEventsView), "view", parameters), (IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters), (SummaryOfEventsConfig)resolver.ResolveOrParameter(typeof(SummaryOfEventsConfig), "config", parameters), (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (IPurchaseSystem)resolver.ResolveOrParameter(typeof(IPurchaseSystem), "purchaseSystem", parameters), (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters), (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters), (ServerTimeService)resolver.ResolveOrParameter(typeof(ServerTimeService), "serverTimeService", parameters));
			Inject(summaryOfEventsPresenter, resolver, parameters);
			return summaryOfEventsPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
