using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Audio
{
	[Preserve]
	internal class SoundEventChannelGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SoundEventChannel soundEventChannel = new SoundEventChannel();
			Inject(soundEventChannel, resolver, parameters);
			return soundEventChannel;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
