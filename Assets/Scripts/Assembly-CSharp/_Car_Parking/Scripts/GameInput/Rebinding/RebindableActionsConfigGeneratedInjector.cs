using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	[Preserve]
	internal class RebindableActionsConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RebindableActionsConfig rebindableActionsConfig = new RebindableActionsConfig();
			Inject(rebindableActionsConfig, resolver, parameters);
			return rebindableActionsConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
