using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Rating
{
	[Preserve]
	internal class FirebaseRatingServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FirebaseRatingService firebaseRatingService = new FirebaseRatingService();
			Inject(firebaseRatingService, resolver, parameters);
			return firebaseRatingService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
