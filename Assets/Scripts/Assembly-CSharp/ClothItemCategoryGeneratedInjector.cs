using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.Offers.Reward;

[Preserve]
internal class ClothItemCategoryGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:ClothItemCategory cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		ClothItemCategory obj = (ClothItemCategory)instance;
		PersonKitConfigs personKitConfigs = (PersonKitConfigs)resolver.ResolveOrParameter(typeof(PersonKitConfigs), "personKitConfigs", parameters);
		ICharacterEquipmentDataService characterEquipmentDataService = (ICharacterEquipmentDataService)resolver.ResolveOrParameter(typeof(ICharacterEquipmentDataService), "characterEquipmentDataService", parameters);
		INotificationService notificationService = (INotificationService)resolver.ResolveOrParameter(typeof(INotificationService), "notificationService", parameters);
		BundleOfferCatalog bundleOfferCatalog = (BundleOfferCatalog)resolver.ResolveOrParameter(typeof(BundleOfferCatalog), "bundleOfferCatalog", parameters);
		obj.Construct(personKitConfigs, characterEquipmentDataService, notificationService, bundleOfferCatalog);
	}
}
