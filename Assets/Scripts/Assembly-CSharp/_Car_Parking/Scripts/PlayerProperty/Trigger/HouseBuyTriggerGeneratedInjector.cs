using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.PlayerProperty.Trigger
{
	[Preserve]
	internal class HouseBuyTriggerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:HouseBuyTrigger cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			HouseBuyTrigger obj = (HouseBuyTrigger)instance;
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
			PropertyTriggerService propertyTriggerService = (PropertyTriggerService)resolver.ResolveOrParameter(typeof(PropertyTriggerService), "propertyTriggerService", parameters);
			PropertyConfig propertyConfig = (PropertyConfig)resolver.ResolveOrParameter(typeof(PropertyConfig), "propertyConfig", parameters);
			OptionalDependency<TriggerCore> triggerCoreDep = (OptionalDependency<TriggerCore>)resolver.ResolveOrParameter(typeof(OptionalDependency<TriggerCore>), "triggerCoreDep", parameters);
			obj.Construct(freeDriveProvider, menuLevelParent, propertyTriggerService, propertyConfig, triggerCoreDep);
		}
	}
}
