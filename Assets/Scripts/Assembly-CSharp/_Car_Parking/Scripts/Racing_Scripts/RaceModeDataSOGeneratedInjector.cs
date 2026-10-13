using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	internal class RaceModeDataSOGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RaceModeDataSO raceModeDataSO = new RaceModeDataSO();
			Inject(raceModeDataSO, resolver, parameters);
			return raceModeDataSO;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
