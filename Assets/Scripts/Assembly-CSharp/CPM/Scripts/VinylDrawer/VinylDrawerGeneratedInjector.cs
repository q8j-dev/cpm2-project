using System;
using System.Collections.Generic;
using VContainer;

namespace CPM.Scripts.VinylDrawer
{
	[Preserve]
	internal class VinylDrawerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:VinylDrawer cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
