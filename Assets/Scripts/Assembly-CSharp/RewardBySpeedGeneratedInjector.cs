using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Steam.Base;

[Preserve]
internal class RewardBySpeedGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:RewardBySpeed cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RewardBySpeed obj = (RewardBySpeed)instance;
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		IEarnMoneyService earnMoneyService = (IEarnMoneyService)resolver.ResolveOrParameter(typeof(IEarnMoneyService), "earnMoneyService", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
		obj.Construct(currencyManagerController, earnMoneyService, managerUI, steamAchievementService);
	}
}
