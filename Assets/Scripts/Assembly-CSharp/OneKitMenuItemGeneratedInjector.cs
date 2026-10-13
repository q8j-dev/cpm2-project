using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Notification;

[Preserve]
internal class OneKitMenuItemGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:OneKitMenuItem cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		OneKitMenuItem obj = (OneKitMenuItem)instance;
		CharactersService charactersService = (CharactersService)resolver.ResolveOrParameter(typeof(CharactersService), "charactersService", parameters);
		ICharacterEquipmentDataService characterEquipmentDataService = (ICharacterEquipmentDataService)resolver.ResolveOrParameter(typeof(ICharacterEquipmentDataService), "characterEquipmentDataService", parameters);
		PersonKitConfigs personKitConfigs = (PersonKitConfigs)resolver.ResolveOrParameter(typeof(PersonKitConfigs), "personKitConfigs", parameters);
		INotificationService notificationService = (INotificationService)resolver.ResolveOrParameter(typeof(INotificationService), "notificationService", parameters);
		PersonClothesEventChecker personClothesEventChecker = (PersonClothesEventChecker)resolver.ResolveOrParameter(typeof(PersonClothesEventChecker), "personClothesEventChecker", parameters);
		obj.Construct(charactersService, characterEquipmentDataService, personKitConfigs, notificationService, personClothesEventChecker);
	}
}
