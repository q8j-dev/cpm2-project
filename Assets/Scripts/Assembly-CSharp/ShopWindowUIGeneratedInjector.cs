using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.CarSlots;

[Preserve]
internal class ShopWindowUIGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:ShopWindowUI cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		ShopWindowUI obj = (ShopWindowUI)instance;
		MultiPlayerMenu multiPlayerMenu = (MultiPlayerMenu)resolver.ResolveOrParameter(typeof(MultiPlayerMenu), "multiPlayerMenu", parameters);
		CharactersService charactersService = (CharactersService)resolver.ResolveOrParameter(typeof(CharactersService), "charactersService", parameters);
		PersonKitConfigs personKitConfigs = (PersonKitConfigs)resolver.ResolveOrParameter(typeof(PersonKitConfigs), "personKitConfigs", parameters);
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
		obj.Construct(multiPlayerMenu, charactersService, personKitConfigs, currencyManagerController, slotService);
	}
}
