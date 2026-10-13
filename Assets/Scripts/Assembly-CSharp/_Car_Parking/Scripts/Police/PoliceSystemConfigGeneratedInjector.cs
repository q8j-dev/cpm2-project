using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Police
{
	[Preserve]
	internal class PoliceSystemConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PoliceSystemConfig policeSystemConfig = new PoliceSystemConfig();
			Inject(policeSystemConfig, resolver, parameters);
			return policeSystemConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
