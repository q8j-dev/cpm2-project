using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Police;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class PoliceVisualScopeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PoliceVisualScope policeVisualScope = new PoliceVisualScope((CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "walletController", parameters), (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters), (PoliceSystemConfig)resolver.ResolveOrParameter(typeof(PoliceSystemConfig), "policeSystemConfig", parameters), (IPoliceSystem)resolver.ResolveOrParameter(typeof(IPoliceSystem), "policeSystem", parameters));
			Inject(policeVisualScope, resolver, parameters);
			return policeVisualScope;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
