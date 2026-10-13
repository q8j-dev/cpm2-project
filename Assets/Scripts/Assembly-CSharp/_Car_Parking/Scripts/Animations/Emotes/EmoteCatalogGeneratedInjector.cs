using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Animations.Emotes
{
	[Preserve]
	internal class EmoteCatalogGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EmoteCatalog emoteCatalog = new EmoteCatalog((AnimationDataSO)resolver.ResolveOrParameter(typeof(AnimationDataSO), "data", parameters), (PoseCatalogSO)resolver.ResolveOrParameter(typeof(PoseCatalogSO), "poseCatalog", parameters));
			Inject(emoteCatalog, resolver, parameters);
			return emoteCatalog;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
