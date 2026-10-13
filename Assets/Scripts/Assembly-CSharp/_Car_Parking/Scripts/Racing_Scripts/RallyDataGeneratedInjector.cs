using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	internal class RallyDataGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyData rallyData = new RallyData();
			Inject(rallyData, resolver, parameters);
			return rallyData;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
