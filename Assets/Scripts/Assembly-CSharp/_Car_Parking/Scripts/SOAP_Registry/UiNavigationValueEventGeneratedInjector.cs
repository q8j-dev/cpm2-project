using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	[Preserve]
	internal class UiNavigationValueEventGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			UiNavigationValueEvent uiNavigationValueEvent = new UiNavigationValueEvent();
			Inject(uiNavigationValueEvent, resolver, parameters);
			return uiNavigationValueEvent;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
