using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	[Preserve]
	internal class ActiveWheelServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ActiveWheelService activeWheelService = new ActiveWheelService();
			Inject(activeWheelService, resolver, parameters);
			return activeWheelService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
