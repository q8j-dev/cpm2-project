using System.Collections.Generic;
using VContainer;

namespace Racing.Scripts.UserColors
{
	[Preserve]
	internal class SavedColorCollectionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SavedColorCollection savedColorCollection = new SavedColorCollection();
			Inject(savedColorCollection, resolver, parameters);
			return savedColorCollection;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
