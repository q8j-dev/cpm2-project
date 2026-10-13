using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace CPM.Scripts.Garage
{
	[Preserve]
	internal class GarageSelectorServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageSelectorService garageSelectorService = new GarageSelectorService((ISceneLoader)resolver.ResolveOrParameter(typeof(ISceneLoader), "sceneLoader", parameters));
			Inject(garageSelectorService, resolver, parameters);
			return garageSelectorService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
