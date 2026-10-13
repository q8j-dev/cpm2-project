using System;
using System.Collections.Generic;
using CPM.Ads.Policy;
using CPM.Database;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class FuelTankGadgetGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:FuelTankGadget cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FuelTankGadget obj = (FuelTankGadget)instance;
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			ICarTuningFactory carTuningFactory = (ICarTuningFactory)resolver.ResolveOrParameter(typeof(ICarTuningFactory), "carTuningFactory", parameters);
			PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
			IPricesService pricesService = (IPricesService)resolver.ResolveOrParameter(typeof(IPricesService), "pricesService", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			VirtualPurchaseAnalytic virtualPurchaseAnalytic = (VirtualPurchaseAnalytic)resolver.ResolveOrParameter(typeof(VirtualPurchaseAnalytic), "virtualPurchaseAnalytic", parameters);
			IAdOfferService adOffers = (IAdOfferService)resolver.ResolveOrParameter(typeof(IAdOfferService), "adOffers", parameters);
			IClientAdRewardConsumeService adRewardConsume = (IClientAdRewardConsumeService)resolver.ResolveOrParameter(typeof(IClientAdRewardConsumeService), "adRewardConsume", parameters);
			obj.Construct(carInfoService, carTuningFactory, playerDataStorage, managerUI, currencyManagerController, pricesService, freeDriveProvider, virtualPurchaseAnalytic, adOffers, adRewardConsume);
		}
	}
}
