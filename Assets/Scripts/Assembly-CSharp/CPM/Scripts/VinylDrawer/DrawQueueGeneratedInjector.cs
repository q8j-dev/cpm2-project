using System.Collections.Generic;
using VContainer;

namespace CPM.Scripts.VinylDrawer
{
	[Preserve]
	internal class DrawQueueGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DrawQueue drawQueue = new DrawQueue((VinylDrawer)resolver.ResolveOrParameter(typeof(VinylDrawer), "vinylDrawer", parameters));
			Inject(drawQueue, resolver, parameters);
			return drawQueue;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
