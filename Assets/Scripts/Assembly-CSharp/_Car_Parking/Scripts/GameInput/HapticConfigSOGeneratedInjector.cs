using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.GameInput
{
	[Preserve]
	internal class HapticConfigSOGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			HapticConfigSO hapticConfigSO = new HapticConfigSO();
			Inject(hapticConfigSO, resolver, parameters);
			return hapticConfigSO;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
