using System;
using System.Collections.Generic;
using VContainer;

namespace CPMEngine
{
	[Preserve]
	internal class WheelProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:WheelProvider cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			WheelProvider obj = (WheelProvider)instance;
			TemperatureTyresInfo temperatureTyresInfo = (TemperatureTyresInfo)resolver.ResolveOrParameter(typeof(TemperatureTyresInfo), "temperatureTyresInfo", parameters);
			obj.Construct(temperatureTyresInfo);
		}
	}
}
