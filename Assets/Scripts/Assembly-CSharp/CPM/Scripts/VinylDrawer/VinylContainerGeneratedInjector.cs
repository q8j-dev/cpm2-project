using System;
using System.Collections.Generic;
using VContainer;

namespace CPM.Scripts.VinylDrawer
{
	[Preserve]
	internal class VinylContainerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:VinylContainer cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VinylContainer obj = (VinylContainer)instance;
			VinylDrawer vinylDrawer = (VinylDrawer)resolver.ResolveOrParameter(typeof(VinylDrawer), "vinylDrawer", parameters);
			obj.Construct(vinylDrawer);
		}
	}
}
