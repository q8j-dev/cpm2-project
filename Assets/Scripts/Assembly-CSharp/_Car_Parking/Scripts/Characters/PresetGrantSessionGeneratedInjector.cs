using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Characters
{
	[Preserve]
	internal class PresetGrantSessionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PresetGrantSession presetGrantSession = new PresetGrantSession();
			Inject(presetGrantSession, resolver, parameters);
			return presetGrantSession;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
