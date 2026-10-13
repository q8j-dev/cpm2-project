using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;

[Preserve]
internal class AnimatedTimePanelGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:AnimatedTimePanel cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		AnimatedTimePanel obj = (AnimatedTimePanel)instance;
		UiNotificationsManager uiNotificationsManager = (UiNotificationsManager)resolver.ResolveOrParameter(typeof(UiNotificationsManager), "uiNotificationsManager", parameters);
		obj.Construct(uiNotificationsManager);
	}
}
