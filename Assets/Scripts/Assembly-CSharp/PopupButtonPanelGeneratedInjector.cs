using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;

[Preserve]
internal class PopupButtonPanelGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:PopupButtonPanel cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		PopupButtonPanel obj = (PopupButtonPanel)instance;
		UiNotificationsManager notificationsManager = (UiNotificationsManager)resolver.ResolveOrParameter(typeof(UiNotificationsManager), "notificationsManager", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		obj.Construct(notificationsManager, managerUI);
	}
}
