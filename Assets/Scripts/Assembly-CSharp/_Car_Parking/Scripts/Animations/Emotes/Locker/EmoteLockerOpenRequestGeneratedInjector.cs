using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	[Preserve]
	internal class EmoteLockerOpenRequestGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EmoteLockerOpenRequest emoteLockerOpenRequest = new EmoteLockerOpenRequest();
			Inject(emoteLockerOpenRequest, resolver, parameters);
			return emoteLockerOpenRequest;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
