using System.Collections.Generic;
using VContainer;

namespace Flags
{
	[Preserve]
	internal class FlagsDataSOGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FlagsDataSO flagsDataSO = new FlagsDataSO();
			Inject(flagsDataSO, resolver, parameters);
			return flagsDataSO;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
