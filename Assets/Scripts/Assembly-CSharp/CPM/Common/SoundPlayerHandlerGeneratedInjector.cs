using System.Collections.Generic;
using VContainer;

namespace CPM.Common
{
	[Preserve]
	internal class SoundPlayerHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SoundPlayerHandler soundPlayerHandler = new SoundPlayerHandler();
			Inject(soundPlayerHandler, resolver, parameters);
			return soundPlayerHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
