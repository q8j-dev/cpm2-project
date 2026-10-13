using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Audio
{
	[Preserve]
	internal class RainSoundStateGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RainSoundState rainSoundState = new RainSoundState();
			Inject(rainSoundState, resolver, parameters);
			return rainSoundState;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
