using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts
{
	[Preserve]
	internal class PatchNotesConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PatchNotesConfig patchNotesConfig = new PatchNotesConfig();
			Inject(patchNotesConfig, resolver, parameters);
			return patchNotesConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
