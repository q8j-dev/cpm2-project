using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	[Preserve]
	internal class EventCollectablesPopupConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventCollectablesPopupConfig eventCollectablesPopupConfig = new EventCollectablesPopupConfig();
			Inject(eventCollectablesPopupConfig, resolver, parameters);
			return eventCollectablesPopupConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
