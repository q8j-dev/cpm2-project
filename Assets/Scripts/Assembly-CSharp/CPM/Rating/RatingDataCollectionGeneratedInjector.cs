using System.Collections.Generic;
using VContainer;

namespace CPM.Rating
{
	[Preserve]
	internal class RatingDataCollectionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RatingDataCollection ratingDataCollection = new RatingDataCollection();
			Inject(ratingDataCollection, resolver, parameters);
			return ratingDataCollection;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
