using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	[Preserve]
	internal class BootStartDataGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BootStartData bootStartData = new BootStartData();
			Inject(bootStartData, resolver, parameters);
			return bootStartData;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
