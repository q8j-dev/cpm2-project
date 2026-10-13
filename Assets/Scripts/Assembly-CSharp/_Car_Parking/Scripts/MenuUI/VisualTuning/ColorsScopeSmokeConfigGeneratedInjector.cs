using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class ColorsScopeSmokeConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ColorsScopeSmokeConfig colorsScopeSmokeConfig = new ColorsScopeSmokeConfig();
			Inject(colorsScopeSmokeConfig, resolver, parameters);
			return colorsScopeSmokeConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
