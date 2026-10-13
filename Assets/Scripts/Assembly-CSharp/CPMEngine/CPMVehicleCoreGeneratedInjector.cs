using System;
using System.Collections.Generic;
using VContainer;

namespace CPMEngine
{
	[Preserve]
	internal class CPMVehicleCoreGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:CPMVehicleCore cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
