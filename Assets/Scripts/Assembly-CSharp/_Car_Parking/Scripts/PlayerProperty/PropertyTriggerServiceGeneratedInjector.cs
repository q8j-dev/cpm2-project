using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.PlayerProperty
{
	[Preserve]
	internal class PropertyTriggerServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PropertyTriggerService propertyTriggerService = new PropertyTriggerService();
			Inject(propertyTriggerService, resolver, parameters);
			return propertyTriggerService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
