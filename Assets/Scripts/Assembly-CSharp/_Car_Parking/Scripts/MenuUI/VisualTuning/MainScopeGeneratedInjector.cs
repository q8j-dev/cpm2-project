using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class MainScopeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MainScope mainScope = new MainScope();
			Inject(mainScope, resolver, parameters);
			return mainScope;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
