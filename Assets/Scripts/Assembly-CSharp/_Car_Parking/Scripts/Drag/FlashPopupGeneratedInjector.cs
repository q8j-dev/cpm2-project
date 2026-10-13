using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class FlashPopupGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:FlashPopup cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FlashPopup obj = (FlashPopup)instance;
			UiNotificationsManager uiNotificationsManager = (UiNotificationsManager)resolver.ResolveOrParameter(typeof(UiNotificationsManager), "uiNotificationsManager", parameters);
			obj.Construct(uiNotificationsManager);
		}
	}
}
