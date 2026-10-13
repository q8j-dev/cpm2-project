using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class UiNotificationsManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			UiNotificationsManager uiNotificationsManager = new UiNotificationsManager();
			Inject(uiNotificationsManager, resolver, parameters);
			return uiNotificationsManager;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			UiNotificationsManager obj = (UiNotificationsManager)instance;
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.Construct(managerUI);
		}
	}
}
