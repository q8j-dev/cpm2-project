using System;
using System.Collections.Generic;
using VContainer;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	[Preserve]
	internal class MirrorCompositionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:MirrorComposition cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
