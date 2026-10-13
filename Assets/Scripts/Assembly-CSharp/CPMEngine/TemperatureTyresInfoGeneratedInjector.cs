using System.Collections.Generic;
using VContainer;

namespace CPMEngine
{
	[Preserve]
	internal class TemperatureTyresInfoGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TemperatureTyresInfo temperatureTyresInfo = new TemperatureTyresInfo();
			Inject(temperatureTyresInfo, resolver, parameters);
			return temperatureTyresInfo;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
